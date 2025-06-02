using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolApi.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        public string? AdmissionNumber { get; set; }
        public string? GradeLevel { get; set; }

        public int? ParentId { get; set; }
        public Parent? Parent { get; set; }

        // Relationships
        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();

        // Link to ApplicationUser
        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}
