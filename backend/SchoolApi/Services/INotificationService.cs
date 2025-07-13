namespace SchoolApi.Services
{
    public interface INotificationService
    {
        Task SendNotificationToUserAsync(string userId, string message, string type);
        Task SendNotificationToRoleAsync(string role, string message, string type);
        Task SendNotificationToGroupAsync(string groupName, string message, string type);
        Task SendGradeNotificationAsync(string studentId, string courseName, double grade);
        Task SendAssignmentNotificationAsync(string studentId, string assignmentTitle, string courseName);
        Task SendAttendanceNotificationAsync(string studentId, string courseName, bool isPresent);
    }
}