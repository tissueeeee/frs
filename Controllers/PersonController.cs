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
using Microsoft.AspNetCore.Identity;
using System.Security.Policy;
using Microsoft.AspNetCore.Authentication;
using System.DirectoryServices;
using Microsoft.AspNetCore.Http; // Required for Session


namespace FR_HKVision.Controllers
{
    public class PersonController : Controller
    {
        private readonly ILogger<PersonController> _logger;
        private readonly IConfiguration _configuration;

        public PersonController(ILogger<PersonController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;


        }




        public IActionResult Add()
        {
            try
            {
                string todayDate = DateTime.Today.ToString("yyyy-MM-dd");

                string OracelDBUserId = _configuration["OracleDBSettings:UserId"];
                string OracelDBPassword = _configuration["OracleDBSettings:Password"];
                string OracelDBDataSource = _configuration["OracleDBSettings:DataSource"];
                string OracelDBFacultyCode = _configuration["OracleDBSettings:FacultyCode"];

                // Get course code from session
                string OracelDBCourseCode = HttpContext.Session.GetString("Coursecode");
                
                // If no course code is selected, redirect to home page
                if (string.IsNullOrEmpty(OracelDBCourseCode))
                {
                    TempData["ErrorMessage"] = "Please select a course code first.";
                    return RedirectToAction("Index", "Home");
                }

                string ApiKey = _configuration["OpenApiSettings:ApiKey"];
                string ApiSecretKey = _configuration["OpenApiSettings:ApiSecretKey"];
                string ApiBaseUrl = _configuration["OpenApiSettings:ApiBaseUrl"];

                //API version
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add(HttpClientImpl.HTTP_HEADER_ACCEPT, "application/json");
                headers.Add(HttpClientImpl.HTTP_HEADER_CONTENT_TYPE, "application/json;charset=UTF-8");

                string url = ApiBaseUrl + "/common/v1/version";
                string Text = "";
                string contentVersion = HttpClientImpl.Post(url, headers, ApiKey, ApiSecretKey, Text);

                List<Person> studentListFinal = new List<Person>();
                List<Student> studentList = OracleConnectionClass.studentListByCoursecode(OracelDBUserId, OracelDBPassword, OracelDBDataSource, OracelDBCourseCode);

                // Print each student
                int StudentSeq = 1;
                foreach (var studentArray in studentList)
                {
                    try
                    {
                        string StudentMessage = "-";
                        string StudentNumber = studentArray.StudentNumber;
                        string StudentName = studentArray.StudentName;
                        string ProgrammeCode = studentArray.ProgrammeCode;
                        string ProgrammeName = studentArray.ProgrammeName;
                        string ProfileStatus = studentArray.ProfileStatus;
                        string StudentPhoto = studentArray.StudentPhoto;
                        string AttendanceTime = studentArray.StudentPhoto;
                        string AttendancePhoto = studentArray.StudentNumber;
                        string profileRecord = "1";

                        string imagePath = @"\\172.27.150.75\e\New Version\files\" + StudentPhoto;

                        if (!System.IO.File.Exists(imagePath))
                        {
                            StudentMessage = "Student Photo not found";

                            Dictionary<string, string> headersSearchPerson = new Dictionary<string, string>();
                            headersSearchPerson.Add(HttpClientImpl.HTTP_HEADER_ACCEPT, "application/json");
                            headersSearchPerson.Add(HttpClientImpl.HTTP_HEADER_CONTENT_TYPE, "application/json;charset=UTF-8");

                            string urlSearchPerson = ApiBaseUrl + "/resource/v1/person/advance/personList";
                            var TextSearchPerson = new
                            {
                                pageNo = 1,
                                pageSize = 10,
                                personName = StudentName
                            };
                            string jsonDataTextSearchPerson = JsonConvert.SerializeObject(TextSearchPerson);
                            string contentSearchPerson = HttpClientImpl.Post(urlSearchPerson, headersSearchPerson, ApiKey, ApiSecretKey, jsonDataTextSearchPerson);

                            if (!string.IsNullOrEmpty(contentSearchPerson))
                            {
                                JObject dataSearchPerson = JObject.Parse(contentSearchPerson);
                                
                                // Safely check for data and total properties
                                if (dataSearchPerson != null && 
                                    dataSearchPerson["data"] != null && 
                                    dataSearchPerson["data"].Type != JTokenType.Null &&
                                    dataSearchPerson["data"]["total"] != null &&
                                    dataSearchPerson["data"]["total"].Type != JTokenType.Null)
                                {
                                    string dataSearchPerson_code = dataSearchPerson["code"]?.ToString();
                                    string totalPersonFind = "1";
                                    if (dataSearchPerson_code == "0")
                                    {
                                        totalPersonFind = dataSearchPerson["data"]["total"]?.ToString();
                                    }
                                    if(totalPersonFind != "0")
                                    {
                                        StudentMessage = "Student Exist";
                                    }
                                }
                            }
                        }
                        else
                        {
                            StudentMessage = "Student Photo found";

                            List<Dictionary<string, string>> faces = new List<Dictionary<string, string>>();
                            faces.Add(new Dictionary<string, string>());

                            string base64String = ConvertImageToBase64(imagePath);
                            faces[0]["faceData"] = base64String;

                            Dictionary<string, string> headersSearchPerson = new Dictionary<string, string>();
                            headersSearchPerson.Add(HttpClientImpl.HTTP_HEADER_ACCEPT, "application/json");
                            headersSearchPerson.Add(HttpClientImpl.HTTP_HEADER_CONTENT_TYPE, "application/json;charset=UTF-8");

                            string urlSearchPerson = ApiBaseUrl + "/resource/v1/person/advance/personList";
                            var TextSearchPerson = new
                            {
                                pageNo = 1,
                                pageSize = 10,
                                personName = StudentName
                            };
                            string jsonDataTextSearchPerson = JsonConvert.SerializeObject(TextSearchPerson);
                            string contentSearchPerson = HttpClientImpl.Post(urlSearchPerson, headersSearchPerson, ApiKey, ApiSecretKey, jsonDataTextSearchPerson);

                            if (!string.IsNullOrEmpty(contentSearchPerson))
                            {
                                JObject dataSearchPerson = JObject.Parse(contentSearchPerson);
                                
                                // Safely check for data and total properties
                                if (dataSearchPerson != null && 
                                    dataSearchPerson["data"] != null && 
                                    dataSearchPerson["data"].Type != JTokenType.Null &&
                                    dataSearchPerson["data"]["total"] != null &&
                                    dataSearchPerson["data"]["total"].Type != JTokenType.Null)
                                {
                                    string dataSearchPerson_code = dataSearchPerson["code"]?.ToString();
                                    string totalPersonFind = "1";
                                    if (dataSearchPerson_code == "0")
                                    {
                                        totalPersonFind = dataSearchPerson["data"]["total"]?.ToString();
                                    }

                                    if (totalPersonFind == "0")
                                    {
                                        Dictionary<string, string> headersPersonCode = new Dictionary<string, string>();
                                        headersPersonCode.Add(HttpClientImpl.HTTP_HEADER_ACCEPT, "application/json");
                                        headersPersonCode.Add(HttpClientImpl.HTTP_HEADER_CONTENT_TYPE, "application/json;charset=UTF-8");

                                        string urlPersonCode = ApiBaseUrl + "/resource/v1/person/single/add";
                                        var TextPersonCode = new
                                        {
                                            personCode = StudentNumber,
                                            personName = StudentName,
                                            personFamilyName = "",
                                            personGivenName = StudentName,
                                            gender = 0,
                                            orgIndexCode = "2",
                                            faces = faces
                                        };
                                        string jsonDataTextPersonCode = JsonConvert.SerializeObject(TextPersonCode);
                                        string contentPersonCode = HttpClientImpl.Post(urlPersonCode, headersPersonCode, ApiKey, ApiSecretKey, jsonDataTextPersonCode);

                                        if (!string.IsNullOrEmpty(contentPersonCode))
                                        {
                                            JObject dataPersonCode = JObject.Parse(contentPersonCode);
                                            
                                            // Safely check for data and code properties
                                            if (dataPersonCode != null && 
                                                dataPersonCode["data"] != null && 
                                                dataPersonCode["data"].Type != JTokenType.Null &&
                                                dataPersonCode["code"] != null &&
                                                dataPersonCode["code"].Type != JTokenType.Null)
                                            {
                                                string dataPersonCode_code = dataPersonCode["code"]?.ToString();
                                                if (dataPersonCode_code == "0")
                                                {
                                                    string personIndexCodes = dataPersonCode["data"]?.ToString();
                                                    StudentMessage = "Student added successful";
                                                }
                                                else
                                                {
                                                    StudentMessage = "Error when adding student info";
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        StudentMessage = "Student Exist";
                                    }
                                }
                            }
                        }

                        Person person = new Person
                        {
                            StudentSeq = StudentSeq,
                            StudentNumber = StudentNumber,
                            StudentName = StudentName,
                            StudentPhoto = StudentPhoto,
                            ProgrammeCode = ProgrammeCode,
                            ProgrammeName = ProgrammeName,
                            ProfileStatus = ProfileStatus,
                            ProfileRecord = profileRecord,
                            AttendanceTime = AttendanceTime,
                            AttendancePhoto = AttendancePhoto,
                            StudentMessage = StudentMessage
                        };
                        studentListFinal.Add(person);
                        StudentSeq++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error processing student {studentArray.StudentNumber}: {ex.Message}");
                        // Continue with next student even if one fails
                        continue;
                    }
                }

                string username = HttpContext.Session.GetString("Username") ?? "";
                ViewBag.Username = username;

                var model = new PersonViewModel
                {
                    Persons = studentListFinal
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Add method: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while processing the request. Please try again.";
                return RedirectToAction("Index", "Home");
            }
        }

        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


       

        public IActionResult Fail()
        {
            TempData["ErrorMessage"] = "Invalid username or password.";
            return RedirectToAction("Login", "Account");
        }

        public static string ConvertImageToBase64(string imagePath)
        {
            // Read the image file into a byte array
            byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);

            // Convert the byte array to a Base64 string
            return Convert.ToBase64String(imageBytes);
        }


    }

}
