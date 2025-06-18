using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolApi.Models
{
    public class Course
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string Code { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public int Credits { get; set; }

        // Removed TeacherId and Teacher navigation property as they are now in TeacherCourse

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<StudentCourse> StudentCourses { get; set; }
        public virtual ICollection<Grade> Grades { get; set; }
        public virtual ICollection<Attendance> Attendances { get; set; }
        public virtual ICollection<TeacherCourse> TeacherCourses { get; set; } // New navigation property for TeacherCourses

        public Course()
        {
            StudentCourses = new HashSet<StudentCourse>();
            Grades = new HashSet<Grade>();
            Attendances = new HashSet<Attendance>();
            TeacherCourses = new HashSet<TeacherCourse>(); // Initialize the new collection
        }
    }
}