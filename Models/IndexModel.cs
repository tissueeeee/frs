namespace FR_HKVision.Models
{

    public class IndexModel
    {
        public string CourseCode { get; set; }
        public List<Student> Students { get; set; }
        public List<Student> StudentListAttend { get; set; }
        public List<Student> StudentListAbsent { get; set; }
        public List<Student> StudentListDebt { get; set; }

        public List<Student> StudentListStranger { get; set; }

        public StudentTotal StudentTotal { get; set; }

        public Programme Programme { get; set; }
        //public List<Program> Program { get; set; }

        public Message Message { get; set; }
    }

    public class Message{
        public string totalUniqueStudentMatchRecords { get; set; }
        public string MessageVersion { get; set; }

        public string MessageCamera { get; set; }

        public string MessagePerson { get; set; }

        public string MessageFaceMatchRecord { get; set; }

        public string MessageCourseCode { get; set; }
        public string MessageFacultyCode { get; set; }
    }

    public class Student
    {
        public string StudentName { get; set; }
        public string StudentNumber { get; set; }

        public string StudentPhoto { get; set; }
        public string ProgrammeName { get; set; }
        public string ProgrammeCode { get; set; }
        public string AttendanceTime { get; set; }
        public string AttendancePhoto { get; set; }

        public string ProfileStatus { get; set; }
        public string ProfileRecord { get; set; }
        

    }

    public class StudentTotal
    {
        public int numberStudentAttend { get; set; }
        public int numberStudentAbsent { get; set; }

        public int numberStudentDebt { get; set; }
        public int numberStudentStranger { get; set; }

    }

    public class Programme
    {
        public string programmeCode { get; set; }
        public string programmeName { get; set; }

        public string programmeLecturer { get; set; }
        
        public int programmeStudent { get; set; }

        public string programmeBlock { get; set; }
        public string programmeRoom { get; set; }

        public string programmeDate { get; set; }
        public string programmeTime { get; set; }

        public string programmeFacultyCode { get; set; }
        public string programmeOnEvery { get; set; }

    }

    public class ProgramInfo
    {
        public string COURSECODE { get; set; }

        public string COURSENAME { get; set; }
        public string FACULTYCODE { get; set; }

        public string ON_EVERY { get; set; }

        public string TIME_FROM { get; set; }
        public string TIME_TO { get; set; }

        public string LECTURER { get; set; }
        public string ROOM { get; set; }

    }

    

    public class StudentAttendance
    {
        public string StudentName { get; set; }
        public string StudentNumber { get; set; }

        public int TransactionId { get; set; }
        public int Attendance { get; set; }

        public int FeeDue { get; set; }
        //public DateTime CreateDate { get; set;
        public string CreateDate { get; set; }


    }

    public class StudentAttendanceStatus
    {
        public int TransactionId { get; set; }
        
        public int Attendance { get; set; }

        public string StudentNumber { get; set; }
    }

    public class StudentFeeStatus
    {
        public int TransactionId { get; set; }

        public int FeeDue { get; set; }

        public string StudentNumber { get; set; }
    }


    public class Transactions
    {
      
        public string LecturerName { get; set; }

        public string CourseCode { get; set; }
        public string Room { get; set; }

        public string Time { get; set; }
        public string TransactionDate { get; set; }
        public string CreateDate { get; set; }


    }

}
