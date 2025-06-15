using System.ComponentModel.DataAnnotations;

namespace SchoolApi.Models
{
    public class Event
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        [StringLength(100)]
        public string Location { get; set; } = string.Empty;

        public bool IsAllDay { get; set; }

        public string? RecurrenceRule { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Foreign key for the user who created the event
        public string CreatedById { get; set; } = string.Empty;

        // Navigation property
        public ApplicationUser? CreatedBy { get; set; }
    }
}