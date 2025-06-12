using System;
using System.Collections.Generic;

namespace SchoolApi.Models.DTOs.Reports
{
    public class StudentProgressReport
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public List<CourseProgress> Courses { get; set; } = new List<CourseProgress>();
    }

    public class CourseProgress
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public double AverageGrade { get; set; }
        public double AttendanceRate { get; set; }
        public double LastGrade { get; set; }
        public DateTime LastGradeDate { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}