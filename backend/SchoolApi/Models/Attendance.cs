namespace SchoolApi.Models
{
    public enum AttendanceStatus
    {
        Absent = 0,
        Present = 1,
        Excused = 2
    }

    public class Attendance
    {
        public int Id { get; set; }
        public AttendanceStatus Status { get; set; }

        // Relationships
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;
    }
}
