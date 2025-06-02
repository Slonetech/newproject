using Microsoft.AspNetCore.Identity;

namespace SchoolApi.Models
{
    public class ApplicationUser : IdentityUser
    {

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
        public virtual Student? Student { get; set; }
        public virtual Teacher? Teacher { get; set; }
        public virtual Parent? Parent { get; set; }
    }
}
