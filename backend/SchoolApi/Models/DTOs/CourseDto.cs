// Models/DTOs/CourseDto.cs
namespace SchoolApi.Models.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
    }

    public class CreateCourseDto
    {
        public string Title { get; set; } = null!;
    }

    public class UpdateCourseDto
    {
        public string? Title { get; set; }
    }
}
