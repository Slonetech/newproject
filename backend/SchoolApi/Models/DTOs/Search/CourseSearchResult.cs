using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolApi.Models.DTOs.Search
{
    public class CourseSearchResult
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty; // Added
        public string Description { get; set; } = string.Empty;
        public List<string> TeacherNames { get; set; } = new List<string>(); // Changed from single string
        public int StudentCount { get; set; }
        public bool HasAttendance { get; set; }
    }
}