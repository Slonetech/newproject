using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SchoolApi.Models
{
    public class Teacher
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(200)]
        public string? Specialization { get; set; }

        [StringLength(100)]
        public string? Department { get; set; }

        public DateTime DateOfBirth { get; set; }

        public DateTime HireDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
        public string? Address { get; set; } // Or string Address { get; set; } = string.Empty;

        // Navigation property for TeacherCourses (the many-to-many intermediary)
        public virtual ICollection<TeacherCourse> TeacherCourses { get; set; }
        public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

        public Teacher()
        {
            TeacherCourses = new HashSet<TeacherCourse>();
        }
    }
}