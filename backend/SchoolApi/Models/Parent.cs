using System.ComponentModel.DataAnnotations;

namespace SchoolApi.Models
{
    public class Parent
    {
        [Key]
        public int ParentId { get; set; }
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
        public required string Address { get; set; } // Mark as required

        // Foreign key for ApplicationUser (can be null)
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; } // Mark as nullable

        // Navigation property for children
        public ICollection<Student> Children { get; set; } = new List<Student>();
    }
}
