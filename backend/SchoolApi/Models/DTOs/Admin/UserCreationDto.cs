using System.ComponentModel.DataAnnotations;

namespace SchoolApi.Models.DTOs.Admin
{
    public class UserCreationDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string? Username { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)] // Enforce strong password
        public string? Password { get; set; }

        [Required]
        [StringLength(100)]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string? LastName { get; set; }

        [Required]
        [StringLength(50)]
        public string? InitialRole { get; set; } // e.g., "Student", "Teacher", "Parent", "Admin"
    }
}
