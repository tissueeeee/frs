using System;
using System.Collections.Generic;

namespace FR_HKVision.Models
{

    public class ReportViewModel
    {
        public List<StudentAttendanceReport> StudentAttendanceReport { get; set; } = new List<StudentAttendanceReport>();
        public List<StudentAttendanceReport> Students { get; set; } = new List<StudentAttendanceReport>();
        public List<Classroom> Classrooms { get; set; } = new List<Classroom>();
        public List<Course> Courses { get; set; } = new List<Course>();

        public string CourseCode { get; set; }   

        public string StudentNumber {get; set; }
        public string StudentName { get; set; }  
        public string Attendance { get; set; }  
        public string FeeDue { get; set; } 
        public string TransactionDate { get; set; }
        public string TransactionTime { get; set; }
    }

    public class StudentAttendanceReport
    {
        
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime AttendanceTime { get; set; }
        public string CourseCode { get; set; }

    }

    public class Classroom
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Course
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }


}
