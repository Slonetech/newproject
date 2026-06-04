using Microsoft.EntityFrameworkCore;
using SchoolApi.Data;
using SchoolApi.Models;
using SchoolApi.Models.DTOs;

namespace SchoolApi.Services
{
    /// <summary>
    /// Service for student-related business logic
    /// </summary>
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StudentService> _logger;

        public StudentService(ApplicationDbContext context, ILogger<StudentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            return await _context.Students
                .Include(s => s.Enrollments)
                .ToListAsync();
        }

        public async Task<Student?> GetStudentByIdAsync(Guid id)
        {
            return await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .Include(s => s.Grades)
                .Include(s => s.Attendances)
                .Include(s => s.ParentLinks)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Student?> GetStudentByUserIdAsync(string userId)
        {
            return await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .Include(s => s.Grades)
                    .ThenInclude(g => g.Course)
                .Include(s => s.Attendances)
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<Student> CreateStudentAsync(CreateUserDto dto, string userId)
        {
            var student = new Student
            {
                UserId = userId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Grade = 1, // Default grade
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                IsActive = true,
                EnrollmentDate = DateTime.UtcNow
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created student {StudentId} for user {UserId}", student.Id, userId);
            return student;
        }

        public async Task<Student> UpdateStudentAsync(Guid id, StudentDto dto)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                throw new KeyNotFoundException($"Student with ID {id} not found");
            }

            student.FirstName = dto.FirstName;
            student.LastName = dto.LastName;
            student.Email = dto.Email;
            student.PhoneNumber = dto.PhoneNumber;
            student.Address = dto.Address;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated student {StudentId}", id);
            return student;
        }

        public async Task DeleteStudentAsync(Guid id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                throw new KeyNotFoundException($"Student with ID {id} not found");
            }

            // Soft delete
            student.IsActive = false;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Soft deleted student {StudentId}", id);
        }

        public async Task<IEnumerable<Course>> GetStudentCoursesAsync(Guid studentId)
        {
            var student = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
            {
                throw new KeyNotFoundException($"Student with ID {studentId} not found");
            }

            return student.Enrollments.Select(e => e.Course).ToList();
        }

        public async Task<IEnumerable<Grade>> GetStudentGradesAsync(Guid studentId)
        {
            return await _context.Grades
                .Include(g => g.Course)
                .Where(g => g.StudentId == studentId)
                .OrderByDescending(g => g.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Attendance>> GetStudentAttendancesAsync(Guid studentId)
        {
            return await _context.Attendances
                .Include(a => a.Course)
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(a => a.Date)
                .ToListAsync();
        }
    }
}
