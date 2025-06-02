// Models/DTOs/TeacherDto.cs
namespace SchoolApi.Models.DTOs
{
    public class TeacherDto
    {
        public int Id { get; set; }
        public string? Subject { get; set; }
        public string UserId { get; set; } = null!;
    }
}
