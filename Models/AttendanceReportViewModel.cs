using System;

namespace FR_HKVision.Models
{
    public class AttendanceReportViewModel
    {
        public string StudentNumber { get; set; }
        public string StudentName { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string Classroom { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionTime { get; set; }
        public string Attendance { get; set; }
        public string FeeDue { get; set; }
    }
} 