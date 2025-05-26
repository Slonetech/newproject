using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Models; // ✅ This line is essential

namespace SchoolApi.Data // ✅ Make sure this matches the folder/project structure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        // Optional: You do not need DbSet<ApplicationUser> unless you're doing custom queries on users
    }
}
