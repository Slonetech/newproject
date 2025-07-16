using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace SchoolApi.Models.DTOs.Users
{
    [SwaggerSchema("UsersRoleAssignmentDto")]
    public class RoleAssignmentDto
    {
        [Required]
        public List<string> Roles { get; set; } = new List<string>();
    }
}