using System.ComponentModel.DataAnnotations;

namespace SchoolApi.Models.DTOs.Users
{
    public class RoleAssignmentDto
    {
        [Required]
        public List<string> Roles { get; set; } = new List<string>();
    }
}