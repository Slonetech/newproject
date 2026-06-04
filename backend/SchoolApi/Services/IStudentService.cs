using SchoolApi.Models;
using SchoolApi.Models.DTOs;

namespace SchoolApi.Services
{
    /// <summary>
    /// Service interface for student-related operations
    /// </summary>
    public interface IStudentService
    {
        Task<IEnumerable<Student>> GetAllStudentsAsync();
        Task<Student?> GetStudentByIdAsync(Guid id);
        Task<Student?> GetStudentByUserIdAsync(string userId);
        Task<Student> CreateStudentAsync(CreateUserDto dto, string userId);
        Task<Student> UpdateStudentAsync(Guid id, StudentDto dto);
        Task DeleteStudentAsync(Guid id);
        Task<IEnumerable<Course>> GetStudentCoursesAsync(Guid studentId);
        Task<IEnumerable<Grade>> GetStudentGradesAsync(Guid studentId);
        Task<IEnumerable<Attendance>> GetStudentAttendancesAsync(Guid studentId);
    }
}
