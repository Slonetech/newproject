using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Add this for [Required]

namespace SchoolApi.Models.DTOs
{
    public class LoginResponseDto
    {
        public required string Token { get; set; } // Mark as required
        public DateTime Expiration { get; set; }
        public required string Username { get; set; } // Mark as required
        [EmailAddress]
        public required string Email { get; set; } // Mark as required
        public required List<string> Roles { get; set; } // Mark as required
    }
}
