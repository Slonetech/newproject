using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolApi.Models
{
    public class Parent
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public ICollection<Student> Children { get; set; }

        public Parent()
        {
            FullName = string.Empty;
            Email = string.Empty;
            PhoneNumber = string.Empty;
            Children = new List<Student>();
        }
    }
}