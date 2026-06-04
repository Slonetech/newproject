using SchoolApi.Models.Enums;

namespace SchoolApi.Models.DTOs
{
    public class AttendanceDto
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
        public DateTime Date { get; set; }
        public AttendanceStatus Status { get; set; }
        public string? Comments { get; set; }
    }

    public class CreateAttendanceDto
    {
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
        public DateTime Date { get; set; }
        public AttendanceStatus Status { get; set; }
        public string? Comments { get; set; }
    }

    public class UpdateAttendanceDto
    {
        public AttendanceStatus Status { get; set; }
        public string? Comments { get; set; }
    }
}
