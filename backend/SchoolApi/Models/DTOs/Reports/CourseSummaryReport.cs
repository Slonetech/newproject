using System;
using System.Collections.Generic;

namespace SchoolApi.Models.DTOs.Reports
{
    public class CourseSummaryReport
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public int TotalStudents { get; set; }
        public double AverageGrade { get; set; }
        public double HighestGrade { get; set; }
        public double LowestGrade { get; set; }
        public double AttendanceRate { get; set; }
        public List<GradeDistribution> GradeDistribution { get; set; } = new List<GradeDistribution>();
    }

    public class GradeDistribution
    {
        public string Range { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}