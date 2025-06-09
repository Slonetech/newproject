using System.ComponentModel.DataAnnotations;

namespace SchoolApi.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; } // Changed to CourseId from Id
        [Required]
        [StringLength(200)]
        public required string Title { get; set; } // Mark as required
        [StringLength(500)]
        public string? Description { get; set; } // Made nullable

        public int Credits { get; set; }

        // Navigation properties for relationships
        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
        public ICollection<TeacherCourse> TeacherCourses { get; set; } = new List<TeacherCourse>();
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
