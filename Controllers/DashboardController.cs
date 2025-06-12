using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using FR_HKVision.Models;
using Oracle.ManagedDataAccess.Client;

namespace FR_HKVision.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IConfiguration _configuration;

        public DashboardController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                TempData["ErrorMessage"] = "Please log in to access dashboard.";
                return RedirectToAction("Login", "Account");
            }

            string userId = _configuration["OracleDBSettings:UserId"];
            string password = _configuration["OracleDBSettings:Password"];
            string dataSource = _configuration["OracleDBSettings:DataSource"];

            var faculties = OracleConnectionClass.GetAllFacultyCodes(userId, password, dataSource);
            return View(faculties);
        }

        [HttpGet]
        public JsonResult GetCoursesByFaculty(string facultyCode)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return Json(new { error = "Session expired. Please log in again." });
            }

            string userId = _configuration["OracleDBSettings:UserId"];
            string password = _configuration["OracleDBSettings:Password"];
            string dataSource = _configuration["OracleDBSettings:DataSource"];

            try
            {
                var courses = OracleConnectionClass.GetCoursesByFacultyCode(userId, password, dataSource, facultyCode);
                
                // Log the data being returned
                Console.WriteLine($"Returning {courses.Count} courses for faculty {facultyCode}");
                if (courses.Count > 0)
                {
                    Console.WriteLine($"First course: Code={courses[0].COURSECODE}, Name={courses[0].COURSENAME}");
                }
                
                return Json(courses);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCoursesByFaculty: {ex.Message}");
                return Json(new { error = "An error occurred while fetching courses." });
            }
        }

        [HttpGet]
        public JsonResult GetCourseStatistics(string courseCode)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return Json(new { error = "Session expired. Please log in again." });
            }

            string userId = _configuration["OracleDBSettings:UserId"];
            string password = _configuration["OracleDBSettings:Password"];
            string dataSource = _configuration["OracleDBSettings:DataSource"];

            try
            {
                Console.WriteLine($"Getting statistics for course: {courseCode}");
                
                // Get today's date for the transaction check
                string today = DateTime.Today.ToString("yyyy-MM-dd");
                
                // Get all students and their attendance/payment status for this course
                string sql = @"
                    SELECT 
                        COUNT(DISTINCT s.STUDENT_NUMBER) as TOTAL_STUDENTS,
                        COUNT(DISTINCT CASE WHEN UPPER(s.PROFILE_STATUS) = 'DEBTOR' THEN s.STUDENT_NUMBER END) as DEBTORS,
                        SUM(CASE WHEN ts.ATTENDANCE = '1' THEN 1 ELSE 0 END) as ATTENDING_COUNT,
                        COUNT(DISTINCT t.TRANSACTION_ID) as TOTAL_CLASSES
                    FROM FR_STUDENT s
                    JOIN FR_STUDENT_COURSE sc ON s.STUDENT_NUMBER = sc.STUDENT_NUMBER
                    LEFT JOIN FR_TRANSACTION t ON sc.COURSE_CODE = t.COURSE_CODE
                    LEFT JOIN FR_TRANSACTION_STUDENT ts ON t.TRANSACTION_ID = ts.TRANSACTION_ID 
                        AND ts.STUDENT_NUMBER = s.STUDENT_NUMBER
                    WHERE sc.COURSE_CODE = :courseCode
                    GROUP BY sc.COURSE_CODE";

                using (var conn = new Oracle.ManagedDataAccess.Client.OracleConnection(
                    $"User Id={userId};Password={password};Data Source={dataSource}"))
                {
                    conn.Open();
                    using (var cmd = new Oracle.ManagedDataAccess.Client.OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("courseCode", courseCode));
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int totalStudents = Convert.ToInt32(reader["TOTAL_STUDENTS"]);
                                int debtors = Convert.ToInt32(reader["DEBTORS"]);
                                int attendingCount = Convert.ToInt32(reader["ATTENDING_COUNT"]);
                                int totalClasses = Convert.ToInt32(reader["TOTAL_CLASSES"]);

                                Console.WriteLine($"Stats for {courseCode}:");
                                Console.WriteLine($"Total Students: {totalStudents}");
                                Console.WriteLine($"Debtors: {debtors}");
                                Console.WriteLine($"Attending Count: {attendingCount}");
                                Console.WriteLine($"Total Classes: {totalClasses}");

                                // Calculate attendance rate based on actual attendance records
                                double attendanceRate = 0;
                                if (totalStudents > 0 && totalClasses > 0)
                                {
                                    // Calculate as (total attended sessions) / (total possible sessions)
                                    // where total possible sessions = students × classes
                                    attendanceRate = (attendingCount * 100.0) / (totalStudents * totalClasses);
                                }

                                // For debugging, let's also show some sample data
                                string debugSql = @"
                                    SELECT 
                                        s.STUDENT_NUMBER, 
                                        s.STUDENT_NAME, 
                                        s.PROFILE_STATUS,
                                        COUNT(CASE WHEN ts.ATTENDANCE = '1' THEN 1 END) as ATTENDANCE_COUNT
                                    FROM FR_STUDENT s
                                    JOIN FR_STUDENT_COURSE sc ON s.STUDENT_NUMBER = sc.STUDENT_NUMBER
                                    LEFT JOIN FR_TRANSACTION t ON sc.COURSE_CODE = t.COURSE_CODE
                                    LEFT JOIN FR_TRANSACTION_STUDENT ts ON t.TRANSACTION_ID = ts.TRANSACTION_ID 
                                        AND ts.STUDENT_NUMBER = s.STUDENT_NUMBER
                                    WHERE sc.COURSE_CODE = :courseCode
                                    GROUP BY s.STUDENT_NUMBER, s.STUDENT_NAME, s.PROFILE_STATUS
                                    FETCH FIRST 5 ROWS ONLY";

                                using (var debugCmd = new Oracle.ManagedDataAccess.Client.OracleCommand(debugSql, conn))
                                {
                                    debugCmd.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("courseCode", courseCode));
                                    using (var debugReader = debugCmd.ExecuteReader())
                                    {
                                        Console.WriteLine("\nSample Student Data:");
                                        while (debugReader.Read())
                                        {
                                            Console.WriteLine($"Student: {debugReader["STUDENT_NUMBER"]}, " +
                                                            $"Name: {debugReader["STUDENT_NAME"]}, " +
                                                            $"Status: {debugReader["PROFILE_STATUS"]}, " +
                                                            $"Attended Classes: {debugReader["ATTENDANCE_COUNT"]}");
                                        }
                                    }
                                }

                                return Json(new { 
                                    success = true,
                                    totalStudents = totalStudents,
                                    debtors = debtors,
                                    attendanceRate = Math.Round(attendanceRate, 1)
                                });
                            }
                            else
                            {
                                Console.WriteLine($"No data found for course {courseCode}");
                                return Json(new { 
                                    success = true,
                                    totalStudents = 0,
                                    debtors = 0,
                                    attendanceRate = 0
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting course statistics: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return Json(new { error = "Failed to load course statistics." });
            }
        }

        [HttpGet]
        public JsonResult GetCourseDetails(string courseCode)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return Json(new { error = "Session expired. Please log in again." });
            }

            string userId = _configuration["OracleDBSettings:UserId"];
            string password = _configuration["OracleDBSettings:Password"];
            string dataSource = _configuration["OracleDBSettings:DataSource"];

            try
            {
                using (var conn = new Oracle.ManagedDataAccess.Client.OracleConnection(
                    $"User Id={userId};Password={password};Data Source={dataSource}"))
                {
                    conn.Open();

                    // Get student details including attendance
                    var students = new List<object>();
                    string studentSql = @"
                        SELECT 
                            s.STUDENT_NUMBER,
                            s.STUDENT_NAME,
                            s.PROFILE_STATUS as STATUS,
                            COUNT(CASE WHEN ts.ATTENDANCE = '1' THEN 1 END) as ATTENDED_CLASSES,
                            COUNT(DISTINCT t.TRANSACTION_ID) as TOTAL_CLASSES,
                            MAX(t.TRANSACTION_DATE) as LAST_ATTENDED
                        FROM FR_STUDENT s
                        JOIN FR_STUDENT_COURSE sc ON s.STUDENT_NUMBER = sc.STUDENT_NUMBER
                        LEFT JOIN FR_TRANSACTION t ON sc.COURSE_CODE = t.COURSE_CODE
                        LEFT JOIN FR_TRANSACTION_STUDENT ts ON t.TRANSACTION_ID = ts.TRANSACTION_ID 
                            AND ts.STUDENT_NUMBER = s.STUDENT_NUMBER
                        WHERE sc.COURSE_CODE = :courseCode
                        GROUP BY s.STUDENT_NUMBER, s.STUDENT_NAME, s.PROFILE_STATUS
                        ORDER BY s.STUDENT_NAME";

                    using (var cmd = new Oracle.ManagedDataAccess.Client.OracleCommand(studentSql, conn))
                    {
                        cmd.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("courseCode", courseCode));
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int attendedClasses = Convert.ToInt32(reader["ATTENDED_CLASSES"]);
                                int totalClasses = Convert.ToInt32(reader["TOTAL_CLASSES"]);
                                double attendanceRate = totalClasses > 0 ? 
                                    (attendedClasses * 100.0 / totalClasses) : 0;

                                students.Add(new
                                {
                                    studentNumber = reader["STUDENT_NUMBER"].ToString(),
                                    studentName = reader["STUDENT_NAME"].ToString(),
                                    status = reader["STATUS"].ToString(),
                                    attendanceRate = Math.Round(attendanceRate, 1),
                                    lastAttended = reader["LAST_ATTENDED"] != DBNull.Value ? 
                                        Convert.ToDateTime(reader["LAST_ATTENDED"]).ToString("yyyy-MM-dd") : null
                                });
                            }
                        }
                    }

                    // Get debtor statistics for the chart (last 10 classes)
                    string debtorSql = @"
                        SELECT 
                            t.TRANSACTION_DATE,
                            COUNT(DISTINCT CASE WHEN UPPER(s.PROFILE_STATUS) = 'DEBTOR' THEN s.STUDENT_NUMBER END) as DEBTOR_COUNT,
                            COUNT(DISTINCT s.STUDENT_NUMBER) as TOTAL_STUDENTS
                        FROM FR_TRANSACTION t
                        JOIN FR_STUDENT_COURSE sc ON t.COURSE_CODE = sc.COURSE_CODE
                        JOIN FR_STUDENT s ON sc.STUDENT_NUMBER = s.STUDENT_NUMBER
                        WHERE t.COURSE_CODE = :courseCode
                        GROUP BY t.TRANSACTION_DATE
                        ORDER BY t.TRANSACTION_DATE DESC
                        FETCH FIRST 10 ROWS ONLY";

                    var debtorData = new { labels = new List<string>(), values = new List<int>() };
                    using (var cmd = new Oracle.ManagedDataAccess.Client.OracleCommand(debtorSql, conn))
                    {
                        cmd.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("courseCode", courseCode));
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var date = Convert.ToDateTime(reader["TRANSACTION_DATE"]).ToString("MM/dd");
                                var debtorCount = Convert.ToInt32(reader["DEBTOR_COUNT"]);
                                Console.WriteLine($"Date: {date}, Debtor Count: {debtorCount}"); // Debug log
                                ((List<string>)debtorData.labels).Add(date);
                                ((List<int>)debtorData.values).Add(debtorCount);
                            }
                        }
                    }

                    // Reverse the lists to show oldest to newest
                    ((List<string>)debtorData.labels).Reverse();
                    ((List<int>)debtorData.values).Reverse();

                    Console.WriteLine("Final debtor data:"); // Debug log
                    for (int i = 0; i < ((List<string>)debtorData.labels).Count; i++)
                    {
                        Console.WriteLine($"Date: {((List<string>)debtorData.labels)[i]}, Count: {((List<int>)debtorData.values)[i]}");
                    }

                    return Json(new { 
                        success = true,
                        students = students,
                        debtorData = debtorData
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting course details: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return Json(new { error = "Failed to load course details. Please try again." });
            }
        }
    }
} 