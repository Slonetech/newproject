using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SchoolApi.Models
{
    public class Student
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Range(1, 12)]
        public int Grade { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; }

        // Navigation properties
        public ApplicationUser? User { get; set; }
        public ICollection<ParentChild> ParentLinks { get; set; } = new List<ParentChild>();
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        [NotMapped]
        public ICollection<Parent> Parents => ParentLinks.Select(pl => pl.Parent).ToList();
        // For compatibility with legacy code, alias StudentCourses to Enrollments
        [NotMapped]
        public ICollection<Enrollment> StudentCourses => Enrollments;
    }
}
