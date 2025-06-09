using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolApi.Models
{
    public class TeacherCourse
    {
        public int TeacherId { get; set; }
        public required Teacher Teacher { get; set; } // Mark as required

        public int CourseId { get; set; }
        public required Course Course { get; set; } // Mark as required

        public DateTime AssignmentDate { get; set; } = DateTime.UtcNow;
    }
}
