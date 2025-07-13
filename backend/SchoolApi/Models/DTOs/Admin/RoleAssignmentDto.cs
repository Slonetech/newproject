using System.ComponentModel.DataAnnotations;

namespace SchoolApi.Models.DTOs.Admin
{
    public class RoleAssignmentDto
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
