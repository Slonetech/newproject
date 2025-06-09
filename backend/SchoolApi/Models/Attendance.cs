using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolApi.Models
{
    public enum AttendanceStatus
    {
        Present,
        Absent,
        Late
    }

    public class Attendance
    {
        [Key]
        public int AttendanceId { get; set; } // Changed to AttendanceId from Id
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public AttendanceStatus Status { get; set; }
        [StringLength(500)]
        public string? Remarks { get; set; } // Made nullable

        // Foreign keys and navigation properties
        public int StudentId { get; set; }
        public required Student Student { get; set; } // Mark as required

        public int CourseId { get; set; }
        public required Course Course { get; set; } // Mark as required

        public int TeacherId { get; set; }
        public required Teacher Teacher { get; set; } // Mark as required
    }
}
