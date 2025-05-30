using System.ComponentModel.DataAnnotations;

namespace SchoolApi.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Title { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Range(1, 10)]
        public int Credits { get; set; }

        // Foreign Key: Teacher
        public string? TeacherId { get; set; }
        public Teacher? Teacher { get; set; }

        // Many-to-many with Students
        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
    }
}
