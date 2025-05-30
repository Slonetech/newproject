using System.ComponentModel.DataAnnotations;

namespace SchoolApi.Models
{
    public class Grade
    {
        public int Id { get; set; }

        [Required]
        [Range(0, 100)]
        public double Score { get; set; }

        [Required]
        public DateTime AwardedDate { get; set; }

        // Foreign Keys
        public int CourseId { get; set; }
        public Course? Course { get; set; }

        public string? StudentId { get; set; }
        public Student? Student { get; set; }
    }
}
