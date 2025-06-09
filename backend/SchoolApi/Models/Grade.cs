using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolApi.Models
{
    public class Grade
    {
        [Key]
        public int GradeId { get; set; } // Changed to GradeId from Id
        [Required]
        [Range(0, 100)]
        public decimal Score { get; set; }
        [Required]
        public DateTime DateAwarded { get; set; }
        [StringLength(500)]
        public string? Comments { get; set; } // Made nullable, as comments can be optional

        // Foreign keys and navigation properties
        public int StudentId { get; set; }
        public required Student Student { get; set; } // Mark as required

        public int CourseId { get; set; }
        public required Course Course { get; set; } // Mark as required

        public int TeacherId { get; set; }
        public required Teacher Teacher { get; set; } // Mark as required
    }
}
