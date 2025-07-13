using System.ComponentModel.DataAnnotations;

namespace SchoolApi.Models.DTOs
{
    public class AssignmentDto
    {
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

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; }

        public int MaxPoints { get; set; }

        [StringLength(50)]
        public string? AssignmentType { get; set; }

        // Navigation properties
        public string? CourseName { get; set; }
        public string? TeacherName { get; set; }
        public int SubmissionCount { get; set; }
    }

    public class CreateAssignmentDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public Guid CourseId { get; set; }

        public DateTime DueDate { get; set; }

        public int MaxPoints { get; set; } = 100;

        [StringLength(50)]
        public string? AssignmentType { get; set; }
    }

    public class UpdateAssignmentDto
    {
        [StringLength(200)]
        public string? Title { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        public int? MaxPoints { get; set; }

        [StringLength(50)]
        public string? AssignmentType { get; set; }

        public bool? IsActive { get; set; }
    }

    public class AssignmentSubmissionDto
    {
        public Guid Id { get; set; }

        public Guid AssignmentId { get; set; }

        public Guid StudentId { get; set; }

        public DateTime SubmittedAt { get; set; }

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
        public string? StudentName { get; set; }
        public string? AssignmentTitle { get; set; }
    }

    public class CreateSubmissionDto
    {
        [Required]
        public Guid AssignmentId { get; set; }

        [StringLength(1000)]
        public string? SubmissionText { get; set; }

        [StringLength(500)]
        public string? FileUrl { get; set; }
    }

    public class GradeSubmissionDto
    {
        [Required]
        [Range(0, 100)]
        public double Grade { get; set; }

        [StringLength(500)]
        public string? TeacherFeedback { get; set; }
    }
}