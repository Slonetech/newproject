using System.ComponentModel.DataAnnotations;

namespace SchoolApi.Models.DTOs.Users
{
    public class UpdateUserDto
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        public string? UserName { get; set; }

        public List<string>? Roles { get; set; }
    }
}