using Microsoft.EntityFrameworkCore;
using SchoolApi.Data;
using SchoolApi.Models;
using SchoolApi.Models.DTOs;

namespace SchoolApi.Services
{
    /// <summary>
    /// Service for grade-related business logic
    /// </summary>
    public class GradeService : IGradeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<GradeService> _logger;

        public GradeService(
            ApplicationDbContext context,
            IEmailService emailService,
            ILogger<GradeService> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<IEnumerable<Grade>> GetAllGradesAsync()
        {
            return await _context.Grades
                .Include(g => g.Student)
                .Include(g => g.Course)
                .OrderByDescending(g => g.Date)
                .ToListAsync();
        }

        public async Task<Grade?> GetGradeByIdAsync(Guid id)
        {
            return await _context.Grades
                .Include(g => g.Student)
                .Include(g => g.Course)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<IEnumerable<Grade>> GetStudentGradesAsync(Guid studentId)
        {
            return await _context.Grades
                .Include(g => g.Course)
                .Where(g => g.StudentId == studentId)
                .OrderByDescending(g => g.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Grade>> GetCourseGradesAsync(Guid courseId)
        {
            return await _context.Grades
                .Include(g => g.Student)
                .Where(g => g.CourseId == courseId)
                .OrderByDescending(g => g.Date)
                .ToListAsync();
        }

        public async Task<Grade> CreateGradeAsync(GradeCreateDto dto, string createdBy)
        {
            // Validate student exists
            var student = await _context.Students
                .Include(s => s.ParentLinks)
                .FirstOrDefaultAsync(s => s.Id == dto.StudentId);

            if (student == null)
            {
                throw new KeyNotFoundException($"Student with ID {dto.StudentId} not found");
            }

            // Validate course exists
            var course = await _context.Courses.FindAsync(dto.CourseId);
            if (course == null)
            {
                throw new KeyNotFoundException($"Course with ID {dto.CourseId} not found");
            }

            // Validate grade value
            if (dto.Value < 0 || dto.Value > 100)
            {
                throw new ArgumentException("Grade value must be between 0 and 100");
            }

            var grade = new Grade
            {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                Value = dto.Value,
                Comments = dto.Comments,
                Date = DateTime.UtcNow
            };

            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();

            // Send email notifications to parents
            await NotifyParentsAsync(student, course, grade);

            _logger.LogInformation("Created grade {GradeId} for student {StudentId} by {CreatedBy}",
                grade.Id, student.Id, createdBy);

            return grade;
        }

        public async Task<Grade> UpdateGradeAsync(Guid id, GradeCreateDto dto)
        {
            var grade = await _context.Grades
                .Include(g => g.Student)
                    .ThenInclude(s => s.ParentLinks)
                .Include(g => g.Course)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (grade == null)
            {
                throw new KeyNotFoundException($"Grade with ID {id} not found");
            }

            // Validate grade value
            if (dto.Value < 0 || dto.Value > 100)
            {
                throw new ArgumentException("Grade value must be between 0 and 100");
            }

            grade.Value = dto.Value;
            grade.Comments = dto.Comments;

            await _context.SaveChangesAsync();

            // Send email notifications to parents
            await NotifyParentsAsync(grade.Student, grade.Course, grade);

            _logger.LogInformation("Updated grade {GradeId}", id);

            return grade;
        }

        public async Task DeleteGradeAsync(Guid id)
        {
            var grade = await _context.Grades.FindAsync(id);
            if (grade == null)
            {
                throw new KeyNotFoundException($"Grade with ID {id} not found");
            }

            _context.Grades.Remove(grade);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted grade {GradeId}", id);
        }

        public async Task<double> GetStudentCourseAverageAsync(Guid studentId, Guid courseId)
        {
            var grades = await _context.Grades
                .Where(g => g.StudentId == studentId && g.CourseId == courseId)
                .ToListAsync();

            if (!grades.Any())
            {
                return 0;
            }

            return grades.Average(g => g.Value);
        }

        public async Task<double> GetStudentOverallAverageAsync(Guid studentId)
        {
            var grades = await _context.Grades
                .Where(g => g.StudentId == studentId)
                .ToListAsync();

            if (!grades.Any())
            {
                return 0;
            }

            return grades.Average(g => g.Value);
        }

        private async Task NotifyParentsAsync(Student student, Course course, Grade grade)
        {
            var parentLinks = await _context.ParentChildren
                .Where(pc => pc.StudentId == student.Id)
                .Include(pc => pc.Parent)
                .ToListAsync();

            foreach (var link in parentLinks)
            {
                if (link.Parent != null)
                {
                    try
                    {
                        await _emailService.SendGradeNotificationAsync(
                            link.Parent.Email,
                            student.FirstName,
                            student.LastName,
                            course.Name,
                            grade.Value,
                            grade.Date
                        );
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send grade notification to parent {ParentEmail}",
                            link.Parent.Email);
                    }
                }
            }
        }
    }
}
