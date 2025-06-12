using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolApi.Models.DTOs.Search
{
    public class StudentSearchResult
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Grade { get; set; }
        public List<CourseInfo> Courses { get; set; } = new();
    }

    public class CourseInfo
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
    }
}