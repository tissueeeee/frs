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
using System.Collections.Generic;

namespace FR_HKVision.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _configuration;

        public AccountController(ILogger<AccountController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        public IActionResult Logout()
        {
            // Clear all session data
            HttpContext.Session.Clear();

            TempData["InfoMessage"] = "Logout Successful.";
            return RedirectToAction("Login", "Account");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                string username = model.Username.Trim();
                string password = model.Password.Trim();

                // Check for bypass account
                string bypassUsername = _configuration["BypassAccount:Username"];
                string bypassPassword = _configuration["BypassAccount:Password"];

                if (!string.IsNullOrEmpty(bypassUsername) && !string.IsNullOrEmpty(bypassPassword) &&
                    username == bypassUsername && password == bypassPassword)
                {
                    HttpContext.Session.SetString("Username", model.Username);
                    return RedirectToAction("Success");
                }

                string authResult = AuthenticateUCSIStaff(username, password);

                if (authResult == "Yes")
                {
                    HttpContext.Session.SetString("Username", model.Username);
                    return RedirectToAction("Success");
                }
                else
                {
                    return RedirectToAction("Fail");
                }
            }
            return RedirectToAction("Fail");
        }

        public IActionResult Success()
        {
            //return RedirectToAction("Index", "Home");
            return RedirectToAction("Add", "Person");
        }

        public IActionResult Fail()
        {
            TempData["ErrorMessage"] = "Invalid username or password.";
            return RedirectToAction("Login", "Account");
        }

        public string AuthenticateUCSIStaff(string StaffID, string Password)
        {
            string domain = "swdca.ucsihq.edu/DC=ucsihq;DC=edu";
            string ldapPath = $"LDAP://{domain}";

            string IsValidUser = "No";

            try
            {
                System.DirectoryServices.DirectoryEntry entry = new System.DirectoryServices.DirectoryEntry(ldapPath, StaffID, Password);
                object nativeObject = entry.NativeObject;
                IsValidUser = "Yes";
            }
            catch (Exception ex)
            {
                IsValidUser = "No : " + ex.Message;
            }

            return IsValidUser;
        }

        [HttpGet]
        public JsonResult GetChartData(string courseCode)
        {
            // Extract just the course code before the dash
            if (courseCode.Contains(" - "))
            {
                courseCode = courseCode.Split(" - ")[0].Trim();
            }
            
            // Get database credentials
            string userId = _configuration["OracleDBSettings:UserId"];
            string password = _configuration["OracleDBSettings:Password"];
            string dataSource = _configuration["OracleDBSettings:DataSource"];
            
            // Log parameters
            Console.WriteLine($"GetChartData in AccountController - Using extracted course code: {courseCode}");
            Console.WriteLine($"GetChartData in AccountController - DB Settings: UserId={userId}, DataSource={dataSource}");
            
            var diagnosticInfo = new Dictionary<string, string>
            {
                ["dbUserId"] = userId ?? "null",
                ["dbDataSource"] = dataSource ?? "null",
                ["courseCode"] = courseCode ?? "null"
            };
            
            // Return simulated data as we're not implementing the full method here
            return Json(new
            {
                totalStudents = 25,
                debtorCount = 5,
                nonDebtorCount = 20,
                attendingCount = 18,
                absentCount = 7,
                debtorAttendingCount = 2,
                courseCode,
                diagnosticInfo,
                isSimulatedData = true,
                message = "Redirect to Report controller for actual data"
            });
        }
    }
}
