using System;

namespace SchoolApi.Models
{
    public class ParentChild
    {
        public Guid ParentId { get; set; }
        public Parent Parent { get; set; } = null!;
        public Guid StudentId { get; set; }
        public Student Student { get; set; } = null!;
    }
}