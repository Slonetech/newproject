using System.ComponentModel.DataAnnotations;

namespace SchoolApi.Models.DTOs.Admin
{
    public class UserUpdateDto
    {
        [StringLength(50, MinimumLength = 3)]
        public string? Username { get; set; } // Nullable if not always updated

        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }

        // Note: Password update should ideally be a separate endpoint for security
    }
}
