using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolApi.Models
{
    public class Student
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string AdmissionNumber { get; set; } = null!;

    [Required]
    public string GradeLevel { get; set; } = null!;

    public int? ParentId { get; set; }
    public Parent? Parent { get; set; }

    public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    public ICollection<Grade> Grades { get; set; } = new List<Grade>();

    [Required]
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}

}
