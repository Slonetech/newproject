using System.ComponentModel.DataAnnotations;

namespace SchoolApi.Models
{
    public class Announcement
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Foreign key for the user who created the announcement
        public string CreatedById { get; set; } = string.Empty;

        // Navigation property
        public ApplicationUser? CreatedBy { get; set; }
    }
}