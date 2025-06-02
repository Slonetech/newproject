using System.ComponentModel.DataAnnotations;

namespace SchoolApi.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        // Navigation property for the many-to-many relationship
        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();

        // If you have grades as well:
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
    }


}
