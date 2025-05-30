using System.ComponentModel.DataAnnotations;

namespace SchoolApi.Models
{
    public class Attendance
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(10)]
        public string? Status { get; set; } // Present, Absent, Late

        // Foreign Keys
        public int CourseId { get; set; }
        public Course? Course { get; set; }

        public string? StudentId { get; set; }
        public Student? Student { get; set; }
    }
}
