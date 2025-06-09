using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolApi.Models
{
    public class Teacher
    {
        [Key]
        public int TeacherId { get; set; }
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
        public required string PhoneNumber { get; set; } // Mark as required
        public required string Department { get; set; } // Mark as required

        // Foreign key for ApplicationUser (can be null if not linked to an Identity user)
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; } // Mark as nullable

        // Navigation property for relationships
        public ICollection<TeacherCourse> TeacherCourses { get; set; } = new List<TeacherCourse>();
        public ICollection<Grade> AssignedGrades { get; set; } = new List<Grade>();
        public ICollection<Attendance> MarkedAttendances { get; set; } = new List<Attendance>();
    }
}
