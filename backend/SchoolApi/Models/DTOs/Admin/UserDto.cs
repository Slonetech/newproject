using System.Collections.Generic; // Ensure this is present

namespace SchoolApi.Models.DTOs.Admin
{
    // This DTO combines ApplicationUser info with linked profile data and roles
    public class UserDto
    {
        public required string Id { get; set; } // Marked as required
        public required string Username { get; set; } // Marked as required
        public required string Email { get; set; } // Marked as required
        public required string FirstName { get; set; } // Marked as required
        public required string LastName { get; set; }  // Marked as required
        public List<string> Roles { get; set; } = new List<string>();

        // Optional: Include basic linked profile info if available
        public int? StudentId { get; set; }
        public int? TeacherId { get; set; }
        public int? ParentId { get; set; }
    }
}
