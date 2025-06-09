using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolApi.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }
        [Required]
        [StringLength(100)]
        public required string FirstName { get; set; } // Mark as required
        [Required]
        [StringLength(100)]
        public required string LastName { get; set; } // Mark as required
        [Required]
        [StringLength(255)]
        [EmailAddress]
        public required string Email { get; set; } // Mark as required
        public DateTime DateOfBirth { get; set; }
        public required string Address { get; set; } // Mark as required
        public required string PhoneNumber { get; set; } // Mark as required

        // Foreign key for ApplicationUser (can be null if not linked to an Identity user)
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; } // Mark as nullable

        // Navigation properties for relationships
        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

        // Relationship with Parent (can be null if student doesn't have a parent linked yet)
        public int? ParentId { get; set; }
        public Parent? Parent { get; set; } // Mark as nullable
    }
}
