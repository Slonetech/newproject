namespace SchoolApi.DTOs
{
    public class GradeCreateDto
    {
        public float Score { get; set; }
        public DateTime DateAwarded { get; set; }
        public int StudentId { get; set; }
        public int TeacherId { get; set; }
        public int CourseId { get; set; }
    }
}
