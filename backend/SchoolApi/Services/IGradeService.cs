using SchoolApi.Models;
using SchoolApi.Models.DTOs;

namespace SchoolApi.Services
{
    /// <summary>
    /// Service interface for grade-related operations
    /// </summary>
    public interface IGradeService
    {
        Task<IEnumerable<Grade>> GetAllGradesAsync();
        Task<Grade?> GetGradeByIdAsync(Guid id);
        Task<IEnumerable<Grade>> GetStudentGradesAsync(Guid studentId);
        Task<IEnumerable<Grade>> GetCourseGradesAsync(Guid courseId);
        Task<Grade> CreateGradeAsync(GradeCreateDto dto, string createdBy);
        Task<Grade> UpdateGradeAsync(Guid id, GradeCreateDto dto);
        Task DeleteGradeAsync(Guid id);
        Task<double> GetStudentCourseAverageAsync(Guid studentId, Guid courseId);
        Task<double> GetStudentOverallAverageAsync(Guid studentId);
    }
}
