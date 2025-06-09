using System.ComponentModel.DataAnnotations;

namespace SchoolApi.Models.DTOs.Admin
{
    public class RoleAssignmentDto
    {
        [Required]
        public string? RoleName { get; set; }
    }
}
