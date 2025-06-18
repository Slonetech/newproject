using System;
using System.Collections.Generic;

namespace SchoolApi.Models.DTOs.Reports
{
    public class CourseSummaryReport
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public int TotalStudents { get; set; }
        public decimal AverageGrade { get; set; }
        public decimal HighestGrade { get; set; }
        public decimal LowestGrade { get; set; }
        public double AttendanceRate { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public List<string> Teachers { get; set; } = new List<string>(); // Or a specific DTO if needed
        public List<GradeDistribution> GradeDistribution { get; set; } = new List<GradeDistribution>();
    }

    public class GradeDistribution
    {
        public string Range { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}