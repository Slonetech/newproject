using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolApi.Models
{
    public class TeacherCourse
    {
        // Composite Key for TeacherCourse
        [Key]
        [Column(Order = 1)] // Specify order for composite key
        public Guid TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!; // Mark as required with null-forgiving operator

        [Key]
        [Column(Order = 2)] // Specify order for composite key
        public Guid CourseId { get; set; }
        public Course Course { get; set; } = null!; // Mark as required with null-forgiving operator

        public DateTime AssignmentDate { get; set; } = DateTime.UtcNow;
    }
}