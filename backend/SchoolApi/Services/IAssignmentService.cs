using SchoolApi.Models;
using SchoolApi.Models.DTOs;

namespace SchoolApi.Services
{
    /// <summary>
    /// Service interface for assignment-related operations
    /// </summary>
    public interface IAssignmentService
    {
        Task<IEnumerable<Assignment>> GetAllAssignmentsAsync();
        Task<Assignment?> GetAssignmentByIdAsync(Guid id);
        Task<IEnumerable<Assignment>> GetCourseAssignmentsAsync(Guid courseId);
        Task<IEnumerable<Assignment>> GetTeacherAssignmentsAsync(Guid teacherId);
        Task<Assignment> CreateAssignmentAsync(AssignmentDto dto, Guid teacherId);
        Task<Assignment> UpdateAssignmentAsync(Guid id, AssignmentDto dto);
        Task DeleteAssignmentAsync(Guid id);
        Task<AssignmentSubmission> SubmitAssignmentAsync(Guid assignmentId, Guid studentId, string submissionText, string? fileUrl);
        Task<AssignmentSubmission> GradeSubmissionAsync(Guid submissionId, double grade, string? feedback);
        Task<IEnumerable<AssignmentSubmission>> GetAssignmentSubmissionsAsync(Guid assignmentId);
        Task<AssignmentSubmission?> GetStudentSubmissionAsync(Guid assignmentId, Guid studentId);
    }
}
