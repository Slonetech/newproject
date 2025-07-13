using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Data;
using SchoolApi.Models;
using SchoolApi.Models.DTOs;
using SchoolApi.Services;
using System.Security.Claims;

namespace SchoolApi.Controllers
{
    /// <summary>
    /// Manages assignments and submissions for courses
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AssignmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<AssignmentsController> _logger;

        public AssignmentsController(
            ApplicationDbContext context,
            IEmailService emailService,
            ILogger<AssignmentsController> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Get all assignments (Admin, Teacher access)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult<IEnumerable<AssignmentDto>>> GetAssignments()
        {
            try
            {
                var assignments = await _context.Assignments
                    .Include(a => a.Course)
                    .Include(a => a.Teacher)
                    .Include(a => a.Submissions)
                    .Where(a => a.IsActive)
                    .Select(a => new AssignmentDto
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Description = a.Description,
                        CourseId = a.CourseId,
                        TeacherId = a.TeacherId,
                        DueDate = a.DueDate,
                        CreatedAt = a.CreatedAt,
                        IsActive = a.IsActive,
                        MaxPoints = a.MaxPoints,
                        AssignmentType = a.AssignmentType,
                        CourseName = a.Course != null ? a.Course.Name : null,
                        TeacherName = a.Teacher != null ? $"{a.Teacher.FirstName} {a.Teacher.LastName}" : null,
                        SubmissionCount = a.Submissions.Count
                    })
                    .ToListAsync();

                return Ok(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving assignments");
                return StatusCode(500, new { message = "An error occurred while retrieving assignments" });
            }
        }

        /// <summary>
        /// Get assignments for a specific course
        /// </summary>
        [HttpGet("course/{courseId}")]
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<ActionResult<IEnumerable<AssignmentDto>>> GetAssignmentsByCourse(Guid courseId)
        {
            try
            {
                var assignments = await _context.Assignments
                    .Include(a => a.Course)
                    .Include(a => a.Teacher)
                    .Include(a => a.Submissions)
                    .Where(a => a.CourseId == courseId && a.IsActive)
                    .Select(a => new AssignmentDto
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Description = a.Description,
                        CourseId = a.CourseId,
                        TeacherId = a.TeacherId,
                        DueDate = a.DueDate,
                        CreatedAt = a.CreatedAt,
                        IsActive = a.IsActive,
                        MaxPoints = a.MaxPoints,
                        AssignmentType = a.AssignmentType,
                        CourseName = a.Course != null ? a.Course.Name : null,
                        TeacherName = a.Teacher != null ? $"{a.Teacher.FirstName} {a.Teacher.LastName}" : null,
                        SubmissionCount = a.Submissions.Count
                    })
                    .ToListAsync();

                return Ok(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving assignments for course {CourseId}", courseId);
                return StatusCode(500, new { message = "An error occurred while retrieving assignments" });
            }
        }

        /// <summary>
        /// Get a specific assignment by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<ActionResult<AssignmentDto>> GetAssignment(Guid id)
        {
            try
            {
                var assignment = await _context.Assignments
                    .Include(a => a.Course)
                    .Include(a => a.Teacher)
                    .Include(a => a.Submissions)
                    .FirstOrDefaultAsync(a => a.Id == id && a.IsActive);

                if (assignment == null)
                {
                    return NotFound(new { message = "Assignment not found" });
                }

                var assignmentDto = new AssignmentDto
                {
                    Id = assignment.Id,
                    Title = assignment.Title,
                    Description = assignment.Description,
                    CourseId = assignment.CourseId,
                    TeacherId = assignment.TeacherId,
                    DueDate = assignment.DueDate,
                    CreatedAt = assignment.CreatedAt,
                    IsActive = assignment.IsActive,
                    MaxPoints = assignment.MaxPoints,
                    AssignmentType = assignment.AssignmentType,
                    CourseName = assignment.Course != null ? assignment.Course.Name : null,
                    TeacherName = assignment.Teacher != null ? $"{assignment.Teacher.FirstName} {assignment.Teacher.LastName}" : null,
                    SubmissionCount = assignment.Submissions.Count
                };

                return Ok(assignmentDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving assignment {AssignmentId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the assignment" });
            }
        }

        /// <summary>
        /// Create a new assignment (Admin, Teacher access)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult<AssignmentDto>> CreateAssignment(CreateAssignmentDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verify the course exists
                var course = await _context.Courses.FindAsync(createDto.CourseId);
                if (course == null)
                {
                    return BadRequest(new { message = "Course not found" });
                }

                // Get the current user's teacher ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);

                if (teacher == null && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                var assignment = new Assignment
                {
                    Id = Guid.NewGuid(),
                    Title = createDto.Title,
                    Description = createDto.Description,
                    CourseId = createDto.CourseId,
                    TeacherId = teacher?.Id ?? createDto.CourseId, // For admin, use course ID as fallback
                    DueDate = createDto.DueDate,
                    MaxPoints = createDto.MaxPoints,
                    AssignmentType = createDto.AssignmentType,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Assignments.Add(assignment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Assignment created: {AssignmentId} by {UserId}", assignment.Id, userId);

                // Send notifications to enrolled students
                var enrolledStudents = await _context.Enrollments
                    .Where(e => e.CourseId == createDto.CourseId)
                    .Include(e => e.Student)
                    .Select(e => e.Student)
                    .ToListAsync();

                foreach (var student in enrolledStudents)
                {
                    try
                    {
                        await _emailService.SendAssignmentNotificationAsync(
                            student.Email,
                            student.FirstName,
                            student.LastName,
                            assignment.Title,
                            course.Name,
                            assignment.DueDate
                        );
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to send assignment notification to {Email}", student.Email);
                    }
                }

                return CreatedAtAction(nameof(GetAssignment), new { id = assignment.Id }, assignment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating assignment");
                return StatusCode(500, new { message = "An error occurred while creating the assignment" });
            }
        }

        /// <summary>
        /// Update an assignment (Admin, Teacher who created it)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> UpdateAssignment(Guid id, UpdateAssignmentDto updateDto)
        {
            try
            {
                var assignment = await _context.Assignments.FindAsync(id);
                if (assignment == null)
                {
                    return NotFound(new { message = "Assignment not found" });
                }

                // Check if teacher is updating their own assignment or if user is admin
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);

                if (!User.IsInRole("Admin") && (teacher == null || assignment.TeacherId != teacher.Id))
                {
                    return Forbid();
                }

                if (updateDto.Title != null)
                    assignment.Title = updateDto.Title;
                if (updateDto.Description != null)
                    assignment.Description = updateDto.Description;
                if (updateDto.DueDate.HasValue)
                    assignment.DueDate = updateDto.DueDate.Value;
                if (updateDto.MaxPoints.HasValue)
                    assignment.MaxPoints = updateDto.MaxPoints.Value;
                if (updateDto.AssignmentType != null)
                    assignment.AssignmentType = updateDto.AssignmentType;
                if (updateDto.IsActive.HasValue)
                    assignment.IsActive = updateDto.IsActive.Value;

                assignment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Assignment updated: {AssignmentId} by {UserId}", id, userId);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating assignment {AssignmentId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the assignment" });
            }
        }

        /// <summary>
        /// Delete an assignment (Admin, Teacher who created it)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> DeleteAssignment(Guid id)
        {
            try
            {
                var assignment = await _context.Assignments.FindAsync(id);
                if (assignment == null)
                {
                    return NotFound(new { message = "Assignment not found" });
                }

                // Check if teacher is deleting their own assignment or if user is admin
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);

                if (!User.IsInRole("Admin") && (teacher == null || assignment.TeacherId != teacher.Id))
                {
                    return Forbid();
                }

                assignment.IsActive = false;
                assignment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Assignment deleted: {AssignmentId} by {UserId}", id, userId);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting assignment {AssignmentId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the assignment" });
            }
        }

        /// <summary>
        /// Submit an assignment (Student access)
        /// </summary>
        [HttpPost("{assignmentId}/submit")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<AssignmentSubmissionDto>> SubmitAssignment(Guid assignmentId, CreateSubmissionDto submissionDto)
        {
            try
            {
                var assignment = await _context.Assignments.FindAsync(assignmentId);
                if (assignment == null || !assignment.IsActive)
                {
                    return NotFound(new { message = "Assignment not found" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
                if (student == null)
                {
                    return NotFound(new { message = "Student profile not found" });
                }

                // Check if student is enrolled in the course
                var enrollment = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.StudentId == student.Id && e.CourseId == assignment.CourseId);
                if (enrollment == null)
                {
                    return BadRequest(new { message = "You are not enrolled in this course" });
                }

                // Check if submission already exists
                var existingSubmission = await _context.AssignmentSubmissions
                    .FirstOrDefaultAsync(s => s.AssignmentId == assignmentId && s.StudentId == student.Id);
                if (existingSubmission != null)
                {
                    return BadRequest(new { message = "You have already submitted this assignment" });
                }

                var submission = new AssignmentSubmission
                {
                    Id = Guid.NewGuid(),
                    AssignmentId = assignmentId,
                    StudentId = student.Id,
                    SubmissionText = submissionDto.SubmissionText,
                    FileUrl = submissionDto.FileUrl,
                    SubmittedAt = DateTime.UtcNow,
                    IsLate = DateTime.UtcNow > assignment.DueDate
                };

                _context.AssignmentSubmissions.Add(submission);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Assignment submitted: {AssignmentId} by student {StudentId}", assignmentId, student.Id);

                return CreatedAtAction(nameof(GetSubmission), new { assignmentId, submissionId = submission.Id }, submission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting assignment {AssignmentId}", assignmentId);
                return StatusCode(500, new { message = "An error occurred while submitting the assignment" });
            }
        }

        /// <summary>
        /// Get submission for a specific assignment and student
        /// </summary>
        [HttpGet("{assignmentId}/submission")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<AssignmentSubmissionDto>> GetSubmission(Guid assignmentId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
                if (student == null)
                {
                    return NotFound(new { message = "Student profile not found" });
                }

                var submission = await _context.AssignmentSubmissions
                    .Include(s => s.Assignment)
                    .Include(s => s.Student)
                    .FirstOrDefaultAsync(s => s.AssignmentId == assignmentId && s.StudentId == student.Id);

                if (submission == null)
                {
                    return NotFound(new { message = "Submission not found" });
                }

                var submissionDto = new AssignmentSubmissionDto
                {
                    Id = submission.Id,
                    AssignmentId = submission.AssignmentId,
                    StudentId = submission.StudentId,
                    SubmittedAt = submission.SubmittedAt,
                    GradedAt = submission.GradedAt,
                    Grade = submission.Grade,
                    SubmissionText = submission.SubmissionText,
                    FileUrl = submission.FileUrl,
                    TeacherFeedback = submission.TeacherFeedback,
                    IsLate = submission.IsLate,
                    StudentName = submission.Student != null ? $"{submission.Student.FirstName} {submission.Student.LastName}" : null,
                    AssignmentTitle = submission.Assignment != null ? submission.Assignment.Title : null,
                };

                return Ok(submissionDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving submission for assignment {AssignmentId}", assignmentId);
                return StatusCode(500, new { message = "An error occurred while retrieving the submission" });
            }
        }

        /// <summary>
        /// Grade a submission (Admin, Teacher access)
        /// </summary>
        [HttpPost("{assignmentId}/submissions/{submissionId}/grade")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GradeSubmission(Guid assignmentId, Guid submissionId, GradeSubmissionDto gradeDto)
        {
            try
            {
                var submission = await _context.AssignmentSubmissions
                    .Include(s => s.Assignment)
                    .Include(s => s.Student)
                    .FirstOrDefaultAsync(s => s.Id == submissionId && s.AssignmentId == assignmentId);

                if (submission == null)
                {
                    return NotFound(new { message = "Submission not found" });
                }

                // Check if teacher is grading their own assignment or if user is admin
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);

                if (!User.IsInRole("Admin") && (teacher == null || submission.Assignment.TeacherId != teacher.Id))
                {
                    return Forbid();
                }

                submission.Grade = gradeDto.Grade;
                submission.TeacherFeedback = gradeDto.TeacherFeedback;
                submission.GradedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Submission graded: {SubmissionId} by {UserId}", submissionId, userId);

                // Send notification to student
                try
                {
                    await _emailService.SendGradeNotificationAsync(
                        submission.Student.Email,
                        submission.Student.FirstName,
                        submission.Student.LastName,
                        submission.Assignment.Title,
                        gradeDto.Grade,
                        DateTime.UtcNow
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send grade notification to {Email}", submission.Student.Email);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error grading submission {SubmissionId}", submissionId);
                return StatusCode(500, new { message = "An error occurred while grading the submission" });
            }
        }

        /// <summary>
        /// Get all submissions for an assignment (Admin, Teacher access)
        /// </summary>
        [HttpGet("{assignmentId}/submissions")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult<IEnumerable<AssignmentSubmissionDto>>> GetAssignmentSubmissions(Guid assignmentId)
        {
            try
            {
                var assignment = await _context.Assignments.FindAsync(assignmentId);
                if (assignment == null)
                {
                    return NotFound(new { message = "Assignment not found" });
                }

                // Check if teacher is accessing their own assignment or if user is admin
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);

                if (!User.IsInRole("Admin") && (teacher == null || assignment.TeacherId != teacher.Id))
                {
                    return Forbid();
                }

                var submissions = await _context.AssignmentSubmissions
                    .Include(s => s.Student)
                    .Include(s => s.Assignment)
                    .Where(s => s.AssignmentId == assignmentId)
                    .Select(s => new AssignmentSubmissionDto
                    {
                        Id = s.Id,
                        AssignmentId = s.AssignmentId,
                        StudentId = s.StudentId,
                        SubmittedAt = s.SubmittedAt,
                        GradedAt = s.GradedAt,
                        Grade = s.Grade,
                        SubmissionText = s.SubmissionText,
                        FileUrl = s.FileUrl,
                        TeacherFeedback = s.TeacherFeedback,
                        IsLate = s.IsLate,
                        StudentName = s.Student != null ? $"{s.Student.FirstName} {s.Student.LastName}" : null,
                        AssignmentTitle = s.Assignment != null ? s.Assignment.Title : null,
                    })
                    .ToListAsync();

                return Ok(submissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving submissions for assignment {AssignmentId}", assignmentId);
                return StatusCode(500, new { message = "An error occurred while retrieving submissions" });
            }
        }
    }
}