using System;
using System.Collections.Generic;

namespace SchoolApi.Models.DTOs.Reports
{
    public class StudentProgressReport
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public List<CourseProgress> Courses { get; set; } = new List<CourseProgress>();
    }

}