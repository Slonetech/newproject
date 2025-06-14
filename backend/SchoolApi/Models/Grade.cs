using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolApi.Models
{
    public class Grade
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid StudentId { get; set; }

        [Required]
        public Guid CourseId { get; set; }

        [Required]
        [Range(0, 100)]
        public double Value { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string? Comments { get; set; }

        // Navigation properties
        [ForeignKey("StudentId")]
        public Student? Student { get; set; }

        [ForeignKey("CourseId")]
        public Course? Course { get; set; }
    }
}
