namespace SchoolApi.Models.DTOs
{
    public class AssignTeacherDto
    {
        public Guid TeacherId { get; set; }
        public Guid CourseId { get; set; }
    }
}