using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolApi.Models
{
    public class Parent
    {
        [Key]
        public string? Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? FullName { get; set; }

        // Parent has many children (Students)
        public ICollection<Student> Students { get; set; } = new List<Student>();

        // Foreign key to ApplicationUser
        public string? UserId { get; set; }

        // Navigation property
        public ApplicationUser? User { get; set; }
    }
}
