// Models/DTOs/CourseDto.cs
namespace SchoolApi.Models.DTOs
{
    public class CourseDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Credits { get; set; }
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
