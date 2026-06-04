using Microsoft.EntityFrameworkCore;
using SchoolApi.Data;
using SchoolApi.Models;
using SchoolApi.Models.DTOs;
using SchoolApi.Models.Enums;

namespace SchoolApi.Services
{
    /// <summary>
    /// Service for attendance-related business logic
    /// </summary>
    public class AttendanceService : IAttendanceService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<AttendanceService> _logger;

        public AttendanceService(
            ApplicationDbContext context,
            IEmailService emailService,
            ILogger<AttendanceService> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<IEnumerable<Attendance>> GetAllAttendancesAsync()
        {
            return await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Course)
                .OrderByDescending(a => a.Date)
                .ToListAsync();
        }

        public async Task<Attendance?> GetAttendanceByIdAsync(Guid id)
        {
            return await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Course)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Attendance>> GetStudentAttendancesAsync(Guid studentId)
        {
            return await _context.Attendances
                .Include(a => a.Course)
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(a => a.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Attendance>> GetCourseAttendancesAsync(Guid courseId)
        {
            return await _context.Attendances
                .Include(a => a.Student)
                .Where(a => a.CourseId == courseId)
                .OrderByDescending(a => a.Date)
                .ToListAsync();
        }

        public async Task<Attendance> CreateAttendanceAsync(CreateAttendanceDto dto, string createdBy)
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

            var attendance = new Attendance
            {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                Date = dto.Date,
                Status = dto.Status,
                Comments = dto.Comments
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            // Send email notifications to parents
            await NotifyParentsAsync(student, course, attendance);

            _logger.LogInformation("Created attendance record {AttendanceId} for student {StudentId} by {CreatedBy}",
                attendance.Id, student.Id, createdBy);

            return attendance;
        }

        public async Task<Attendance> UpdateAttendanceAsync(Guid id, UpdateAttendanceDto dto)
        {
            var attendance = await _context.Attendances
                .Include(a => a.Student)
                    .ThenInclude(s => s.ParentLinks)
                .Include(a => a.Course)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attendance == null)
            {
                throw new KeyNotFoundException($"Attendance with ID {id} not found");
            }

            attendance.Status = dto.Status;
            attendance.Comments = dto.Comments;

            await _context.SaveChangesAsync();

            // Send email notifications to parents
            await NotifyParentsAsync(attendance.Student, attendance.Course, attendance);

            _logger.LogInformation("Updated attendance record {AttendanceId}", id);

            return attendance;
        }

        public async Task DeleteAttendanceAsync(Guid id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null)
            {
                throw new KeyNotFoundException($"Attendance with ID {id} not found");
            }

            _context.Attendances.Remove(attendance);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted attendance record {AttendanceId}", id);
        }

        public async Task<Dictionary<AttendanceStatus, int>> GetAttendanceSummaryAsync(Guid studentId, Guid? courseId = null)
        {
            var query = _context.Attendances
                .Where(a => a.StudentId == studentId);

            if (courseId.HasValue)
            {
                query = query.Where(a => a.CourseId == courseId.Value);
            }

            var attendances = await query.ToListAsync();

            return attendances
                .GroupBy(a => a.Status)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        private async Task NotifyParentsAsync(Student student, Course course, Attendance attendance)
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
                        await _emailService.SendAttendanceNotificationAsync(
                            link.Parent.Email,
                            student.FirstName,
                            student.LastName,
                            course.Name,
                            attendance.Status,
                            attendance.Date
                        );
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send attendance notification to parent {ParentEmail}",
                            link.Parent.Email);
                    }
                }
            }
        }
    }
}
