using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace SchoolApi.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlContent);
        Task SendGradeNotificationAsync(string toEmail, string studentFirstName, string studentLastName, string courseTitle, double gradeValue, DateTime date);
        Task SendAttendanceNotificationAsync(string toEmail, string studentFirstName, string studentLastName, string courseTitle, bool isPresent, DateTime date);
        Task SendPasswordResetAsync(string to, string resetLink);
        Task SendAssignmentNotificationAsync(string toEmail, string studentFirstName, string studentLastName, string assignmentTitle, string courseName, DateTime dueDate);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly string _sendGridApiKey;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _fromEmail = _configuration["Email:FromEmail"] ?? throw new ArgumentNullException("Email:FromEmail configuration is missing");
            _fromName = _configuration["Email:FromName"] ?? throw new ArgumentNullException("Email:FromName configuration is missing");
            _sendGridApiKey = _configuration["Email:SendGridApiKey"] ?? throw new ArgumentNullException("Email:SendGridApiKey configuration is missing");
        }

        public async Task SendEmailAsync(string to, string subject, string htmlContent)
        {
            var client = new SendGridClient(_sendGridApiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(_fromEmail, _fromName),
                Subject = subject,
                HtmlContent = htmlContent
            };
            msg.AddTo(new EmailAddress(to));

            var response = await client.SendEmailAsync(msg);
            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                throw new Exception($"Failed to send email: {response.StatusCode}");
            }
        }

        public async Task SendGradeNotificationAsync(string toEmail, string studentFirstName, string studentLastName, string courseTitle, double gradeValue, DateTime date)
        {
            var client = new SendGridClient(_sendGridApiKey);
            var from = new EmailAddress(_fromEmail, _fromName);
            var to = new EmailAddress(toEmail);
            var subject = $"Grade Update for {studentFirstName} {studentLastName}";
            var plainTextContent = $"A new grade has been recorded for {studentFirstName} {studentLastName} in {courseTitle}. Grade: {gradeValue} Date: {date:d}";
            var htmlContent = $@"
                <h2>Grade Update</h2>
                <p>A new grade has been recorded for {studentFirstName} {studentLastName} in {courseTitle}.</p>
                <p><strong>Grade:</strong> {gradeValue}</p>
                <p><strong>Date:</strong> {date:d}</p>
            ";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            await client.SendEmailAsync(msg);
        }

        public async Task SendAttendanceNotificationAsync(string toEmail, string studentFirstName, string studentLastName, string courseTitle, bool isPresent, DateTime date)
        {
            var client = new SendGridClient(_sendGridApiKey);
            var from = new EmailAddress(_fromEmail, _fromName);
            var to = new EmailAddress(toEmail);
            var subject = $"Attendance Update for {studentFirstName} {studentLastName}";
            var status = isPresent ? "Present" : "Absent";
            var plainTextContent = $"Attendance has been recorded for {studentFirstName} {studentLastName} in {courseTitle}. Status: {status} Date: {date:d}";
            var htmlContent = $@"
                <h2>Attendance Update</h2>
                <p>Attendance has been recorded for {studentFirstName} {studentLastName} in {courseTitle}.</p>
                <p><strong>Status:</strong> {status}</p>
                <p><strong>Date:</strong> {date:d}</p>
            ";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            await client.SendEmailAsync(msg);
        }

        public async Task SendPasswordResetAsync(string to, string resetLink)
        {
            var subject = "Password Reset Request";
            var htmlContent = $@"
                <h2>Password Reset Request</h2>
                <p>You have requested to reset your password. Click the link below to proceed:</p>
                <p><a href='{resetLink}'>Reset Password</a></p>
                <p>If you did not request this reset, please ignore this email.</p>";

            await SendEmailAsync(to, subject, htmlContent);
        }

        public async Task SendAssignmentNotificationAsync(string toEmail, string studentFirstName, string studentLastName, string assignmentTitle, string courseName, DateTime dueDate)
        {
            var subject = $"New Assignment: {assignmentTitle}";
            var htmlContent = $@"
                <h2>New Assignment Posted</h2>
                <p>Hello {studentFirstName} {studentLastName},</p>
                <p>A new assignment has been posted for your course <strong>{courseName}</strong>.</p>
                <h3>Assignment Details:</h3>
                <ul>
                    <li><strong>Title:</strong> {assignmentTitle}</li>
                    <li><strong>Course:</strong> {courseName}</li>
                    <li><strong>Due Date:</strong> {dueDate:MMM dd, yyyy 'at' HH:mm}</li>
                </ul>
                <p>Please log into your student portal to view the full assignment details and submit your work.</p>
                <p>Good luck with your assignment!</p>";

            await SendEmailAsync(toEmail, subject, htmlContent);
        }
    }
}