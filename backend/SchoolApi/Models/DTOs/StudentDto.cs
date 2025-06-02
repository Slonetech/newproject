// Models/DTOs/StudentDto.cs
namespace SchoolApi.Models.DTOs
{
    public class StudentDto
    {
        public int Id { get; set; }
        public string? AdmissionNumber { get; set; }
        public string? GradeLevel { get; set; }
        public string UserId { get; set; } = null!;
    }
}
