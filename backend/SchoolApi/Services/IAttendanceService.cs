using SchoolApi.Models;
using SchoolApi.Models.DTOs;
using SchoolApi.Models.Enums;

namespace SchoolApi.Services
{
    /// <summary>
    /// Service interface for attendance-related operations
    /// </summary>
    public interface IAttendanceService
    {
        Task<IEnumerable<Attendance>> GetAllAttendancesAsync();
        Task<Attendance?> GetAttendanceByIdAsync(Guid id);
        Task<IEnumerable<Attendance>> GetStudentAttendancesAsync(Guid studentId);
        Task<IEnumerable<Attendance>> GetCourseAttendancesAsync(Guid courseId);
        Task<Attendance> CreateAttendanceAsync(CreateAttendanceDto dto, string createdBy);
        Task<Attendance> UpdateAttendanceAsync(Guid id, UpdateAttendanceDto dto);
        Task DeleteAttendanceAsync(Guid id);
        Task<Dictionary<AttendanceStatus, int>> GetAttendanceSummaryAsync(Guid studentId, Guid? courseId = null);
    }
}
