// Models/DTOs/AttendanceDto.cs
namespace SchoolApi.Models.DTOs
{
    public class AttendanceDto
    {
        public int Id { get; set; }
        public string Status { get; set; } = null!;
        public int StudentId { get; set; }
        public int TeacherId { get; set; }
    }

    public class CreateAttendanceDto
    {
        public string Status { get; set; } = null!;
        public int StudentId { get; set; }
        public int TeacherId { get; set; }
    }
}
