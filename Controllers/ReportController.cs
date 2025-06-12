using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using FR_HKVision.Models;
using Oracle.ManagedDataAccess.Client;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FR_HKVision.Controllers
{
    public class ReportController : Controller
    {
        private readonly IConfiguration _configuration; // Declare IConfiguration

        // Constructor to inject IConfiguration
        public ReportController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index(){
            //check if the users is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                TempData["ErrorMessage"] = "Please log in to access report view.";
                return RedirectToAction ("Login", "Account"); //Redirect to Login page
            }
            return View(); 
        }

        [HttpGet]
        public JsonResult GetFilteredTimeAndDate(string classroom)
        {
            string userId = _configuration["OracleDBSettings:UserId"];
            string password = _configuration["OracleDBSettings:Password"];
            string dataSource = _configuration["OracleDBSettings:DataSource"];

            // Fetch filtered time slots & dates for the selected classroom
            List<string> filteredTimes = OracleConnectionClass.GetFilteredTimes(userId, password, dataSource, classroom);
            List<string> filteredDates = OracleConnectionClass.GetFilteredDates(userId, password, dataSource, classroom);

            return Json(new { times = filteredTimes, dates = filteredDates });
        }
        
        [HttpGet]
        public JsonResult GetFilteredTimes(string classroom, string date)
        {
            Console.WriteLine($"Fetching Times for Classroom: {classroom}, Date: {date}");

            if (string.IsNullOrEmpty(classroom) || string.IsNullOrEmpty(date))
            {
                Console.WriteLine("Error: Missing classroom or date");
                return Json(new { times = new List<string>() });
            }

            string userId = _configuration["OracleDBSettings:UserId"];
            string password = _configuration["OracleDBSettings:Password"];
            string dataSource = _configuration["OracleDBSettings:DataSource"];

            List<string> filteredTimes = OracleConnectionClass.GetAvailableTimesForDate(userId, password, dataSource, classroom, date);

            Console.WriteLine($"Times Retrieved: {string.Join(", ", filteredTimes)}");
            return Json(new { times = filteredTimes });
        }

        [HttpGet]
        public JsonResult GetClassroomsByCourseCode(string courseCode)
        {
            if (string.IsNullOrEmpty(courseCode))
            {
                return Json(new { classrooms = new List<string>() });
            }

            string userId = _configuration["OracleDBSettings:UserId"];
            string password = _configuration["OracleDBSettings:Password"];
            string dataSource = _configuration["OracleDBSettings:DataSource"];

            List<string> classrooms = OracleConnectionClass.GetClassroomsByCourseCode(userId, password, dataSource, courseCode);
            
            return Json(new { classrooms = classrooms });
        }

        public IActionResult GetReport(string selectedCourseCode, string selectedClassroom, string selectedDate, string selectedTime)
        {
            string userId = _configuration["OracleDBSettings:UserId"];
            string password = _configuration["OracleDBSettings:Password"];
            string dataSource = _configuration["OracleDBSettings:DataSource"];

            // Fetch course codes
            List<ProgramInfo> courseCodes = OracleConnectionClass.GetAllCourseCodes(userId, password, dataSource);

            // Fetch classrooms (either all or filtered by course code)
            List<string> classrooms;
            if (!string.IsNullOrEmpty(selectedCourseCode))
            {
                classrooms = OracleConnectionClass.GetClassroomsByCourseCode(userId, password, dataSource, selectedCourseCode);
            }
            else
            {
                classrooms = OracleConnectionClass.GetClassrooms(userId, password, dataSource);
            }
            
            //Fetch time slots
            List<string> timeSlots = OracleConnectionClass.GetAvailableTimes(userId, password, dataSource);

            //Fetch dates
            List<string> availableDates = OracleConnectionClass.GetAvailableDates(userId, password, dataSource);

            // Fetch attendance report only if all filters are selected
            List<ReportViewModel> reportData = new List<ReportViewModel>();
            if (!string.IsNullOrEmpty(selectedClassroom) && !string.IsNullOrEmpty(selectedDate) && !string.IsNullOrEmpty(selectedTime))
            {
                reportData = OracleConnectionClass.GetAttendanceReport(userId, password, dataSource, selectedClassroom, selectedDate, selectedTime);
                
                // Ensure all report items have the correct course code
                foreach (var item in reportData)
                {
                    if (string.IsNullOrEmpty(item.CourseCode))
                    {
                        item.CourseCode = selectedCourseCode;
                    }
                }
            }

            // Debugging logs
            Console.WriteLine($"Selected Course: {selectedCourseCode}, Selected Room: {selectedClassroom}, Selected Date: {selectedDate}, Selected Time: {selectedTime}");
            Console.WriteLine("Records Retrieved: " + reportData.Count);

            // Pass data to view
            ViewData["CourseCodes"] = courseCodes;
            ViewData["Classrooms"] = classrooms;
            ViewData["TimeSlots"] = timeSlots;
            ViewData["SelectedDate"] = selectedDate;
            ViewData["SelectedTime"] = selectedTime;
            ViewData["AvailableDates"] = availableDates;
            ViewData["ReportData"] = reportData;
            ViewData["SelectedCourseCode"] = selectedCourseCode;
            ViewData["SelectedClassroom"] = selectedClassroom;

            return View();
        }
        
        [HttpGet]
        public IActionResult ExportToCsv(string selectedCourseCode, string selectedClassroom, string selectedDate, string selectedTime)
        {
            try
            {
                // Check if user is logged in
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                {
                    return RedirectToAction("Login", "Account");
                }
                
                // Get DB credentials
                string userId = _configuration["OracleDBSettings:UserId"];
                string password = _configuration["OracleDBSettings:Password"];
                string dataSource = _configuration["OracleDBSettings:DataSource"];
                
                // Fetch report data
                List<ReportViewModel> reportData = OracleConnectionClass.GetAttendanceReport(
                    userId, password, dataSource, selectedClassroom, selectedDate, selectedTime);
                
                if (reportData == null || !reportData.Any())
                {
                    return Content("No data available for export");
                }
                
                // Ensure all report items have the correct course code
                foreach (var item in reportData)
                {
                    if (string.IsNullOrEmpty(item.CourseCode))
                    {
                        item.CourseCode = selectedCourseCode;
                    }
                }
                
                // Build CSV content
                var csv = new StringBuilder();
                
                // Add headers
                csv.AppendLine("Count,Course Code,Student Name,Student ID,Attendance,Finance Statement");
                
                // Add data rows
                int count = 1;
                foreach (var record in reportData)
                {
                    string attendance = record.Attendance == "1" ? "Present" : "Absent";
                    string finance = record.FeeDue == "1" ? "Debtor" : "N/A";
                    
                    // Escape fields with commas
                    string studentName = record.StudentName.Contains(",") ? $"\"{record.StudentName}\"" : record.StudentName;
                    
                    csv.AppendLine($"{count},{selectedCourseCode},{studentName},{record.StudentNumber},{attendance},{finance}");
                    count++;
                }
                
                // Set file name
                string fileName = $"Attendance_Report_{selectedCourseCode}_{selectedDate}.csv";
                
                // Return CSV file
                return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting to CSV: {ex.Message}");
                return Content("Error generating CSV file: " + ex.Message);
            }
        }
        
        [HttpGet]
        public IActionResult ExportToPdf(string selectedCourseCode, string selectedClassroom, string selectedDate, string selectedTime)
        {
            try
            {
                // Check if user is logged in
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                {
                    return RedirectToAction("Login", "Account");
                }
                
                // Get DB credentials
                string userId = _configuration["OracleDBSettings:UserId"];
                string password = _configuration["OracleDBSettings:Password"];
                string dataSource = _configuration["OracleDBSettings:DataSource"];
                
                // Fetch report data
                List<ReportViewModel> reportData = OracleConnectionClass.GetAttendanceReport(
                    userId, password, dataSource, selectedClassroom, selectedDate, selectedTime);
                
                if (reportData == null || !reportData.Any())
                {
                    return Content("No data available for export");
                }
                
                // Ensure all report items have the correct course code
                foreach (var item in reportData)
                {
                    if (string.IsNullOrEmpty(item.CourseCode))
                    {
                        item.CourseCode = selectedCourseCode;
                    }
                }
                
                // This is a placeholder for server-side PDF generation
                // You would typically use a library like iTextSharp, PDFsharp, or DinkToPdf
                // For now, we'll return a simple HTML response that can be printed to PDF from the browser
                
                var html = new StringBuilder();
                html.AppendLine("<!DOCTYPE html>");
                html.AppendLine("<html>");
                html.AppendLine("<head>");
                html.AppendLine("<title>Attendance Report</title>");
                html.AppendLine("<style>");
                html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
                html.AppendLine("h1 { color: #333; }");
                html.AppendLine("table { border-collapse: collapse; width: 100%; margin-top: 20px; }");
                html.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
                html.AppendLine("th { background-color: #f2f2f2; }");
                html.AppendLine(".present { color: green; }");
                html.AppendLine(".absent { color: red; }");
                html.AppendLine(".debtor { color: red; }");
                html.AppendLine(".info { margin-bottom: 5px; }");
                html.AppendLine("</style>");
                html.AppendLine("</head>");
                html.AppendLine("<body>");
                html.AppendLine("<h1>Attendance Report</h1>");
                
                // Course information
                var courseInfo = OracleConnectionClass.GetAllCourseCodes(userId, password, dataSource).FirstOrDefault(c => c.COURSECODE == selectedCourseCode);
                string courseName = courseInfo?.COURSENAME ?? selectedCourseCode;
                
                html.AppendLine($"<p class='info'><strong>Course:</strong> {selectedCourseCode} - {courseName}</p>");
                html.AppendLine($"<p class='info'><strong>Classroom:</strong> {selectedClassroom}</p>");
                html.AppendLine($"<p class='info'><strong>Date:</strong> {selectedDate}</p>");
                html.AppendLine($"<p class='info'><strong>Time:</strong> {selectedTime}</p>");
                html.AppendLine($"<p class='info'><strong>Generated:</strong> {DateTime.Now}</p>");
                
                // Table with attendance data
                html.AppendLine("<table>");
                html.AppendLine("<thead>");
                html.AppendLine("<tr>");
                html.AppendLine("<th>Count</th>");
                html.AppendLine("<th>Course Code</th>");
                html.AppendLine("<th>Student Name</th>");
                html.AppendLine("<th>Student ID</th>");
                html.AppendLine("<th>Attendance</th>");
                html.AppendLine("<th>Finance Statement</th>");
                html.AppendLine("</tr>");
                html.AppendLine("</thead>");
                html.AppendLine("<tbody>");
                
                int count = 1;
                foreach (var record in reportData)
                {
                    string attendanceClass = record.Attendance == "1" ? "present" : "absent";
                    string attendanceText = record.Attendance == "1" ? "Present" : "Absent";
                    
                    string financeClass = record.FeeDue == "1" ? "debtor" : "";
                    string financeText = record.FeeDue == "1" ? "Debtor" : "N/A";
                    
                    html.AppendLine("<tr>");
                    html.AppendLine($"<td>{count++}</td>");
                    html.AppendLine($"<td>{selectedCourseCode}</td>");
                    html.AppendLine($"<td>{record.StudentName}</td>");
                    html.AppendLine($"<td>{record.StudentNumber}</td>");
                    html.AppendLine($"<td class='{attendanceClass}'>{attendanceText}</td>");
                    html.AppendLine($"<td class='{financeClass}'>{financeText}</td>");
                    html.AppendLine("</tr>");
                }
                
                html.AppendLine("</tbody>");
                html.AppendLine("</table>");
                
                // Add print script to automatically print when page loads
                html.AppendLine("<script>");
                html.AppendLine("window.onload = function() {");
                html.AppendLine("  window.print();");
                html.AppendLine("};");
                html.AppendLine("</script>");
                
                html.AppendLine("</body>");
                html.AppendLine("</html>");
                
                // Return HTML content that can be printed
                return Content(html.ToString(), "text/html");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting to PDF: {ex.Message}");
                return Content("Error generating PDF file: " + ex.Message);
            }
        }

        // Add this model for student details
        public class StudentDetailViewModel
        {
            public string StudentId { get; set; }
            public string StudentName { get; set; }
            public string ProfileStatus { get; set; }
            public bool IsAttending { get; set; }
            public bool IsDebtor { get; set; }
            public int ClassesAttended { get; set; }
            public int TotalClasses { get; set; }
        }
        
        // Keep the GetCourseStudentDetail endpoint for returning student data for the modal
        [HttpGet]
        public JsonResult GetCourseStudentDetail(string courseCode, string startDate, string endDate)
        {
            try
            {
                // Get credentials from configuration instead of session
                var userId = _configuration["OracleDBSettings:UserId"];
                var password = _configuration["OracleDBSettings:Password"];
                var dataSource = _configuration["OracleDBSettings:DataSource"];

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(dataSource))
                {
                    return Json(new { error = "Database configuration is missing. Please contact administrator." });
                }

                if (string.IsNullOrEmpty(courseCode))
                {
                    var dashboardData = OracleConnectionClass.GetDashboardData(userId, password, dataSource, startDate, endDate);
                    if (dashboardData == null)
                    {
                        return Json(new { error = "Error fetching dashboard data" });
                    }
                    return Json(dashboardData);
                }
                else
                {
                    var studentDetails = OracleConnectionClass.GetStudentDetails(userId, password, dataSource, courseCode, startDate, endDate);
                    if (studentDetails == null)
                    {
                        return Json(new { error = "Error fetching student details" });
                    }
                    return Json(new { students = studentDetails });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetChartData(string courseCode)
        {
            try
            {
                Console.WriteLine($"GetChartData called for course: {courseCode}");
                
                var diagnosticInfo = new Dictionary<string, string>();
                
                string userId = _configuration["OracleDBSettings:UserId"];
                string password = _configuration["OracleDBSettings:Password"];
                string dataSource = _configuration["OracleDBSettings:DataSource"];
                
                Console.WriteLine($"DB Connection - UserId: {userId}, DataSource: {dataSource}");
                diagnosticInfo["dbUserId"] = userId ?? "null";
                diagnosticInfo["dbDataSource"] = dataSource ?? "null";
                
                var reportData = new List<ReportViewModel>();
                
                if (!string.IsNullOrEmpty(courseCode))
                {
                    List<string> classrooms = OracleConnectionClass.GetClassroomsByCourseCode(userId, password, dataSource, courseCode);
                    Console.WriteLine($"Found {classrooms.Count} classrooms for course {courseCode}");
                    diagnosticInfo["classroomsFound"] = classrooms.Count.ToString();
                    
                    if (classrooms.Count == 0)
                    {
                        return Json(new { error = "No classrooms found for this course." });
                    }
                    
                    foreach (var classroom in classrooms)
                    {
                        string recentDate = OracleConnectionClass.GetMostRecentDate(userId, password, dataSource, classroom, courseCode);
                        
                        string recentTime = string.Empty;
                        if (!string.IsNullOrEmpty(recentDate)) {
                            recentTime = OracleConnectionClass.GetMostRecentTime(userId, password, dataSource, classroom, courseCode, recentDate);
                        }
                        
                        diagnosticInfo[$"classroom_{classroom}_date"] = recentDate ?? "null";
                        diagnosticInfo[$"classroom_{classroom}_time"] = recentTime ?? "null";
                        
                        if (!string.IsNullOrEmpty(recentDate) && !string.IsNullOrEmpty(recentTime))
                        {
                            var classroomData = OracleConnectionClass.GetAttendanceReport(userId, password, dataSource, classroom, recentDate, recentTime);
                            diagnosticInfo[$"classroom_{classroom}_records"] = classroomData.Count.ToString();
                            reportData.AddRange(classroomData);
                        }
                    }
                }
                
                if (reportData.Count == 0)
                {
                    return Json(new { error = "No data found for this course." });
                }
                
                int totalStudents = reportData.Count;
                int debtorCount = reportData.Count(s => s.FeeDue == "1");
                int nonDebtorCount = totalStudents - debtorCount;
                int attendingCount = reportData.Count(s => s.Attendance == "1");
                int absentCount = totalStudents - attendingCount;
                int debtorAttendingCount = reportData.Count(s => s.FeeDue == "1" && s.Attendance == "1");
                
                return Json(new
                {
                    totalStudents,
                    debtorCount,
                    nonDebtorCount,
                    attendingCount,
                    absentCount,
                    debtorAttendingCount,
                    courseCode,
                    diagnosticInfo
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetChartData: {ex.Message}");
                return Json(new { error = "An error occurred while fetching the data." });
            }
        }
    }
}