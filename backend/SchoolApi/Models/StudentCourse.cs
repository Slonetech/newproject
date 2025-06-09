using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolApi.Models
{
    public class StudentCourse
    {
        public int StudentId { get; set; }
        public required Student Student { get; set; } // Mark as required

        public int CourseId { get; set; }
        public required Course Course { get; set; } // Mark as required

        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    }
}
