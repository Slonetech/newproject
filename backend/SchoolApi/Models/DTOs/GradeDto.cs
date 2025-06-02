namespace SchoolApi.DTOs
{
    public class GradeDto
    {
        public int Id { get; set; }
        public float Score { get; set; }
        public DateTime DateAwarded { get; set; }
        public int StudentId { get; set; }
        public int TeacherId { get; set; }
        public int CourseId { get; set; }
        public string? StudentName { get; set; }
        public string? CourseName { get; set; }
    }
}
