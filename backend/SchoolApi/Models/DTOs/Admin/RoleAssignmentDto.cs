using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace SchoolApi.Models.DTOs.Admin
{
    [SwaggerSchema("AdminRoleAssignmentDto")]
    public class RoleAssignmentDto
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
