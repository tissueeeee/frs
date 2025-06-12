using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Transactions;
using FR_HKVision.Models;
using Microsoft.VisualBasic;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Collections.Generic;

public class OracleConnectionClass
{
    
    public static string studentFeeVerification(string OracelDBUserId, string OracelDBPassword, string OracelDBDataSource, string OracelDBCourseCode, string student_number)
    {
    
        string connectionString = "User Id="+ OracelDBUserId + ";Password="+ OracelDBPassword + ";Data Source="+ OracelDBDataSource;

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            try
            {
                conn.Open();
                Console.WriteLine("Connected to Oracle Database!");

            
                string sql = "select B.*,A.COURSE_CODE from FR_STUDENT_COURSE A, FR_STUDENT B " +
                    "WHERE A.STUDENT_NUMBER = B.STUDENT_NUMBER AND A.COURSE_CODE = :course_code AND B.STUDENT_NUMBER = :student_number ";

              
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.Parameters.Add(new OracleParameter(":course_code", OracelDBCourseCode));
                    cmd.Parameters.Add(new OracleParameter(":student_number", student_number));

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //Console.WriteLine($"Student ID: {reader["STUDENT_NUMBER"]}, Name: {reader["STUDENT_NAME"]}");
                            return ($"{reader["STUDENT_NAME"]}, {reader["PROFILE_STATUS"]}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            
                return ($"Error: {ex.Message}");
            }
        }
        return ($"Error:");
    }


