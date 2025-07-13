using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolApi.Models
{
    public class Assignment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public Guid CourseId { get; set; }

        [Required]
        public Guid TeacherId { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public int MaxPoints { get; set; } = 100;

        [StringLength(50)]
        public string? AssignmentType { get; set; } // Homework, Quiz, Exam, Project, etc.

        // Navigation properties
        [ForeignKey("CourseId")]
        public virtual Course? Course { get; set; }

        [ForeignKey("TeacherId")]
        public virtual Teacher? Teacher { get; set; }

        public virtual ICollection<AssignmentSubmission> Submissions { get; set; } = new List<AssignmentSubmission>();
    }

    public class AssignmentSubmission
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid AssignmentId { get; set; }

        [Required]
        public Guid StudentId { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public DateTime? GradedAt { get; set; }

        public double? Grade { get; set; }

        [StringLength(1000)]
        public string? SubmissionText { get; set; }

        [StringLength(500)]
        public string? FileUrl { get; set; }

        [StringLength(500)]
        public string? TeacherFeedback { get; set; }

        public bool IsLate { get; set; }

        // Navigation properties
        [ForeignKey("AssignmentId")]
        public virtual Assignment? Assignment { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }
    }
}