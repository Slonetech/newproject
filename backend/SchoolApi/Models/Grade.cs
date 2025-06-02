using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolApi.Models
{
    public class Grade
{
    public int Id { get; set; }

    [Required]
    public float Score { get; set; }

    [Required]
    public DateTime DateAwarded { get; set; }

    [Required]
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;

    [Required]
    public int TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;

    [Required]
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
}

}
