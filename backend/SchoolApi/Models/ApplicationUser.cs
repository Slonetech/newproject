using Microsoft.AspNetCore.Identity;

namespace SchoolApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public Parent? Parent { get; set; }
        public Student? Student { get; set; }
        public Teacher? Teacher { get; set; }

    }
}
