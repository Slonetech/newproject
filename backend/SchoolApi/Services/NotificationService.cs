using Microsoft.AspNetCore.SignalR;
using SchoolApi.Hubs;
using SchoolApi.Data;
using Microsoft.EntityFrameworkCore;

namespace SchoolApi.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IHubContext<NotificationHub> hubContext,
            ApplicationDbContext context,
            ILogger<NotificationService> logger)
        {
            _hubContext = hubContext;
            _context = context;
            _logger = logger;
        }

        public async Task SendNotificationToUserAsync(string userId, string message, string type)
        {
            try
            {
                await _hubContext.Clients.Group($"user_{userId}").SendAsync("ReceiveNotification", new
                {
                    message,
                    type,
                    timestamp = DateTime.UtcNow
                });

                _logger.LogInformation("Notification sent to user {UserId}: {Message}", userId, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to user {UserId}", userId);
            }
        }

        public async Task SendNotificationToRoleAsync(string role, string message, string type)
        {
            try
            {
                await _hubContext.Clients.Group(role).SendAsync("ReceiveNotification", new
                {
                    message,
                    type,
                    timestamp = DateTime.UtcNow
                });

                _logger.LogInformation("Notification sent to role {Role}: {Message}", role, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to role {Role}", role);
            }
        }

        public async Task SendNotificationToGroupAsync(string groupName, string message, string type)
        {
            try
            {
                await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNotification", new
                {
                    message,
                    type,
                    timestamp = DateTime.UtcNow
                });

                _logger.LogInformation("Notification sent to group {GroupName}: {Message}", groupName, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to group {GroupName}", groupName);
            }
        }

        public async Task SendGradeNotificationAsync(string studentId, string courseName, double grade)
        {
            try
            {
                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.Id.ToString() == studentId);

                if (student != null)
                {
                    var message = $"New grade recorded for {courseName}: {grade}";
                    await SendNotificationToUserAsync(student.UserId, message, "grade");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending grade notification for student {StudentId}", studentId);
            }
        }

        public async Task SendAssignmentNotificationAsync(string studentId, string assignmentTitle, string courseName)
        {
            try
            {
                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.Id.ToString() == studentId);

                if (student != null)
                {
                    var message = $"New assignment posted for {courseName}: {assignmentTitle}";
                    await SendNotificationToUserAsync(student.UserId, message, "assignment");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending assignment notification for student {StudentId}", studentId);
            }
        }

        public async Task SendAttendanceNotificationAsync(string studentId, string courseName, bool isPresent)
        {
            try
            {
                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.Id.ToString() == studentId);

                if (student != null)
                {
                    var status = isPresent ? "present" : "absent";
                    var message = $"Attendance recorded for {courseName}: {status}";
                    await SendNotificationToUserAsync(student.UserId, message, "attendance");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending attendance notification for student {StudentId}", studentId);
            }
        }
    }
}