using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolApi.Models
{
    public class Teacher
{
    [Key]
    public int Id { get; set; }  // 👈 This is critical!
    
    [Required]
    [StringLength(100)]
    public string? FullName { get; set; }

    // Courses they teach
    public ICollection<Course> Courses { get; set; } = new List<Course>();

    // FK to ApplicationUser
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
}

}
