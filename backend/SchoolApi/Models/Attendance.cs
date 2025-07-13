using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolApi.Models
{
    public class Attendance
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid StudentId { get; set; }
        public Student Student { get; set; } = null!;

        [Required]
        public Guid CourseId { get; set; }
        public Course Course { get; set; } = null!;

        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required]
        public bool IsPresent { get; set; }

        [StringLength(500)]
        public string? Comments { get; set; }

        // Navigation properties
        // [ForeignKey("StudentId")] // This line is removed as per the edit hint
        // public Student? Student { get; set; }

        // [ForeignKey("CourseId")] // This line is removed as per the edit hint
        // public Course? Course { get; set; }
    }
}
