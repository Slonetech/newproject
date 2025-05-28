using System.ComponentModel.DataAnnotations;

namespace SchoolApi.Models
{
    public class Teacher
    {
        [Key]
        public int Id { get; set; }

        public string? FullName { get; set; }

        public string? Subject { get; set; }

        // Add more properties as needed
    }
}
