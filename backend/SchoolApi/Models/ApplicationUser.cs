using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // Navigation properties
        public virtual Student? Student { get; set; }
        public virtual Teacher? Teacher { get; set; }
        public virtual Parent? Parent { get; set; }

        // Foreign keys
        [ForeignKey("Student")]
        public Guid? StudentId { get; set; }

        [ForeignKey("Teacher")]
        public Guid? TeacherId { get; set; }

        [ForeignKey("Parent")]
        public Guid? ParentId { get; set; }
    }
}
