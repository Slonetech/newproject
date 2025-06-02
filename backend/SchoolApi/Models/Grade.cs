namespace SchoolApi.Models
{
    public class Grade
    {
        public int Id { get; set; }
        public float Score { get; set; }
        public DateTime DateAwarded { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;
    }
}
