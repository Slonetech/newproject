using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;

namespace SchoolApi.Models
{
    public class Teacher
{
    public int Id { get; set; }

    [Required]
    public string Subject { get; set; } = null!;

    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    public ICollection<Grade> Grades { get; set; } = new List<Grade>();

    [Required]
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}

}