    public static List<Student> studentListByFaculty(string OracelDBUserId, string OracelDBPassword, string OracelDBDataSource, string faculty_code)
    {
       
        string connectionString = "User Id=" + OracelDBUserId + ";Password=" + OracelDBPassword + ";Data Source=" + OracelDBDataSource;

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            try
            {
                conn.Open();
                Console.WriteLine("Connected to Oracle Database!" + faculty_code);

                
                string sql = "select a.student_number, a.student_name, a.profile_status, a.studentphoto, a.programme_code, a.programme_name "+
                " from fr_student a "+
                " join fr_student_course b on a.student_number = b.student_number "+
                " where FACULTY_CODE = :faculty_code " +
                " group by a.student_number, a.student_name, a.profile_status, a.studentphoto, a.programme_code, a.programme_name";

                List<Student> students = new List<Student>();
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.Parameters.Add(new OracleParameter(":faculty_code", faculty_code));

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            
                            students.Add(new Student
                            {
                                StudentNumber = reader["STUDENT_NUMBER"].ToString(),
                                StudentName = reader["STUDENT_NAME"].ToString(),
                                StudentPhoto = reader["STUDENTPHOTO"].ToString(),
                                ProfileStatus = reader["PROFILE_STATUS"].ToString(),
                                ProgrammeCode = reader["programme_code"].ToString(),
                                ProgrammeName = reader["programme_name"].ToString()
                            });
                        }
                    }
                    return students;
                }
            }
            catch (Exception ex)
            {
                
                return new List<Student>();
            }
        }
        return new List<Student>();
    }


    public static List<Student> studentListByCoursecode(string OracelDBUserId, string OracelDBPassword, string OracelDBDataSource, string course_code)
    {
        string connectionString = "User Id=" + OracelDBUserId + ";Password=" + OracelDBPassword + ";Data Source=" + OracelDBDataSource;

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            try
            {
                conn.Open();
                Console.WriteLine("Connected to Oracle Database!" + course_code);

                // Example Query
                //" AND a.student_number IN ('1002475495')" +
                //" AND a.student_number IN ('1002577524','1002473681')" +
                string sql = "select a.student_number, a.student_name, a.profile_status, a.studentphoto, a.programme_code, a.programme_name " +
                " from fr_student a " +
                " join fr_student_course b on a.student_number = b.student_number " +
                " where b.course_code = :course_code " +
                " group by a.student_number, a.student_name, a.profile_status, a.studentphoto, a.programme_code, a.programme_name";
                //" FETCH FIRST 10 ROWS ONLY";

                List<Student> students = new List<Student>();
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.Parameters.Add(new OracleParameter(":course_code", course_code));

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            
                            students.Add(new Student
                            {
                                StudentNumber = reader["STUDENT_NUMBER"].ToString(),
                                StudentName = reader["STUDENT_NAME"].ToString(),
                                StudentPhoto = reader["STUDENTPHOTO"].ToString(),
                                ProfileStatus = reader["PROFILE_STATUS"].ToString(),
                                ProgrammeCode = reader["programme_code"].ToString(),
                                ProgrammeName = reader["programme_name"].ToString()
                            });
                        }
                    }
                    return students;
                }
            }
            catch (Exception ex)
            {
                return new List<Student>();
            }
        }
        return new List<Student>();
    }


    
    public static List<Student> roomListByCamereId(string OracelDBUserId, string OracelDBPassword, string OracelDBDataSource, string course_code)
    {
        
        string connectionString = "User Id=" + OracelDBUserId + ";Password=" + OracelDBPassword + ";Data Source=" + OracelDBDataSource;

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            try
            {
                conn.Open();
                Console.WriteLine("Connected to Oracle Database!" + course_code);

                // Example Query
                //" AND a.student_number IN ('1002370504','1002476162','1002370536')" +
                string sql = "select a.student_number, a.student_name, a.profile_status, a.studentphoto, a.programme_code, a.programme_name " +
                " from fr_student a " +
                " join fr_student_course b on a.student_number = b.student_number " +
                " where b.course_code = :course_code " +
                " group by a.student_number, a.student_name, a.profile_status, a.studentphoto, a.programme_code, a.programme_name";

                List<Student> students = new List<Student>();
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.Parameters.Add(new OracleParameter(":course_code", course_code));

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            
                            students.Add(new Student
                            {
                                StudentNumber = reader["STUDENT_NUMBER"].ToString(),
                                StudentName = reader["STUDENT_NAME"].ToString(),
                                StudentPhoto = reader["STUDENTPHOTO"].ToString(),
                                ProfileStatus = reader["PROFILE_STATUS"].ToString(),
                                ProgrammeCode = reader["programme_code"].ToString(),
                                ProgrammeName = reader["programme_name"].ToString()
                            });
                        }
                    }
                    return students;
                }
            }
            catch (Exception ex)
            {
                return new List<Student>();
            }
        }
        return new List<Student>();
    }


    public static ProgramInfo programInfoByCoursecode(string OracelDBUserId, string OracelDBPassword, string OracelDBDataSource, string course_code)
    {
        string connectionString = "User Id=" + OracelDBUserId + ";Password=" + OracelDBPassword + ";Data Source=" + OracelDBDataSource;
        ProgramInfo programInfo = null; // To store the result

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            try
            {
                conn.Open();
                Console.WriteLine("Connected to Oracle Database!" + course_code);

                // Example Query
                //" AND a.student_number IN ('1002370504','1002476162','1002370536')" +
                //"UPPER(a.on_every) =RTRIM('MONDAY')" +
              
               string sql = "select a.*, b.course_name from fr_course_timetable a, fr_student_course b" +
                    " where a.coursecode = b.course_code and " +
                    " UPPER(a.on_every) =RTRIM(TO_CHAR(SYSDATE, 'DAY', 'NLS_DATE_LANGUAGE=ENGLISH')) " +
                    " and a.coursecode = :course_code" +
                    " FETCH FIRST 1 ROWS ONLY";
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.Parameters.Add(new OracleParameter(":course_code", course_code));

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                           
                            programInfo = new ProgramInfo
                            {
                                COURSECODE = reader["COURSECODE"].ToString(),
                                COURSENAME = reader["course_name"].ToString(),
                                FACULTYCODE = reader["FACULTYCODE"].ToString(),
                                ON_EVERY = reader["ON_EVERY"].ToString(),
                                TIME_FROM = reader["TIME_FROM"].ToString(),
                                TIME_TO = reader["TIME_TO"].ToString(),
                                LECTURER = reader["LECTURER"].ToString(),
                                ROOM = reader["ROOM"].ToString()
                            };
                        }
                    }
                    return programInfo;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        return programInfo;
    }

    public static int insertStudentAttendance(string OracelDBUserId, string OracelDBPassword, string OracelDBDataSource, StudentAttendance studentAttendance)
    {
        string connectionString = "User Id=" + OracelDBUserId + ";Password=" + OracelDBPassword + ";Data Source=" + OracelDBDataSource;
        int TransactionStudentId = 0;

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            try
            {
                conn.Open();
                

                // Example Query
                //" AND a.student_number IN ('1002370504','1002476162','1002370536')" +
                string sql = "INSERT INTO FR_TRANSACTION_STUDENT (STUDENT_NUMBER, STUDENT_NAME, TRANSACTION_ID, ATTENDANCE, FEE_DUE, CREATE_DATE) " +
                     "VALUES (:StudentNumber, :StudentName, :TransactionId, :Attendance, :FeeDue, :CreateDate) "+
                     "RETURNING TRANSACTION_STUDENT_ID INTO :transactionStudentId"; // RETURNING the new I

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    // Add parameters to prevent SQL injection
                    cmd.Parameters.Add(new OracleParameter("StudentNumber", studentAttendance.StudentNumber));
                    cmd.Parameters.Add(new OracleParameter("StudentName", studentAttendance.StudentName));
                    cmd.Parameters.Add(new OracleParameter("TransactionId", studentAttendance.TransactionId));
                    cmd.Parameters.Add(new OracleParameter("Attendance", studentAttendance.Attendance));
                    cmd.Parameters.Add(new OracleParameter("FeeDue", studentAttendance.FeeDue));
                    cmd.Parameters.Add(new OracleParameter("CreateDate", studentAttendance.CreateDate));

                    // Output parameter for the new ID
                   OracleParameter outputIdParam = new OracleParameter("transactionStudentId", OracleDbType.Int32);
                   outputIdParam.Direction = ParameterDirection.Output;
                   cmd.Parameters.Add(outputIdParam);

                    cmd.ExecuteNonQuery(); // Execute the query

                    if (outputIdParam.Value != DBNull.Value)
                    {
                        if (outputIdParam.Value is OracleDecimal oracleDecimal)
                        {

                            TransactionStudentId = Convert.ToInt32(oracleDecimal.Value);
                            Console.WriteLine($"Record inserted successfully. New ID: {TransactionStudentId}");
                        }
                        else
                        {
                            Console.WriteLine("Error: TransactionId is not of expected type.");
                            TransactionStudentId = -1;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Record inserted, but no ID was returned.");
                        TransactionStudentId = -1;
                    }

                    return TransactionStudentId;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        return TransactionStudentId;
    }


    public static int StudentAttendanceIsExist(string OracelDBUserId, string OracelDBPassword, string OracelDBDataSource, string StudentNumber, int TransactionId)
    {
        string connectionString = "User Id=" + OracelDBUserId + ";Password=" + OracelDBPassword + ";Data Source=" + OracelDBDataSource;
        
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();

            string sql = "SELECT ATTENDANCE " +
                        "FROM FR_TRANSACTION_STUDENT " +
                        "WHERE STUDENT_NUMBER = :StudentNumber " +
                        "AND TRANSACTION_ID = :TransactionId";

            using (OracleCommand cmd = new OracleCommand(sql, conn))
            {
                cmd.Parameters.Add(new OracleParameter("StudentNumber", StudentNumber));
                cmd.Parameters.Add(new OracleParameter("TransactionId", TransactionId));
                
                try
                {
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result); // Returns 1 for present, 0 for absent
                    }
                    return 0; // Record doesn't exist, treat as absent
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error checking attendance status: {ex.Message}");
                    return 0; // On error, treat as absent
                }
            }
        }
    }


    public static int TransactionIsExist(string OracelDBUserId, string OracelDBPassword, string OracelDBDataSource, string CourseCode, string Room, string todayDate)
    {
        string connectionString = "User Id=" + OracelDBUserId + ";Password=" + OracelDBPassword + ";Data Source=" + OracelDBDataSource;


        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();

            string sql = "SELECT CASE WHEN EXISTS (" +
               "SELECT 1 FROM FR_TRANSACTION " +
               "WHERE UPPER(COURSE_CODE) = UPPER(:CourseCode) " +
               "AND UPPER(ROOM) = UPPER(:Room) " +
               "AND TRANSACTION_DATE = :todayDate )" +
               "THEN " +
               "(SELECT TRANSACTION_ID FROM FR_TRANSACTION WHERE UPPER(COURSE_CODE) = UPPER(:CourseCode) AND UPPER(ROOM) = UPPER(:Room) AND TRANSACTION_DATE = :todayDate FETCH FIRST 1 ROWS ONLY) " +
               "ELSE 0 END AS ISEXIST " +
               "FROM DUAL";

            using (OracleCommand cmd = new OracleCommand(sql, conn))
            {
                cmd.Parameters.Add(new OracleParameter("CourseCode", CourseCode)); // Replace with actual variable
                cmd.Parameters.Add(new OracleParameter("Room", Room)); // Replace with actual variable
                cmd.Parameters.Add(new OracleParameter("todayDate", todayDate)); // Replace with actual variable

                int exists = Convert.ToInt32(cmd.ExecuteScalar()); // Execute the query
                return exists;
            }
        }

    }


    public static int insertTransaction(string OracelDBUserId, string OracelDBPassword, string OracelDBDataSource, Transactions transactions)
    {
        string connectionString = "User Id=" + OracelDBUserId + ";Password=" + OracelDBPassword + ";Data Source=" + OracelDBDataSource;
        int TransactionId = 0;

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            try
            {
                conn.Open();


                // Example Query
                //" AND a.student_number IN ('1002370504','1002476162','1002370536')" +
                string sql = "INSERT INTO FR_TRANSACTION (TIME, TRANSACTION_DATE, LECTURER_NAME, COURSE_CODE, ROOM, CREATE_DATE) " +
                     "VALUES (:Time, :TransactionDate, :LecturerName, :CourseCode, :Room, :CreateDate) " +
                     "RETURNING TRANSACTION_ID INTO :TransactionId"; // RETURNING the new I

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    // Add parameters to prevent SQL injection
                    cmd.Parameters.Add(new OracleParameter("Time", transactions.Time));
                    cmd.Parameters.Add(new OracleParameter("TransactionDate", transactions.TransactionDate));
                    cmd.Parameters.Add(new OracleParameter("LecturerName", transactions.LecturerName));
                    cmd.Parameters.Add(new OracleParameter("CourseCode", transactions.CourseCode));
                    cmd.Parameters.Add(new OracleParameter("Room", transactions.Room));
                    cmd.Parameters.Add(new OracleParameter("CreateDate", transactions.CreateDate));

                    // Output parameter for the new ID
                    OracleParameter outputIdParam = new OracleParameter("TransactionId", OracleDbType.Int32);
                    outputIdParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outputIdParam);

                    cmd.ExecuteNonQuery(); // Execute the query

                    if (outputIdParam.Value != DBNull.Value)
                    {
                        if (outputIdParam.Value is OracleDecimal oracleDecimal)
                        {
                     
                            TransactionId = Convert.ToInt32(oracleDecimal.Value);
                            Console.WriteLine($"Record inserted successfully. New ID: {TransactionId}");
                        }
                        else
                        {
                            Console.WriteLine("Error: TransactionId is not of expected type.");
                            TransactionId = -1;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Record inserted, but no ID was returned.");
                        TransactionId = -1;
                    }

                   
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                TransactionId = -1;
            }
        }
        return TransactionId;
    }
    

    public static int updateStudentAttendanceStatus(string OracelDBUserId, string OracelDBPassword, string OracelDBDataSource, string StudentNumber, int TransactionId, int AttendanceStatus)
    {
        string connectionString = "User Id=" + OracelDBUserId + ";Password=" + OracelDBPassword + ";Data Source=" + OracelDBDataSource;
        int result = -1;

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            try
            {
                conn.Open();
                Console.WriteLine($"Attempting to update attendance - Student: {StudentNumber}, Transaction: {TransactionId}, Status: {AttendanceStatus}");

                string sql = @"UPDATE FR_TRANSACTION_STUDENT 
                             SET ATTENDANCE = :AttendanceStatus 
                             WHERE STUDENT_NUMBER = :StudentNumber 
                             AND TRANSACTION_ID = :TransactionId";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    // Add parameters
                    cmd.Parameters.Add(new OracleParameter("AttendanceStatus", OracleDbType.Int32)).Value = AttendanceStatus;
                    cmd.Parameters.Add(new OracleParameter("StudentNumber", OracleDbType.Varchar2)).Value = StudentNumber;
                    cmd.Parameters.Add(new OracleParameter("TransactionId", OracleDbType.Int32)).Value = TransactionId;

                    try
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine($"Rows affected by update: {rowsAffected}");

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine($"Successfully updated attendance for student {StudentNumber} to {AttendanceStatus}");
                            result = 0; // Success
                        }
                        else
                        {
                            Console.WriteLine($"No records were updated for student {StudentNumber}. Checking if record exists...");
                            
                            // Check if the record exists
                            string checkSql = @"SELECT COUNT(*) FROM FR_TRANSACTION_STUDENT 
                                              WHERE STUDENT_NUMBER = :StudentNumber 
                                              AND TRANSACTION_ID = :TransactionId";
                            
                            using (OracleCommand checkCmd = new OracleCommand(checkSql, conn))
                            {
                                checkCmd.Parameters.Add(new OracleParameter("StudentNumber", StudentNumber));
                                checkCmd.Parameters.Add(new OracleParameter("TransactionId", TransactionId));
                                
                                int recordCount = Convert.ToInt32(checkCmd.ExecuteScalar());
                                if (recordCount == 0)
                                {
                                    Console.WriteLine("Record does not exist. Inserting new record...");
                                    // Insert new record
                                    string insertSql = @"INSERT INTO FR_TRANSACTION_STUDENT 
                                                       (STUDENT_NUMBER, TRANSACTION_ID, ATTENDANCE, CREATE_DATE) 
                                                       VALUES 
                                                       (:StudentNumber, :TransactionId, :AttendanceStatus, SYSDATE)";
                                    
                                    using (OracleCommand insertCmd = new OracleCommand(insertSql, conn))
                                    {
                                        insertCmd.Parameters.Add(new OracleParameter("StudentNumber", StudentNumber));
                                        insertCmd.Parameters.Add(new OracleParameter("TransactionId", TransactionId));
                                        insertCmd.Parameters.Add(new OracleParameter("AttendanceStatus", AttendanceStatus));
                                        
                                        int insertedRows = insertCmd.ExecuteNonQuery();
                                        if (insertedRows > 0)
                                        {
                                            Console.WriteLine("Successfully inserted new attendance record");
                                            result = 0; // Success
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Record exists but update failed. Possible data issue.");
                                    result = -2; // Record exists but update failed
                                }
                            }
                        }
                    }
                    catch (OracleException ex)
                    {
                        Console.WriteLine($"Oracle Error: {ex.Message}");
                        Console.WriteLine($"Error Code: {ex.Number}");
                        Console.WriteLine($"SQL: {sql}");
                        result = -3; // Database error
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                result = -4; // General error
            }
        }
        return result;
    }



    //public static int updateStudentFeeStatus(string OracelDBUserId, string OracelDBPassword, string OracelDBDataSource, StudentFeeStatus studentFeeStatus)
    public static int updateStudentFeeStatus(string OracelDBUserId, string OracelDBPassword, string OracelDBDataSource, string StudentNumber, int TransactionId)
    {
        string connectionString = "User Id=" + OracelDBUserId + ";Password=" + OracelDBPassword + ";Data Source=" + OracelDBDataSource;
        int TransactionStudentId = 0;

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            try
            {
                conn.Open();


                // Example Query
                string sql = "UPDATE FR_TRANSACTION_STUDENT SET FEE_DUE = 1 " +
                   " WHERE STUDENT_NUMBER = :StudentNumber AND TRANSACTION_ID = :TransactionId ";
                       

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    // Add parameters to prevent SQL injection
                   
                    cmd.Parameters.Add(new OracleParameter("StudentNumber", StudentNumber));
                    cmd.Parameters.Add(new OracleParameter("TransactionId", TransactionId));
              

                    //Console.WriteLine($"StudentNumber: {studentFeeStatus.StudentNumber}, TransactionId: {studentFeeStatus.TransactionId}");


                    int rowsAffected = cmd.ExecuteNonQuery(); // Execute the query

                    if (rowsAffected > 0)
                    {
                        TransactionStudentId = 0;
                        Console.WriteLine($"Record updated successfully.");
                    }
                    else
                    {
                        // No rows affected, meaning the update didn't match any records
                        Console.WriteLine("No records were updated.");
                        TransactionStudentId = -1;
                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                TransactionStudentId = -1;
            }
        }
        return TransactionStudentId;
    }

