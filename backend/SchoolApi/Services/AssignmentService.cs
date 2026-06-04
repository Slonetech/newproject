using Microsoft.EntityFrameworkCore;
using SchoolApi.Data;
using SchoolApi.Models;
using SchoolApi.Models.DTOs;

namespace SchoolApi.Services
{
    /// <summary>
    /// Service for assignment-related business logic
    /// </summary>
    public class AssignmentService : IAssignmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<AssignmentService> _logger;

        public AssignmentService(
            ApplicationDbContext context,
            IEmailService emailService,
            ILogger<AssignmentService> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<IEnumerable<Assignment>> GetAllAssignmentsAsync()
        {
            return await _context.Assignments
                .Include(a => a.Course)
                .Include(a => a.Teacher)
                .Include(a => a.Submissions)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Assignment?> GetAssignmentByIdAsync(Guid id)
        {
            return await _context.Assignments
                .Include(a => a.Course)
                .Include(a => a.Teacher)
                .Include(a => a.Submissions)
                    .ThenInclude(s => s.Student)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Assignment>> GetCourseAssignmentsAsync(Guid courseId)
        {
            return await _context.Assignments
                .Include(a => a.Teacher)
                .Include(a => a.Submissions)
                .Where(a => a.CourseId == courseId && a.IsActive)
                .OrderByDescending(a => a.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Assignment>> GetTeacherAssignmentsAsync(Guid teacherId)
        {
            return await _context.Assignments
                .Include(a => a.Course)
                .Include(a => a.Submissions)
                .Where(a => a.TeacherId == teacherId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Assignment> CreateAssignmentAsync(AssignmentDto dto, Guid teacherId)
        {
            // Validate course exists
            var course = await _context.Courses
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.Id == dto.CourseId);

            if (course == null)
            {
                throw new KeyNotFoundException($"Course with ID {dto.CourseId} not found");
            }

            var assignment = new Assignment
            {
                Title = dto.Title,
                Description = dto.Description,
                CourseId = dto.CourseId,
                TeacherId = teacherId,
                DueDate = dto.DueDate,
                MaxPoints = dto.MaxPoints,
                AssignmentType = dto.AssignmentType,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();

            // Notify enrolled students
            await NotifyStudentsAsync(course, assignment);

            _logger.LogInformation("Created assignment {AssignmentId} for course {CourseId} by teacher {TeacherId}",
                assignment.Id, dto.CourseId, teacherId);

            return assignment;
        }

        public async Task<Assignment> UpdateAssignmentAsync(Guid id, AssignmentDto dto)
        {
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null)
            {
                throw new KeyNotFoundException($"Assignment with ID {id} not found");
            }

            assignment.Title = dto.Title;
            assignment.Description = dto.Description;
            assignment.DueDate = dto.DueDate;
            assignment.MaxPoints = dto.MaxPoints;
            assignment.AssignmentType = dto.AssignmentType;
            assignment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated assignment {AssignmentId}", id);

            return assignment;
        }

        public async Task DeleteAssignmentAsync(Guid id)
        {
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null)
            {
                throw new KeyNotFoundException($"Assignment with ID {id} not found");
            }

            // Soft delete
            assignment.IsActive = false;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Soft deleted assignment {AssignmentId}", id);
        }

        public async Task<AssignmentSubmission> SubmitAssignmentAsync(Guid assignmentId, Guid studentId, string submissionText, string? fileUrl)
        {
            var assignment = await _context.Assignments.FindAsync(assignmentId);
            if (assignment == null)
            {
                throw new KeyNotFoundException($"Assignment with ID {assignmentId} not found");
            }

            var student = await _context.Students.FindAsync(studentId);
            if (student == null)
            {
                throw new KeyNotFoundException($"Student with ID {studentId} not found");
            }

            // Check if student already submitted
            var existingSubmission = await _context.AssignmentSubmissions
                .FirstOrDefaultAsync(s => s.AssignmentId == assignmentId && s.StudentId == studentId);

            if (existingSubmission != null)
            {
                throw new InvalidOperationException("Student has already submitted this assignment");
            }

            var isLate = DateTime.UtcNow > assignment.DueDate;

            var submission = new AssignmentSubmission
            {
                AssignmentId = assignmentId,
                StudentId = studentId,
                SubmissionText = submissionText,
                FileUrl = fileUrl,
                SubmittedAt = DateTime.UtcNow,
                IsLate = isLate
            };

            _context.AssignmentSubmissions.Add(submission);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Student {StudentId} submitted assignment {AssignmentId} (Late: {IsLate})",
                studentId, assignmentId, isLate);

            return submission;
        }

        public async Task<AssignmentSubmission> GradeSubmissionAsync(Guid submissionId, double grade, string? feedback)
        {
            var submission = await _context.AssignmentSubmissions
                .Include(s => s.Assignment)
                .Include(s => s.Student)
                .FirstOrDefaultAsync(s => s.Id == submissionId);

            if (submission == null)
            {
                throw new KeyNotFoundException($"Submission with ID {submissionId} not found");
            }

            if (grade < 0 || grade > submission.Assignment.MaxPoints)
            {
                throw new ArgumentException($"Grade must be between 0 and {submission.Assignment.MaxPoints}");
            }

            submission.Grade = grade;
            submission.TeacherFeedback = feedback;
            submission.GradedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Graded submission {SubmissionId} with grade {Grade}", submissionId, grade);

            return submission;
        }

        public async Task<IEnumerable<AssignmentSubmission>> GetAssignmentSubmissionsAsync(Guid assignmentId)
        {
            return await _context.AssignmentSubmissions
                .Include(s => s.Student)
                .Where(s => s.AssignmentId == assignmentId)
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();
        }

        public async Task<AssignmentSubmission?> GetStudentSubmissionAsync(Guid assignmentId, Guid studentId)
        {
            return await _context.AssignmentSubmissions
                .Include(s => s.Assignment)
                .FirstOrDefaultAsync(s => s.AssignmentId == assignmentId && s.StudentId == studentId);
        }

        private async Task NotifyStudentsAsync(Course course, Assignment assignment)
        {
            foreach (var enrollment in course.Enrollments)
            {
                try
                {
                    await _emailService.SendAssignmentNotificationAsync(
                        enrollment.Student.Email,
                        enrollment.Student.FirstName,
                        enrollment.Student.LastName,
                        assignment.Title,
                        course.Name,
                        assignment.DueDate
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send assignment notification to student {StudentEmail}",
                        enrollment.Student.Email);
                }
            }
        }
    }
}
