using CSHCSDKDemo;
using FR_HKVision.Models;
using Microsoft.AspNetCore.Mvc;
using Swan.Formatters;
using System;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json.Linq;
using System.Data;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http; // Required for Session
using FR_HKVision.Services;

namespace FR_HKVision.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IEmailService emailService)
        {
            _logger = logger;
            _configuration = configuration;
            _emailService = emailService;
        }

        public IActionResult Index(string courseCode = null)
        {
            string OracelDBUserId = _configuration["OracleDBSettings:UserId"];
            string OracelDBPassword = _configuration["OracleDBSettings:Password"];
            string OracelDBDataSource = _configuration["OracleDBSettings:DataSource"];

            // Get all available course codes
            var courseCodes = OracleConnectionClass.GetAllCourseCodes(OracelDBUserId, OracelDBPassword, OracelDBDataSource);
            ViewBag.CourseCodes = courseCodes;

            // If no course code is selected, return empty view
            if (string.IsNullOrEmpty(courseCode))
            {
                return View(new IndexModel());
            }

            // Store the selected course code in session
            HttpContext.Session.SetString("Coursecode", courseCode);

            string todayDate = DateTime.Today.ToString("yyyy-MM-dd");
            string OracelDBFacultyCode = _configuration["OracleDBSettings:FacultyCode"];
            string OracelDBCourseCode = courseCode;

            // Initialize the model
            var model = new IndexModel
            {
                CourseCode = courseCode,
                Message = new Message
                {
                    MessageFacultyCode = OracelDBFacultyCode,
                    MessageCourseCode = OracelDBCourseCode,
                    MessageVersion = "",
                    MessageCamera = "",
                    MessagePerson = "",
                    MessageFaceMatchRecord = ""
                }
            };

            // Get program info
            ProgramInfo info = OracleConnectionClass.programInfoByCoursecode(OracelDBUserId, OracelDBPassword, OracelDBDataSource, OracelDBCourseCode);
            if (info != null)
            {
                model.Programme = new Programme
                {
                    programmeCode = info.COURSECODE,
                    programmeName = info.COURSENAME,
                    programmeFacultyCode = info.FACULTYCODE,
                    programmeLecturer = info.LECTURER,
                    programmeRoom = info.ROOM,
                    programmeOnEvery = info.ON_EVERY,
                    programmeTime = info.TIME_FROM + '-' + info.TIME_TO,
                    programmeDate = todayDate,
                    programmeBlock = "E"
                };
            }

            // Get student list
            List<Student> studentList = OracleConnectionClass.studentListByCoursecode(OracelDBUserId, OracelDBPassword, OracelDBDataSource, OracelDBCourseCode);
            model.Students = studentList;

            // Initialize student lists
            model.StudentListAttend = new List<Student>();
            model.StudentListAbsent = new List<Student>();
            model.StudentListDebt = new List<Student>();
            model.StudentListStranger = new List<Student>();

            // Add all students to absent list by default
            model.StudentListAbsent.AddRange(studentList);

            // Get transaction info
            int TransactionId = OracleConnectionClass.TransactionIsExist(OracelDBUserId, OracelDBPassword, OracelDBDataSource, OracelDBCourseCode, model.Programme?.programmeRoom ?? "", todayDate);
            if (TransactionId == 0)
            {
                Transactions transactions = new Transactions
                {
                    LecturerName = model.Programme?.programmeLecturer ?? "",
                    CourseCode = OracelDBCourseCode,
                    Room = model.Programme?.programmeRoom ?? "",
                    Time = model.Programme?.programmeTime ?? "",
                    TransactionDate = todayDate,
                    CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
                TransactionId = OracleConnectionClass.insertTransaction(OracelDBUserId, OracelDBPassword, OracelDBDataSource, transactions);
            }

            // Process each student's attendance status
            foreach (var student in studentList)
            {
                int attendanceStatus = OracleConnectionClass.StudentAttendanceIsExist(OracelDBUserId, OracelDBPassword, OracelDBDataSource, student.StudentNumber, TransactionId);
                
                // Check if student exists in attendance table, if not create record with absent status
                if (attendanceStatus == 0)
                {
                    StudentAttendance studentAttendance = new StudentAttendance
                    {
                        StudentNumber = student.StudentNumber,
                        StudentName = student.StudentName,
                        TransactionId = TransactionId,
                        Attendance = 0, // Initialize as absent (0)
                        FeeDue = 0,
                        CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                    OracleConnectionClass.insertStudentAttendance(OracelDBUserId, OracelDBPassword, OracelDBDataSource, studentAttendance);
                }

                // Get updated attendance status
                attendanceStatus = OracleConnectionClass.StudentAttendanceIsExist(OracelDBUserId, OracelDBPassword, OracelDBDataSource, student.StudentNumber, TransactionId);

                // Only move student to attendance list if they have been marked as present
                if (attendanceStatus == 1)
                {
                    var studentToMove = model.StudentListAbsent.FirstOrDefault(s => s.StudentNumber == student.StudentNumber);
                    if (studentToMove != null)
                    {
                        model.StudentListAbsent.Remove(studentToMove);
                        model.StudentListAttend.Add(studentToMove);
                    }
                }

                // Check if student is a debtor
                if (student.ProfileStatus == "Debtor")
                {
                    model.StudentListDebt.Add(student);
                }
            }

            // Update student totals
            model.StudentTotal = new StudentTotal
            {
                numberStudentAttend = model.StudentListAttend.Count,
                numberStudentAbsent = model.StudentListAbsent.Count,
                numberStudentDebt = model.StudentListDebt.Count,
                numberStudentStranger = model.StudentListStranger.Count
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(IndexModel model)
        {
            if (string.IsNullOrEmpty(model.CourseCode))
            {
                // If no course code is selected, return to the same page with empty model
                return RedirectToAction("Index");
            }

            // Store the selected course code in session
            HttpContext.Session.SetString("Coursecode", model.CourseCode);

            // Get all available course codes for the dropdown
            string OracelDBUserId = _configuration["OracleDBSettings:UserId"];
            string OracelDBPassword = _configuration["OracleDBSettings:Password"];
            string OracelDBDataSource = _configuration["OracleDBSettings:DataSource"];
            var courseCodes = OracleConnectionClass.GetAllCourseCodes(OracelDBUserId, OracelDBPassword, OracelDBDataSource);
            ViewBag.CourseCodes = courseCodes;

            // Initialize the model with the selected course code
            model = new IndexModel
            {
                CourseCode = model.CourseCode,
                Message = new Message
                {
                    MessageFacultyCode = _configuration["OracleDBSettings:FacultyCode"],
                    MessageCourseCode = model.CourseCode,
                    MessageVersion = "",
                    MessageCamera = "",
                    MessagePerson = "",
                    MessageFaceMatchRecord = ""
                }
            };

            // Get program info
            ProgramInfo info = OracleConnectionClass.programInfoByCoursecode(OracelDBUserId, OracelDBPassword, OracelDBDataSource, model.CourseCode);
            if (info != null)
            {
                model.Programme = new Programme
                {
                    programmeCode = info.COURSECODE,
                    programmeName = info.COURSENAME,
                    programmeFacultyCode = info.FACULTYCODE,
                    programmeLecturer = info.LECTURER,
                    programmeRoom = info.ROOM,
                    programmeOnEvery = info.ON_EVERY,
                    programmeTime = info.TIME_FROM + '-' + info.TIME_TO,
                    programmeDate = DateTime.Today.ToString("yyyy-MM-dd"),
                    programmeBlock = "E"
                };
            }

            // Get student list
            List<Student> studentList = OracleConnectionClass.studentListByCoursecode(OracelDBUserId, OracelDBPassword, OracelDBDataSource, model.CourseCode);
            model.Students = studentList;

            // Initialize student lists
            model.StudentListAttend = new List<Student>();
            model.StudentListAbsent = new List<Student>();
            model.StudentListDebt = new List<Student>();
            model.StudentListStranger = new List<Student>();

            // Add all students to absent list by default
            model.StudentListAbsent.AddRange(studentList);

            // Get transaction info
            int TransactionId = OracleConnectionClass.TransactionIsExist(OracelDBUserId, OracelDBPassword, OracelDBDataSource, model.CourseCode, model.Programme?.programmeRoom ?? "", DateTime.Today.ToString("yyyy-MM-dd"));
            if (TransactionId == 0)
            {
                // Ensure we have the lecturer name from program info
                if (info == null || string.IsNullOrEmpty(info.LECTURER))
                {
                    // If no program info or lecturer name, redirect back with error
                    TempData["ErrorMessage"] = "Unable to find lecturer information for this course.";
                    return RedirectToAction("Index");
                }

                Transactions transactions = new Transactions
                {
                    LecturerName = info.LECTURER, // Use lecturer name from program info
                    CourseCode = model.CourseCode,
                    Room = model.Programme?.programmeRoom ?? "",
                    Time = model.Programme?.programmeTime ?? "",
                    TransactionDate = DateTime.Today.ToString("yyyy-MM-dd"),
                    CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
                TransactionId = OracleConnectionClass.insertTransaction(OracelDBUserId, OracelDBPassword, OracelDBDataSource, transactions);
            }

            // Process each student's attendance status
            foreach (var student in studentList)
            {
                int attendanceStatus = OracleConnectionClass.StudentAttendanceIsExist(OracelDBUserId, OracelDBPassword, OracelDBDataSource, student.StudentNumber, TransactionId);
                
                // Check if student exists in attendance table, if not create record with absent status
                if (attendanceStatus == 0)
                {
                    StudentAttendance studentAttendance = new StudentAttendance
                    {
                        StudentNumber = student.StudentNumber,
                        StudentName = student.StudentName,
                        TransactionId = TransactionId,
                        Attendance = 0, // Initialize as absent (0)
                        FeeDue = 0,
                        CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                    OracleConnectionClass.insertStudentAttendance(OracelDBUserId, OracelDBPassword, OracelDBDataSource, studentAttendance);
                }

                // Get updated attendance status
                attendanceStatus = OracleConnectionClass.StudentAttendanceIsExist(OracelDBUserId, OracelDBPassword, OracelDBDataSource, student.StudentNumber, TransactionId);

                // Only move student to attendance list if they have been marked as present
                if (attendanceStatus == 1)
                {
                    var studentToMove = model.StudentListAbsent.FirstOrDefault(s => s.StudentNumber == student.StudentNumber);
                    if (studentToMove != null)
                    {
                        model.StudentListAbsent.Remove(studentToMove);
                        model.StudentListAttend.Add(studentToMove);
                    }
                }

                // Check if student is a debtor
                if (student.ProfileStatus == "Debtor")
                {
                    model.StudentListDebt.Add(student);
                }
            }

            // Update student totals
            model.StudentTotal = new StudentTotal
            {
                numberStudentAttend = model.StudentListAttend.Count,
                numberStudentAbsent = model.StudentListAbsent.Count,
                numberStudentDebt = model.StudentListDebt.Count,
                numberStudentStranger = model.StudentListStranger.Count
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult StoreData(string courseCode)
        {
            _logger.LogInformation($"Starting data storage process for course: {courseCode}");

            if (string.IsNullOrEmpty(courseCode))
            {
                _logger.LogWarning("No course code provided for data storage");
                TempData["ErrorMessage"] = "No course code provided.";
                return RedirectToAction("Index");
            }

            string OracelDBUserId = _configuration["OracleDBSettings:UserId"];
            string OracelDBPassword = _configuration["OracleDBSettings:Password"];
            string OracelDBDataSource = _configuration["OracleDBSettings:DataSource"];
            string todayDate = DateTime.Today.ToString("yyyy-MM-dd");

            _logger.LogInformation($"Retrieving program info for course: {courseCode}");
            // Get program info
            ProgramInfo info = OracleConnectionClass.programInfoByCoursecode(OracelDBUserId, OracelDBPassword, OracelDBDataSource, courseCode);
            if (info == null)
            {
                _logger.LogError($"Unable to find course information for course: {courseCode}");
                TempData["ErrorMessage"] = "Unable to find course information.";
                return RedirectToAction("Index");
            }

            _logger.LogInformation($"Checking transaction existence for course: {courseCode}");
            // Get transaction info
            int TransactionId = OracleConnectionClass.TransactionIsExist(OracelDBUserId, OracelDBPassword, OracelDBDataSource, courseCode, info.ROOM, todayDate);
            if (TransactionId == 0)
            {
                _logger.LogInformation($"Creating new transaction for course: {courseCode}");
                // Create new transaction
                Transactions transactions = new Transactions
                {
                    LecturerName = info.LECTURER,
                    CourseCode = courseCode,
                    Room = info.ROOM,
                    Time = info.TIME_FROM + '-' + info.TIME_TO,
                    TransactionDate = todayDate,
                    CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
                TransactionId = OracleConnectionClass.insertTransaction(OracelDBUserId, OracelDBPassword, OracelDBDataSource, transactions);
                _logger.LogInformation($"New transaction created with ID: {TransactionId}");
            }
            else
            {
                _logger.LogInformation($"Using existing transaction with ID: {TransactionId}");
            }

            _logger.LogInformation($"Retrieving student list for course: {courseCode}");
            // Get student list
            List<Student> studentList = OracleConnectionClass.studentListByCoursecode(OracelDBUserId, OracelDBPassword, OracelDBDataSource, courseCode);
            _logger.LogInformation($"Found {studentList.Count} students for course: {courseCode}");

            int newRecordsCount = 0;
            // Process each student's attendance
            foreach (var student in studentList)
            {
                int attendanceStatus = OracleConnectionClass.StudentAttendanceIsExist(OracelDBUserId, OracelDBPassword, OracelDBDataSource, student.StudentNumber, TransactionId);
                
                if (attendanceStatus == 0)
                {
                    _logger.LogInformation($"Creating attendance record for student: {student.StudentNumber}");
                    // Create new attendance record
                    StudentAttendance studentAttendance = new StudentAttendance
                    {
                        StudentNumber = student.StudentNumber,
                        StudentName = student.StudentName,
                        TransactionId = TransactionId,
                        Attendance = 0,
                        FeeDue = student.ProfileStatus == "Debtor" ? 1 : 0,
                        CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                    OracleConnectionClass.insertStudentAttendance(OracelDBUserId, OracelDBPassword, OracelDBDataSource, studentAttendance);
                    newRecordsCount++;
                }
            }

            _logger.LogInformation($"Data storage completed successfully for course: {courseCode}");
            _logger.LogInformation($"Total new records created: {newRecordsCount}");
            _logger.LogInformation($"Total students processed: {studentList.Count}");

            TempData["SuccessMessage"] = "Data has been successfully verified and stored.";
            return RedirectToAction("Index", new { courseCode = courseCode });
        }

        [HttpPost]
        public IActionResult UpdateAttendanceStatus([FromBody] AttendanceUpdateModel model)
        {
            try
            {
                _logger.LogInformation($"Received attendance update request - Student: {model.StudentNumber}, Status: {model.AttendanceStatus}");

                if (string.IsNullOrEmpty(model.StudentNumber))
                {
                    return Json(new { success = false, message = "Student number is required" });
                }

                string OracelDBUserId = _configuration["OracleDBSettings:UserId"];
                string OracelDBPassword = _configuration["OracleDBSettings:Password"];
                string OracelDBDataSource = _configuration["OracleDBSettings:DataSource"];
                string OracelDBCourseCode = HttpContext.Session.GetString("Coursecode");

                if (string.IsNullOrEmpty(OracelDBCourseCode))
                {
                    return Json(new { success = false, message = "No course selected" });
                }

                string todayDate = DateTime.Today.ToString("yyyy-MM-dd");
                _logger.LogInformation($"Processing update for course: {OracelDBCourseCode}, Date: {todayDate}");

                // Get program info to retrieve the room
                ProgramInfo info = OracleConnectionClass.programInfoByCoursecode(
                    OracelDBUserId,
                    OracelDBPassword,
                    OracelDBDataSource,
                    OracelDBCourseCode
                );

                if (info == null)
                {
                    return Json(new { success = false, message = "Course information not found" });
                }

                // Get transaction ID for current course, room, and date
                int transactionId = OracleConnectionClass.TransactionIsExist(
                    OracelDBUserId,
                    OracelDBPassword,
                    OracelDBDataSource,
                    OracelDBCourseCode,
                    info.ROOM,
                    todayDate
                );

                if (transactionId <= 0)
                {
                    return Json(new { success = false, message = "No active transaction found for this course and date" });
                }

                // Update attendance status
                int result = OracleConnectionClass.updateStudentAttendanceStatus(
                    OracelDBUserId,
                    OracelDBPassword,
                    OracelDBDataSource,
                    model.StudentNumber,
                    transactionId,
                    model.AttendanceStatus
                );

                // Handle different result codes
                switch (result)
                {
                    case 0:
                        _logger.LogInformation($"Successfully updated attendance for student {model.StudentNumber}");
                        return Json(new { success = true, message = "Attendance updated successfully" });
                    case -1:
                        _logger.LogWarning($"Update failed for student {model.StudentNumber}");
                        return Json(new { success = false, message = "Failed to update attendance status" });
                    case -2:
                        _logger.LogWarning($"Record exists but update failed for student {model.StudentNumber}");
                        return Json(new { success = false, message = "Record exists but update failed. Please try again." });
                    case -3:
                        _logger.LogError($"Database error occurred for student {model.StudentNumber}");
                        return Json(new { success = false, message = "A database error occurred. Please try again." });
                    case -4:
                        _logger.LogError($"General error occurred for student {model.StudentNumber}");
                        return Json(new { success = false, message = "An unexpected error occurred. Please try again." });
                    default:
                        _logger.LogError($"Unknown error code {result} for student {model.StudentNumber}");
                        return Json(new { success = false, message = "An unknown error occurred. Please try again." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating attendance status: {ex.Message}");
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendReminder(string courseCode)
        {
            try
            {
                if (string.IsNullOrEmpty(courseCode))
                {
                    return Json(new { success = false, message = "Course code is required" });
                }

                string OracelDBUserId = _configuration["OracleDBSettings:UserId"];
                string OracelDBPassword = _configuration["OracleDBSettings:Password"];
                string OracelDBDataSource = _configuration["OracleDBSettings:DataSource"];

                // Get program info
                var programInfo = OracleConnectionClass.programInfoByCoursecode(
                    OracelDBUserId,
                    OracelDBPassword,
                    OracelDBDataSource,
                    courseCode
                );

                if (programInfo == null)
                {
                    return Json(new { success = false, message = "Course information not found" });
                }

                // Get students for this class
                var students = OracleConnectionClass.studentListByCoursecode(
                    OracelDBUserId,
                    OracelDBPassword,
                    OracelDBDataSource,
                    courseCode
                );

                if (students == null || !students.Any())
                {
                    return Json(new { success = false, message = "No students found for this course" });
                }

                // Send reminder to each student
                foreach (var student in students)
                {
                    // In a real application, you would get the email from the student record
                    string studentEmail = $"{student.StudentNumber}@example.com"; // Dummy email

                    await _emailService.SendClassReminderAsync(
                        studentEmail,
                        student.StudentName,
                        programInfo.COURSENAME,
                        programInfo.COURSECODE,
                        programInfo.ROOM,
                        $"{programInfo.TIME_FROM}-{programInfo.TIME_TO}"
                    );
                }

                _logger.LogInformation($"Sent reminders for course {courseCode} to {students.Count} students");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending reminders");
                return Json(new { success = false, message = ex.Message });
            }
        }

        public class AttendanceUpdateModel
        {
            public string StudentNumber { get; set; }
            public int AttendanceStatus { get; set; }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
