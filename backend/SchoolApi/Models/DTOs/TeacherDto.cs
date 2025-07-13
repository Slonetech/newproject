// Models/DTOs/TeacherDto.cs
using System.ComponentModel.DataAnnotations;

namespace SchoolApi.Models.DTOs
{
    public class TeacherDto
    {
        public int Id { get; set; }
        public string? Subject { get; set; }
        public string UserId { get; set; } = null!;
    }

    public class TeacherCreateDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Specialization { get; set; }
        public string? Department { get; set; }
        public DateTime DateOfBirth { get; set; } = DateTime.UtcNow.AddYears(-25); // Default to 25 years ago
        public DateTime HireDate { get; set; } = DateTime.UtcNow;
        public string? Address { get; set; }
    }
}