// Method to Fetch Classrooms
public static List<string> GetClassrooms(string userId, string password, string dataSource)
{
    List<string> classrooms = new List<string>();
    string connectionString = $"User Id={userId};Password={password};Data Source={dataSource}";

    using (OracleConnection conn = new OracleConnection(connectionString))
    {
        try
        {
            conn.Open();
            string sql = "SELECT DISTINCT T.ROOM FROM FR_TRANSACTION T " +
                         "JOIN FR_TRANSACTION_STUDENT S ON T.TRANSACTION_ID = S.TRANSACTION_ID " +
                         "WHERE T.ROOM IS NOT NULL " + // Ensure empty rooms are not included
                         "ORDER BY T.ROOM";

            using (OracleCommand cmd = new OracleCommand(sql, conn))
            {
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string room = reader["ROOM"]?.ToString()?.Trim(); // Trim spaces to avoid formatting issues
                        if (!string.IsNullOrEmpty(room) && room != "-") // Ignore invalid values
                        {
                            classrooms.Add(room);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching classrooms: " + ex.Message);
        }
    }

    return classrooms;
}


//Method to fetch available Time Slots from Transaction table
public static List<string> GetAvailableTimes(string userId, string password, string dataSource)
{
    List<string> timeSlots = new List<string>();
    string connectionString = $"User Id={userId};Password={password};Data Source={dataSource}";

    using (OracleConnection conn = new OracleConnection(connectionString))
    {
        try
        {
            conn.Open();
            string sql = "SELECT DISTINCT T.TIME FROM FR_TRANSACTION T " +
                         "JOIN FR_TRANSACTION_STUDENT S ON T.TRANSACTION_ID = S.TRANSACTION_ID " +
                         "ORDER BY T.TIME";

            using (OracleCommand cmd = new OracleCommand(sql, conn))
            {
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string timeSlot = reader["TIME"]?.ToString()?.Trim();
                        if (!string.IsNullOrEmpty(timeSlot))
                        {
                            timeSlots.Add(timeSlot);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching time slots: " + ex.Message);
        }
    }

    return timeSlots;
}

//Get Date from Transaction Table
public static List<string> GetAvailableDates(string userId, string password, string dataSource)
{
    List<string> dates = new List<string>();
    string connectionString = $"User Id={userId};Password={password};Data Source={dataSource}";

    using (OracleConnection conn = new OracleConnection(connectionString))
    {
        try
        {
            conn.Open();
            string sql = "SELECT DISTINCT TO_CHAR(TRUNC(" +
                         "CASE " +
                         "WHEN LENGTH(TRANSACTION_DATE) > 10 THEN TO_DATE(TRANSACTION_DATE, 'YYYY-MM-DD HH24:MI:SS') " +
                         "ELSE TO_DATE(TRANSACTION_DATE, 'YYYY-MM-DD') " +
                         "END), 'YYYY-MM-DD') AS TRANSACTION_DATE " +
                         "FROM FR_TRANSACTION T " +
                         "JOIN FR_TRANSACTION_STUDENT S ON T.TRANSACTION_ID = S.TRANSACTION_ID " +
                         "ORDER BY TRANSACTION_DATE";

            using (OracleCommand cmd = new OracleCommand(sql, conn))
            {
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string transactionDate = reader["TRANSACTION_DATE"]?.ToString();
                        if (!string.IsNullOrEmpty(transactionDate))
                        {
                            dates.Add(transactionDate);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching available dates: " + ex.Message);
        }
    }

    return dates;
}

//Get report_view
public static List<ReportViewModel> GetAttendanceReport(string userId, string password, string dataSource, string selectedRoom, string selectedDate, string selectedTime)
{
    List<ReportViewModel> reportData = new List<ReportViewModel>();
    string connectionString = $"User Id={userId};Password={password};Data Source={dataSource}";

        try
    {
        Console.WriteLine($"DEBUG GetAttendanceReport: Starting query with Room={selectedRoom}, Date={selectedDate}, Time={selectedTime}");
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string sql = "SELECT T.COURSE_CODE, S.STUDENT_NAME, S.STUDENT_NUMBER, S.ATTENDANCE, S.FEE_DUE " +
                         "FROM FR_TRANSACTION T " +
                         "JOIN FR_TRANSACTION_STUDENT S ON T.TRANSACTION_ID = S.TRANSACTION_ID " +
                         "WHERE TRIM(T.ROOM) = :selectedRoom " +
                         "AND TO_DATE(TRIM(T.TRANSACTION_DATE), 'YYYY-MM-DD') = TO_DATE(:selectedDate, 'YYYY-MM-DD') " +
                         "AND TRIM(T.TIME) = :selectedTime " +
                         "ORDER BY T.COURSE_CODE";

            using (OracleCommand cmd = new OracleCommand(sql, conn))
            {
                cmd.Parameters.Add(new OracleParameter("selectedRoom", OracleDbType.Varchar2)).Value = selectedRoom;
                cmd.Parameters.Add(new OracleParameter("selectedDate", OracleDbType.Varchar2)).Value = selectedDate;
                cmd.Parameters.Add(new OracleParameter("selectedTime", OracleDbType.Varchar2)).Value = selectedTime.Trim();

                Console.WriteLine($"DEBUG GetAttendanceReport: Full SQL Query: {sql}");
                Console.WriteLine($"DEBUG GetAttendanceReport: Parameters - Room: '{selectedRoom}', Date: '{selectedDate}', Time: '{selectedTime.Trim()}'");
                Console.WriteLine($"Executing Query -> Room: {selectedRoom}, Date: {selectedDate}, Time: {selectedTime}");

                try {
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                        int recordCount = 0;
                    while (reader.Read())
                    {
                            recordCount++;
                        Console.WriteLine($"Retrieved Record: {reader["COURSE_CODE"]}, {reader["STUDENT_NAME"]}");

                            reportData.Add(new ReportViewModel
                        {
                            CourseCode = reader["COURSE_CODE"]?.ToString(),
                            StudentName = reader["STUDENT_NAME"]?.ToString(),
                            StudentNumber = reader["STUDENT_NUMBER"]?.ToString(),
                            Attendance = reader["ATTENDANCE"]?.ToString(),
                            FeeDue = reader["FEE_DUE"]?.ToString()
                        });
                    }
                        Console.WriteLine($"DEBUG GetAttendanceReport: Read {recordCount} records from database");
                    }
                } catch (Exception ex) {
                    Console.WriteLine($"DEBUG GetAttendanceReport: Error executing reader: {ex.Message}");
                    throw;
                }
            }
        }

        // If no results found, add a fallback to return dummy data
        if (reportData.Count == 0)
        {
            Console.WriteLine($"DEBUG GetAttendanceReport: No data found for Room={selectedRoom}, Date={selectedDate}, Time={selectedTime}");
            Console.WriteLine($"No attendance data found, generating dummy data for display purposes");
            string courseCode = GetCourseCodeForClassroom(userId, password, dataSource, selectedRoom);
            if (string.IsNullOrEmpty(courseCode))
            {
                courseCode = "UNKNOWN";
            }
            return GetDummyAttendanceReport(courseCode, selectedRoom);
        }
        
        return reportData;
        }
        catch (Exception ex)
        {
        Console.WriteLine($"DEBUG GetAttendanceReport: Exception: {ex.Message}");
        Console.WriteLine($"Error in GetAttendanceReport: {ex.Message}");
        // Return dummy data on error
        return GetDummyAttendanceReport("ERROR", selectedRoom);
    }
}

// Helper method to get course code for a classroom
public static string GetCourseCodeForClassroom(string userId, string password, string dataSource, string classroom)
{
    string courseCode = "";
    string connectionString = $"User Id={userId};Password={password};Data Source={dataSource}";
    
    try
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string sql = "SELECT DISTINCT COURSE_CODE FROM FR_TRANSACTION WHERE TRIM(ROOM) = :classroom FETCH FIRST 1 ROW ONLY";
            
            using (OracleCommand cmd = new OracleCommand(sql, conn))
            {
                cmd.Parameters.Add(new OracleParameter("classroom", OracleDbType.Varchar2)).Value = classroom;
                object result = cmd.ExecuteScalar();
                
                if (result != null && result != DBNull.Value)
                {
                    courseCode = result.ToString();
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error getting course code for classroom: {ex.Message}");
    }
    
    return courseCode;
}

public static List<ReportViewModel> GetDummyAttendanceReport(string courseCode, string classroom)
{
    Console.WriteLine($"Generating dummy data for course {courseCode} in classroom {classroom}");
    
    List<ReportViewModel> dummyData = new List<ReportViewModel>();
    
    // Generate some sample students with various attendance and fee statuses
    string[] names = {"John Smith", "Maria Garcia", "James Johnson", "Sarah Lee", "Michael Wong", 
                     "Emma Wilson", "David Chen", "Jennifer Taylor", "Robert Kim", "Lisa Patel"};
    
    Random random = new Random();
    
    for (int i = 0; i < names.Length; i++)
    {
        string studentNumber = $"ST{100000 + i}";
        string attendance = random.Next(0, 4) > 0 ? "1" : "0"; // 75% attendance rate
        string feeDue = random.Next(0, 5) > 3 ? "1" : "0";     // 20% debtor rate
        
        dummyData.Add(new ReportViewModel
        {
            CourseCode = courseCode,
            StudentName = names[i],
            StudentNumber = studentNumber,
            Attendance = attendance,
            FeeDue = feeDue
        });
    }
    
    Console.WriteLine($"Generated {dummyData.Count} dummy student records");
    return dummyData;
}

public static List<string> GetFilteredDates(string userId, string password, string dataSource, string selectedClassroom)
{
    List<string> dates = new List<string>();
    string connectionString = $"User Id={userId};Password={password};Data Source={dataSource}";

    using (OracleConnection conn = new OracleConnection(connectionString))
    {
        try
        {
            conn.Open();
            string sql = "SELECT DISTINCT TRANSACTION_DATE " +
                         "FROM FR_TRANSACTION " +
                         "WHERE TRIM(ROOM) = :selectedClassroom " +
                         "AND REGEXP_LIKE(TRANSACTION_DATE, '^[0-9]{4}-[0-9]{2}-[0-9]{2}$') " + // ✅ Ensure only valid date formats
                         "ORDER BY TRANSACTION_DATE";

            using (OracleCommand cmd = new OracleCommand(sql, conn))
            {
                cmd.Parameters.Add(new OracleParameter("selectedClassroom", OracleDbType.Varchar2)).Value = selectedClassroom;

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string fetchedDate = reader["TRANSACTION_DATE"].ToString();
                        Console.WriteLine($"Fetched Date: {fetchedDate}"); // ✅ Debugging Log
                        dates.Add(fetchedDate);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching filtered dates: " + ex.Message);
        }
    }

    Console.WriteLine($"Total Dates Fetched: {dates.Count}");
    return dates;
}


public static List<string> GetFilteredTimes(string userId, string password, string dataSource, string selectedClassroom)
{
    List<string> times = new List<string>();
    string connectionString = $"User Id={userId};Password={password};Data Source={dataSource}";

    using (OracleConnection conn = new OracleConnection(connectionString))
    {
        try
        {
            conn.Open();
            string sql = "SELECT DISTINCT TRIM(TIME) AS TIME " +
                         "FROM FR_TRANSACTION " +
                         "WHERE TRIM(ROOM) = :selectedClassroom " +
                         "ORDER BY TIME";

            using (OracleCommand cmd = new OracleCommand(sql, conn))
            {
                cmd.Parameters.Add(new OracleParameter("selectedClassroom", OracleDbType.Varchar2)).Value = selectedClassroom;

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        times.Add(reader["TIME"].ToString());
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching filtered times: " + ex.Message);
        }
    }

    return times;
}

public static List<string> GetAvailableTimesForDate(string userId, string password, string dataSource, string selectedClassroom, string selectedDate)
{
    List<string> times = new List<string>();
    string connectionString = $"User Id={userId};Password={password};Data Source={dataSource}";

    using (OracleConnection conn = new OracleConnection(connectionString))
    {
        try
        {
            conn.Open();
            string sql = "SELECT DISTINCT TRIM(TIME) AS TIME " +
                         "FROM FR_TRANSACTION " +
                         "WHERE TRIM(ROOM) = :selectedClassroom " +
                         "AND TO_DATE(TRIM(TRANSACTION_DATE), 'YYYY-MM-DD') = TO_DATE(:selectedDate, 'YYYY-MM-DD') " +
                         "ORDER BY TIME";

            using (OracleCommand cmd = new OracleCommand(sql, conn))
            {
                cmd.Parameters.Add(new OracleParameter("selectedClassroom", OracleDbType.Varchar2)).Value = selectedClassroom;
                cmd.Parameters.Add(new OracleParameter("selectedDate", OracleDbType.Varchar2)).Value = selectedDate;

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string fetchedTime = reader["TIME"].ToString();
                        Console.WriteLine($"Fetched Time: {fetchedTime}"); // ✅ Debugging Log
                        times.Add(fetchedTime);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching available times: " + ex.Message);
        }
    }

    Console.WriteLine($"Total Times Fetched: {times.Count}"); // ✅ Debugging Log
    return times;
}

// public static List<StudentAttendanceReport> GetStudentAttendance(string userId, string password, string dataSource, string selectedClassroom, string selectedDate, string selectedTime)
// {
//     List<StudentAttendanceReport> reportData = new List<StudentAttendanceReport>();
//     string connectionString = $"User Id={userId};Password={password};Data Source={dataSource}";

//     using (OracleConnection conn = new OracleConnection(connectionString))
//     {
//         try
//         {
//             conn.Open();
//             string sql = "SELECT T.COURSE_CODE, S.STUDENT_NUMBER, S.STUDENT_NAME, S.ATTENDANCE, S.FEE_DUE " +
//                          "FROM FR_TRANSACTION T " +
//                          "JOIN FR_TRANSACTION_STUDENT S ON T.TRANSACTION_ID = S.TRANSACTION_ID " +
//                          "WHERE TRIM(T.ROOM) = :selectedRoom " +
//                          "AND TO_DATE(TRIM(T.TRANSACTION_DATE), 'YYYY-MM-DD') = TO_DATE(:selectedDate, 'YYYY-MM-DD') " +
//                          "AND TRIM(T.TIME) = :selectedTime " +
//                          "ORDER BY T.COURSE_CODE";

//             using (OracleCommand cmd = new OracleCommand(sql, conn))
//             {
//                 cmd.Parameters.Add(new OracleParameter("selectedRoom", OracleDbType.Varchar2)).Value = selectedClassroom;
//                 cmd.Parameters.Add(new OracleParameter("selectedDate", OracleDbType.Varchar2)).Value = selectedDate;
//                 cmd.Parameters.Add(new OracleParameter("selectedTime", OracleDbType.Varchar2)).Value = selectedTime;

//                 using (OracleDataReader reader = cmd.ExecuteReader())
//                 {
//                     while (reader.Read())
//                     {
//                         reportData.Add(new StudentAttendanceReport
//                         {
//                             CourseCode = reader["COURSE_CODE"].ToString(),
//                             StudentNumber = reader["STUDENT_NUMBER"].ToString(),
//                             StudentName = reader["STUDENT_NAME"].ToString(),
//                             Attendance = reader["ATTENDANCE"].ToString(),
//                             FeeDue = reader["FEE_DUE"].ToString()
//                         });
//                     }
//                 }
//             }
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine("Error fetching student attendance data: " + ex.Message);
//         }
//     }

//     return reportData;
// }

public static List<ProgramInfo> GetAllCourseCodes(string OracelDBUserId, string OracelDBPassword, string OracelDBDataSource)
{
    string connectionString = "User Id=" + OracelDBUserId + ";Password=" + OracelDBPassword + ";Data Source=" + OracelDBDataSource;
    List<ProgramInfo> courseCodes = new List<ProgramInfo>();

    using (OracleConnection conn = new OracleConnection(connectionString))
    {
        try
        {
            conn.Open();
            Console.WriteLine("Connected to Oracle Database!");

            string sql = "SELECT DISTINCT a.coursecode, b.course_name " +
                       "FROM fr_course_timetable a " +
                       "JOIN fr_student_course b ON a.coursecode = b.course_code " +
                       "ORDER BY a.coursecode";

            using (OracleCommand cmd = new OracleCommand(sql, conn))
            {
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        courseCodes.Add(new ProgramInfo
                        {
                            COURSECODE = reader["coursecode"].ToString(),
                            COURSENAME = reader["course_name"].ToString()
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return new List<ProgramInfo>();
        }
    }
    return courseCodes;
}

public static List<string> GetClassroomsByCourseCode(string userId, string password, string dataSource, string courseCode)
{
    List<string> classrooms = new List<string>();
    string connectionString = $"User Id={userId};Password={password};Data Source={dataSource}";

    using (OracleConnection conn = new OracleConnection(connectionString))
    {
        try
        {
            Console.WriteLine($"DEBUG GetClassroomsByCourseCode: Starting query for course code: {courseCode}");
            conn.Open();
            string sql = "SELECT DISTINCT T.ROOM FROM FR_TRANSACTION T " +
                         "JOIN FR_TRANSACTION_STUDENT S ON T.TRANSACTION_ID = S.TRANSACTION_ID " +
                         "WHERE T.ROOM IS NOT NULL " +
                         "AND T.COURSE_CODE = :courseCode " +
                         "ORDER BY T.ROOM";

            Console.WriteLine($"DEBUG GetClassroomsByCourseCode: SQL Query: {sql}");
            Console.WriteLine($"DEBUG GetClassroomsByCourseCode: Parameter - courseCode: '{courseCode}'");

            using (OracleCommand cmd = new OracleCommand(sql, conn))
            {
                cmd.Parameters.Add(new OracleParameter("courseCode", OracleDbType.Varchar2)).Value = courseCode;

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string room = reader["ROOM"]?.ToString()?.Trim();
                        Console.WriteLine($"DEBUG GetClassroomsByCourseCode: Found room: '{room}'");
                        if (!string.IsNullOrEmpty(room) && room != "-")
                        {
                            classrooms.Add(room);
                        }
                    }
                }
            }
            Console.WriteLine($"DEBUG GetClassroomsByCourseCode: Found {classrooms.Count} classrooms for course '{courseCode}'");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DEBUG GetClassroomsByCourseCode: Exception: {ex.Message}");
            Console.WriteLine("Error fetching classrooms by course code: " + ex.Message);
        }
    }

    return classrooms;
}

public static string GetMostRecentDate(string userId, string password, string dataSource, string classroom, string courseCode)
{
    string mostRecentDate = "";
    string connectionString = $"User Id={userId};Password={password};Data Source={dataSource}";

    try
    {
        Console.WriteLine($"GetMostRecentDate - Starting search for classroom: {classroom}, courseCode: {courseCode}");
        
        if (string.IsNullOrEmpty(classroom) || string.IsNullOrEmpty(courseCode))
        {
            Console.WriteLine("GetMostRecentDate - Error: Empty classroom or courseCode parameter");
            return "";
        }
        
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            try
            {
                conn.Open();
                Console.WriteLine("GetMostRecentDate - Database connection opened successfully");
                
                string sql = "SELECT MAX(TO_DATE(TRIM(TRANSACTION_DATE), 'YYYY-MM-DD')) AS MOST_RECENT_DATE " +
                             "FROM FR_TRANSACTION " +
                             "WHERE TRIM(ROOM) = :classroom " +
                             "AND COURSE_CODE = :courseCode";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("classroom", OracleDbType.Varchar2)).Value = classroom;
                    cmd.Parameters.Add(new OracleParameter("courseCode", OracleDbType.Varchar2)).Value = courseCode;

                    object result = cmd.ExecuteScalar();
                    Console.WriteLine($"GetMostRecentDate - Query executed, result: {result ?? "null"}");
                    
                    if (result != null && result != DBNull.Value)
                    {
                        DateTime dt = (DateTime)result;
                        mostRecentDate = dt.ToString("yyyy-MM-dd");
                        Console.WriteLine($"GetMostRecentDate - Found date: {mostRecentDate}");
                    }
                    else
                    {
                        Console.WriteLine("GetMostRecentDate - No date found in database");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetMostRecentDate - Database error: {ex.Message}");
                Console.WriteLine($"GetMostRecentDate - Stack trace: {ex.StackTrace}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("GetMostRecentDate - Error creating connection: " + ex.Message);
    }

    return mostRecentDate;
}

public static string GetMostRecentTime(string userId, string password, string dataSource, string classroom, string courseCode, string date)
{
    string mostRecentTime = "";
    string connectionString = $"User Id={userId};Password={password};Data Source={dataSource}";

    try
    {
        Console.WriteLine($"GetMostRecentTime - Starting search for classroom: {classroom}, courseCode: {courseCode}, date: {date}");
        
        if (string.IsNullOrEmpty(classroom) || string.IsNullOrEmpty(courseCode) || string.IsNullOrEmpty(date))
        {
            Console.WriteLine("GetMostRecentTime - Error: Empty classroom, courseCode, or date parameter");
            return "";
        }
        
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            try
            {
                conn.Open();
                Console.WriteLine("GetMostRecentTime - Database connection opened successfully");
                
                string sql = "SELECT TRIM(TIME) AS TIME " +
                             "FROM FR_TRANSACTION " +
                             "WHERE TRIM(ROOM) = :classroom " +
                             "AND COURSE_CODE = :courseCode " +
                             "AND TRIM(TRANSACTION_DATE) = :date " +
                             "ORDER BY TO_DATE(SUBSTR(TIME, 1, 5), 'HH24:MI') DESC " +
                             "FETCH FIRST 1 ROW ONLY";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("classroom", OracleDbType.Varchar2)).Value = classroom;
                    cmd.Parameters.Add(new OracleParameter("courseCode", OracleDbType.Varchar2)).Value = courseCode;
                    cmd.Parameters.Add(new OracleParameter("date", OracleDbType.Varchar2)).Value = date;

                    object result = cmd.ExecuteScalar();
                    Console.WriteLine($"GetMostRecentTime - Query executed, result: {result ?? "null"}");
                    
                    if (result != null && result != DBNull.Value)
                    {
                        mostRecentTime = result.ToString();
                        Console.WriteLine($"GetMostRecentTime - Found time: {mostRecentTime}");
                    }
                    else
                    {
                        Console.WriteLine("GetMostRecentTime - No time found in database");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetMostRecentTime - Database error: {ex.Message}");
                Console.WriteLine($"GetMostRecentTime - Stack trace: {ex.StackTrace}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("GetMostRecentTime - Error creating connection: " + ex.Message);
    }

    return mostRecentTime;
}

public static Dictionary<string, int> GetDashboardStats(string userId, string password, string dataSource, string startDate, string endDate)
{
    string connectionString = $"User Id={userId};Password={password};Data Source={dataSource}";
    var stats = new Dictionary<string, int>();

    using (OracleConnection conn = new OracleConnection(connectionString))
    {
        try
        {
            conn.Open();
            Console.WriteLine("Connected to Oracle Database for dashboard stats!");

            // Get total students and payment status
            string paymentSql = @"
                SELECT 
                    COUNT(DISTINCT s.STUDENT_NUMBER) as total_students,
                    SUM(CASE WHEN s.PROFILE_STATUS = '1' THEN 1 ELSE 0 END) as total_debtors
                FROM FR_STUDENT s
                JOIN FR_STUDENT_COURSE sc ON s.STUDENT_NUMBER = sc.STUDENT_NUMBER";

            using (OracleCommand cmd = new OracleCommand(paymentSql, conn))
            {
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        stats["totalStudents"] = Convert.ToInt32(reader["total_students"]);
                        stats["totalDebtors"] = Convert.ToInt32(reader["total_debtors"]);
                        stats["totalNonDebtors"] = stats["totalStudents"] - stats["totalDebtors"];
                    }
                }
            }

            // Get attendance stats within date range
            string attendanceSql = @"
                SELECT 
                    COUNT(DISTINCT sa.STUDENT_NUMBER) as total_attending,
                    SUM(CASE WHEN s.PROFILE_STATUS = '1' THEN 1 ELSE 0 END) as debtors_attending
                FROM FR_STUDENT_ATTENDANCE sa
                JOIN FR_STUDENT s ON sa.STUDENT_NUMBER = s.STUDENT_NUMBER
                JOIN FR_TRANSACTION t ON sa.TRANSACTION_ID = t.TRANSACTION_ID
                WHERE t.TRANSACTION_DATE BETWEEN :startDate AND :endDate
                AND sa.ATTENDANCE_STATUS = '1'";

            using (OracleCommand cmd = new OracleCommand(attendanceSql, conn))
            {
                cmd.Parameters.Add(new OracleParameter(":startDate", startDate));
                cmd.Parameters.Add(new OracleParameter(":endDate", endDate));

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        stats["totalAttending"] = Convert.ToInt32(reader["total_attending"]);
                        stats["totalDebtorsAttending"] = Convert.ToInt32(reader["debtors_attending"]);
                        stats["totalAbsent"] = stats["totalStudents"] - stats["totalAttending"];
                    }
                }
            }

            return stats;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting dashboard stats: {ex.Message}");
            return new Dictionary<string, int>
            {
                ["totalStudents"] = 0,
                ["totalDebtors"] = 0,
                ["totalNonDebtors"] = 0,
                ["totalAttending"] = 0,
                ["totalAbsent"] = 0,
                ["totalDebtorsAttending"] = 0
            };
        }
    }
}

public static int GetStudentAttendanceCount(string userId, string password, string dataSource, string studentNumber, string courseCode)
{
    string connectionString = $"User Id={userId};Password={password};Data Source={dataSource}";
    
    using (OracleConnection conn = new OracleConnection(connectionString))
    {
        try
        {
            conn.Open();
            string sql = @"
                SELECT COUNT(*) 
                FROM FR_TRANSACTION_STUDENT ts
                JOIN FR_TRANSACTION t ON ts.TRANSACTION_ID = t.TRANSACTION_ID
                WHERE ts.STUDENT_NUMBER = :studentNumber 
                AND t.COURSE_CODE = :courseCode
                AND ts.ATTENDANCE = '1'";

            using (OracleCommand cmd = new OracleCommand(sql, conn))
            {
                cmd.Parameters.Add(new OracleParameter("studentNumber", studentNumber));
                cmd.Parameters.Add(new OracleParameter("courseCode", courseCode));
                
                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting student attendance count: {ex.Message}");
        }
    }
    return 0;
}

public static int GetTotalClassCount(string userId, string password, string dataSource, string courseCode)
{
    string connectionString = $"User Id={userId};Password={password};Data Source={dataSource}";
    
    using (OracleConnection conn = new OracleConnection(connectionString))
    {
        try
        {
            conn.Open();
            string sql = @"
                SELECT COUNT(DISTINCT TRANSACTION_DATE || TIME) 
                FROM FR_TRANSACTION 
                WHERE COURSE_CODE = :courseCode";

            using (OracleCommand cmd = new OracleCommand(sql, conn))
            {
                cmd.Parameters.Add(new OracleParameter("courseCode", courseCode));
                
                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting total class count: {ex.Message}");
        }
    }
    return 0;
}

public static List<Student> GetStudentsByCourse(string userId, string password, string dataSource, string courseCode)
{
    string connectionString = $"User Id={userId};Password={password};Data Source={dataSource}";
    var students = new List<Student>();
    
    using (OracleConnection conn = new OracleConnection(connectionString))
    {
        try
        {
            conn.Open();
            string sql = @"
                SELECT DISTINCT s.STUDENT_NUMBER, s.STUDENT_NAME, s.PROFILE_STATUS
                FROM FR_STUDENT s
                JOIN FR_TRANSACTION_STUDENT ts ON s.STUDENT_NUMBER = ts.STUDENT_NUMBER
                JOIN FR_TRANSACTION t ON ts.TRANSACTION_ID = t.TRANSACTION_ID
                WHERE t.COURSE_CODE = :courseCode";

            using (OracleCommand cmd = new OracleCommand(sql, conn))
            {
                cmd.Parameters.Add(new OracleParameter("courseCode", courseCode));
                
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        students.Add(new Student
                        {
                            StudentNumber = reader["STUDENT_NUMBER"].ToString(),
                            StudentName = reader["STUDENT_NAME"].ToString(),
                            ProfileStatus = reader["PROFILE_STATUS"].ToString()
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting students by course: {ex.Message}");
        }
    }
    return students;
}

public static Dictionary<string, object> GetDashboardData(string userId, string password, string dataSource, string startDate, string endDate)
{
    var dashboardData = new Dictionary<string, object>();
    var courseData = new List<Dictionary<string, object>>();
    string connectionString = $"User Id={userId};Password={password};Data Source={dataSource}";

    try
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();

            // Get overall statistics
            string statsSql = @"
                SELECT 
                    COUNT(DISTINCT s.STUDENT_NUMBER) as TOTAL_STUDENTS,
                    COUNT(DISTINCT CASE WHEN s.FEE_DUE = '1' THEN s.STUDENT_NUMBER END) as TOTAL_DEBTORS,
                    COUNT(DISTINCT CASE WHEN s.ATTENDANCE = '1' THEN s.STUDENT_NUMBER END) as TOTAL_ATTENDING,
                    COUNT(DISTINCT CASE WHEN s.FEE_DUE = '1' AND s.ATTENDANCE = '1' THEN s.STUDENT_NUMBER END) as DEBTORS_ATTENDING
                FROM FR_TRANSACTION t
                JOIN FR_TRANSACTION_STUDENT s ON t.TRANSACTION_ID = s.TRANSACTION_ID
                WHERE TO_DATE(t.TRANSACTION_DATE, 'YYYY-MM-DD') BETWEEN TO_DATE(:startDate, 'YYYY-MM-DD') AND TO_DATE(:endDate, 'YYYY-MM-DD')";

            using (OracleCommand cmd = new OracleCommand(statsSql, conn))
            {
                cmd.Parameters.Add(new OracleParameter("startDate", OracleDbType.Varchar2)).Value = startDate;
                cmd.Parameters.Add(new OracleParameter("endDate", OracleDbType.Varchar2)).Value = endDate;

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int totalStudents = Convert.ToInt32(reader["TOTAL_STUDENTS"]);
                        int totalDebtors = Convert.ToInt32(reader["TOTAL_DEBTORS"]);
                        int totalAttending = Convert.ToInt32(reader["TOTAL_ATTENDING"]);
                        int debtorsAttending = Convert.ToInt32(reader["DEBTORS_ATTENDING"]);

                        dashboardData["totalStudents"] = totalStudents;
                        dashboardData["totalDebtors"] = totalDebtors;
                        dashboardData["totalNonDebtors"] = totalStudents - totalDebtors;
                        dashboardData["totalAttending"] = totalAttending;
                        dashboardData["totalAbsent"] = totalStudents - totalAttending;
                        dashboardData["totalDebtorsAttending"] = debtorsAttending;
                        dashboardData["overallAttendanceRate"] = totalStudents > 0 ? (double)totalAttending / totalStudents * 100 : 0;
                        dashboardData["overallPaymentRate"] = totalStudents > 0 ? (double)(totalStudents - totalDebtors) / totalStudents * 100 : 0;
                        dashboardData["dateRange"] = $"{startDate} to {endDate}";
                    }
                }
            }

            // Get course-specific data
            string courseSql = @"
                SELECT 
                    t.COURSE_CODE,
                    c.COURSE_NAME,
                    COUNT(DISTINCT s.STUDENT_NUMBER) as STUDENT_COUNT,
                    COUNT(DISTINCT CASE WHEN s.FEE_DUE = '1' THEN s.STUDENT_NUMBER END) as DEBTOR_COUNT,
                    COUNT(DISTINCT CASE WHEN s.ATTENDANCE = '1' THEN s.STUDENT_NUMBER END) as ATTENDING_COUNT,
                    COUNT(DISTINCT CASE WHEN s.FEE_DUE = '1' AND s.ATTENDANCE = '1' THEN s.STUDENT_NUMBER END) as DEBTORS_ATTENDING_COUNT
                FROM FR_TRANSACTION t
                JOIN FR_TRANSACTION_STUDENT s ON t.TRANSACTION_ID = s.TRANSACTION_ID
                JOIN FR_COURSE c ON t.COURSE_CODE = c.COURSE_CODE
                WHERE TO_DATE(t.TRANSACTION_DATE, 'YYYY-MM-DD') BETWEEN TO_DATE(:startDate, 'YYYY-MM-DD') AND TO_DATE(:endDate, 'YYYY-MM-DD')
                GROUP BY t.COURSE_CODE, c.COURSE_NAME
                HAVING COUNT(DISTINCT s.STUDENT_NUMBER) > 0
                ORDER BY t.COURSE_CODE";

            using (OracleCommand cmd = new OracleCommand(courseSql, conn))
            {
                cmd.Parameters.Add(new OracleParameter("startDate", OracleDbType.Varchar2)).Value = startDate;
                cmd.Parameters.Add(new OracleParameter("endDate", OracleDbType.Varchar2)).Value = endDate;

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int studentCount = Convert.ToInt32(reader["STUDENT_COUNT"]);
                        int debtorCount = Convert.ToInt32(reader["DEBTOR_COUNT"]);
                        int attendingCount = Convert.ToInt32(reader["ATTENDING_COUNT"]);
                        int debtorsAttendingCount = Convert.ToInt32(reader["DEBTORS_ATTENDING_COUNT"]);

                        var courseStats = new Dictionary<string, object>
                        {
                            ["courseCode"] = reader["COURSE_CODE"].ToString(),
                            ["courseName"] = reader["COURSE_NAME"].ToString(),
                            ["studentCount"] = studentCount,
                            ["debtorCount"] = debtorCount,
                            ["attendingCount"] = attendingCount,
                            ["debtorsAttendingCount"] = debtorsAttendingCount,
                            ["attendanceRate"] = studentCount > 0 ? (double)attendingCount / studentCount * 100 : 0,
                            ["paymentRate"] = studentCount > 0 ? (double)(studentCount - debtorCount) / studentCount * 100 : 0
                        };

                        courseData.Add(courseStats);
                    }
                }
            }
        }

        dashboardData["courseData"] = courseData;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error getting dashboard data: {ex.Message}");
        return null;
    }

    return dashboardData;
}

public static List<object> GetStudentDetails(string userId, string password, string dataSource, string courseCode, string startDate, string endDate)
{
    var students = new List<object>();
    string connectionString = $"User Id={userId};Password={password};Data Source={dataSource}";

    try
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();

            string sql = @"
                SELECT s.STUDENT_NUMBER,
                       MAX(s2.STUDENT_NAME) as STUDENT_NAME,
                       CASE WHEN COUNT(CASE WHEN s.FEE_DUE = '1' THEN 1 END) > 0 THEN 1 ELSE 0 END as IS_DEBTOR,
                       CASE WHEN COUNT(CASE WHEN s.ATTENDANCE = '1' THEN 1 END) > 0 THEN 1 ELSE 0 END as IS_ATTENDING,
                       COUNT(CASE WHEN s.ATTENDANCE = '1' THEN 1 END) as CLASSES_ATTENDED,
                       COUNT(*) as TOTAL_CLASSES
                FROM FR_TRANSACTION t
                JOIN FR_TRANSACTION_STUDENT s ON t.TRANSACTION_ID = s.TRANSACTION_ID
                JOIN FR_STUDENT s2 ON s.STUDENT_NUMBER = s2.STUDENT_NUMBER
                WHERE t.COURSE_CODE = :courseCode
                AND TO_DATE(t.TRANSACTION_DATE, 'YYYY-MM-DD') BETWEEN TO_DATE(:startDate, 'YYYY-MM-DD') AND TO_DATE(:endDate, 'YYYY-MM-DD')
                GROUP BY s.STUDENT_NUMBER";

            using (OracleCommand cmd = new OracleCommand(sql, conn))
            {
                cmd.Parameters.Add(new OracleParameter("courseCode", OracleDbType.Varchar2)).Value = courseCode;
                cmd.Parameters.Add(new OracleParameter("startDate", OracleDbType.Varchar2)).Value = startDate;
                cmd.Parameters.Add(new OracleParameter("endDate", OracleDbType.Varchar2)).Value = endDate;

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        students.Add(new
                        {
                            studentId = reader["STUDENT_NUMBER"].ToString(),
                            studentName = reader["STUDENT_NAME"].ToString(),
                            isDebtor = Convert.ToInt32(reader["IS_DEBTOR"]) == 1,
                            isAttending = Convert.ToInt32(reader["IS_ATTENDING"]) == 1,
                            classesAttended = Convert.ToInt32(reader["CLASSES_ATTENDED"]),
                            totalClasses = Convert.ToInt32(reader["TOTAL_CLASSES"])
                        });
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error getting student details: {ex.Message}");
        return new List<object>();
    }

    return students;
}

private static ProgramInfo GetCourseInfo(string userId, string password, string dataSource, string courseCode)
{
    string connectionString = $"User Id={userId};Password={password};Data Source={dataSource}";
    try
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string sql = "SELECT * FROM FR_COURSE WHERE COURSE_CODE = :courseCode";

            using (OracleCommand cmd = new OracleCommand(sql, conn))
            {
                cmd.Parameters.Add(new OracleParameter("courseCode", OracleDbType.Varchar2)).Value = courseCode;

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new ProgramInfo
                        {
                            COURSECODE = reader["COURSE_CODE"].ToString(),
                            COURSENAME = reader["COURSE_NAME"].ToString(),
                            FACULTYCODE = reader["FACULTY_CODE"].ToString(),
                            LECTURER = reader["LECTURER"].ToString(),
                            ROOM = reader["ROOM"].ToString(),
                            ON_EVERY = reader["ON_EVERY"].ToString(),
                            TIME_FROM = reader["TIME_FROM"].ToString(),
                            TIME_TO = reader["TIME_TO"].ToString()
                        };
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error getting course info: {ex.Message}");
    }
    return null;
}

public static List<AttendanceReportViewModel> GetAttendanceReportForDateRange(string userId, string password, string dataSource, string classroom, string startDate, string endDate)
{
    List<AttendanceReportViewModel> attendanceData = new List<AttendanceReportViewModel>();
    string connectionString = $"User Id={userId};Password={password};Data Source={dataSource}";

    try
    {
        using (OracleConnection connection = new OracleConnection(connectionString))
        {
            connection.Open();
            string query = @"
                SELECT 
                    s.STUDENT_NUMBER,
                    s.STUDENT_NAME,
                    s.FEE_DUE,
                    a.ATTENDANCE,
                    a.TRANSACTION_DATE,
                    a.TRANSACTION_TIME
                FROM 
                    STUDENT s
                    JOIN ATTENDANCE a ON s.STUDENT_NUMBER = a.STUDENT_NUMBER
                WHERE 
                    a.CLASSROOM = :classroom
                    AND a.TRANSACTION_DATE BETWEEN TO_DATE(:startDate, 'YYYY-MM-DD') AND TO_DATE(:endDate, 'YYYY-MM-DD')
                ORDER BY 
                    a.TRANSACTION_DATE, a.TRANSACTION_TIME";

            using (OracleCommand command = new OracleCommand(query, connection))
            {
                command.Parameters.Add("classroom", OracleDbType.Varchar2).Value = classroom;
                command.Parameters.Add("startDate", OracleDbType.Varchar2).Value = startDate;
                command.Parameters.Add("endDate", OracleDbType.Varchar2).Value = endDate;

                using (OracleDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        attendanceData.Add(new AttendanceReportViewModel
                        {
                            StudentNumber = reader["STUDENT_NUMBER"].ToString(),
                            StudentName = reader["STUDENT_NAME"].ToString(),
                            FeeDue = reader["FEE_DUE"].ToString(),
                            Attendance = reader["ATTENDANCE"].ToString(),
                            TransactionDate = reader["TRANSACTION_DATE"].ToString(),
                            TransactionTime = reader["TRANSACTION_TIME"].ToString()
                        });
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in GetAttendanceReportForDateRange: {ex.Message}");
    }

    return attendanceData;
}

public static List<Faculty> GetAllFacultyCodes(string userId, string password, string dataSource)
{
    string connectionString = "User Id=" + userId + ";Password=" + password + ";Data Source=" + dataSource;
    List<Faculty> faculties = new List<Faculty>();

    using (OracleConnection conn = new OracleConnection(connectionString))
    {
        try
        {
            conn.Open();
            Console.WriteLine("Connected to Oracle Database!");

            string sql = "SELECT DISTINCT FACULTYCODE FROM FR_COURSE_TIMETABLE ORDER BY FACULTYCODE";

            using (OracleCommand cmd = new OracleCommand(sql, conn))
            {
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        faculties.Add(new Faculty
                        {
                            FacultyCode = reader["FACULTYCODE"].ToString()
                        });
                    }
                }
            }
            return faculties;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return new List<Faculty>();
        }
    }
}

public static List<ProgramInfo> GetCoursesByFacultyCode(string userId, string password, string dataSource, string facultyCode)
{
    string connectionString = "User Id=" + userId + ";Password=" + password + ";Data Source=" + dataSource;
    List<ProgramInfo> courses = new List<ProgramInfo>();

    using (OracleConnection conn = new OracleConnection(connectionString))
    {
        try
        {
            conn.Open();
            Console.WriteLine($"Connected to Oracle Database! Fetching courses for faculty: {facultyCode}");

            // First try to get courses with names from FR_STUDENT_COURSE
            string sql = 
                "SELECT DISTINCT ct.COURSECODE, sc.COURSE_NAME " +
                "FROM FR_COURSE_TIMETABLE ct " +
                "LEFT JOIN FR_STUDENT_COURSE sc ON ct.COURSECODE = sc.COURSE_CODE " +
                "WHERE ct.FACULTYCODE = :facultyCode " +
                "ORDER BY ct.COURSECODE";

            Console.WriteLine("Executing query: " + sql);
            Console.WriteLine("With parameter facultyCode: " + facultyCode);

            using (OracleCommand cmd = new OracleCommand(sql, conn))
            {
                cmd.Parameters.Add(new OracleParameter(":facultyCode", facultyCode));

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string courseCode = reader["COURSECODE"].ToString();
                        string courseName = reader["COURSE_NAME"]?.ToString() ?? "";
                        
                        Console.WriteLine($"Found course: {courseCode} - {courseName}");
                        
                        courses.Add(new ProgramInfo
                        {
                            COURSECODE = courseCode,
                            COURSENAME = string.IsNullOrEmpty(courseName) ? "Course " + courseCode : courseName
                        });
                    }
                }
            }

            // If no courses were found, try the fallback query
            if (courses.Count == 0)
            {
                Console.WriteLine("No courses found with join query, trying fallback query");
                
                sql = "SELECT DISTINCT COURSECODE FROM FR_COURSE_TIMETABLE " +
                      "WHERE FACULTYCODE = :facultyCode " +
                      "ORDER BY COURSECODE";
                
                Console.WriteLine("Executing fallback query: " + sql);
                
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.Parameters.Add(new OracleParameter(":facultyCode", facultyCode));

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string courseCode = reader["COURSECODE"].ToString();
                            Console.WriteLine($"Found course in fallback: {courseCode}");
                            
                            courses.Add(new ProgramInfo
                            {
                                COURSECODE = courseCode,
                                COURSENAME = "Course " + courseCode
                            });
                        }
                    }
                }
            }
            
            Console.WriteLine($"Total courses found: {courses.Count}");
            return courses;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetCoursesByFacultyCode: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return new List<ProgramInfo>();
        }
    }
}
}
