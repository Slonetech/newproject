using System.ComponentModel.DataAnnotations;

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

        [Required]
        public AttendanceStatus Status { get; set; }

        [Required]
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        [Required]
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;
    }

}
