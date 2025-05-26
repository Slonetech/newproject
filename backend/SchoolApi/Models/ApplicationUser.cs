using Microsoft.AspNetCore.Identity;

namespace SchoolApi.Models // ✅ Must match the namespace used in ApplicationDbContext
{
    public class ApplicationUser : IdentityUser
    {
        // Add custom properties here if needed
        // public string FullName { get; set; }
    }
}
