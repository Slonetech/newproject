using System.ComponentModel.DataAnnotations;

namespace SchoolApi.Models
{
    public class Parent
    {
        [Key]
        public int Id { get; set; }

        public ICollection<Student> Students { get; set; } = new List<Student>();

        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}
