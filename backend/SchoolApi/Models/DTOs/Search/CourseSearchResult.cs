using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolApi.Models.DTOs.Search
{
    public class CourseSearchResult
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public bool HasAttendance { get; set; }
    }
}