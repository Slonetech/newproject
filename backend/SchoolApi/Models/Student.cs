using System.ComponentModel.DataAnnotations;

namespace SchoolApi.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        public string? FullName { get; set; }
    }
}
