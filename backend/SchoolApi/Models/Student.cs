using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SchoolApi.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? FullName { get; set; }

        // ... other student properties ...

        // Foreign key to ApplicationUser
        public string? UserId { get; set; }

        // Navigation property
        public ApplicationUser? User { get; set; }

        // Navigation to Parent
        public string? ParentId { get; set; }
        public Parent? Parent { get; set; }

        // Many-to-many with courses
        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
    }
}
