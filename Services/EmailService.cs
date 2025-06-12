using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace FR_HKVision.Services
{
    public interface IEmailService
    {
        Task SendClassReminderAsync(string studentEmail, string studentName, string courseName, string courseCode, string room, string time);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            // In production, these would come from configuration
            _smtpServer = "smtp.example.com"; // Dummy SMTP server
            _smtpPort = 587;
            _fromEmail = "noreply@example.com";
            _fromName = "Class Reminder System";
        }

        public async Task SendClassReminderAsync(string studentEmail, string studentName, string courseName, string courseCode, string room, string time)
        {
            try
            {
                // For development/testing, just log the email instead of sending it
                Console.WriteLine($"[EMAIL SIMULATION] Sending class reminder:");
                Console.WriteLine($"To: {studentEmail}");
                Console.WriteLine($"Subject: Reminder: {courseName} class today");
                Console.WriteLine($"Body: Dear {studentName},\n\n" +
                                $"This is a reminder that your class {courseName} ({courseCode}) " +
                                $"is scheduled for today at {time} in room {room}.\n\n" +
                                $"Please ensure you arrive on time.\n\n" +
                                $"Best regards,\nClass Reminder System");

                // Uncomment below code when ready to send real emails
                /*
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(_fromEmail, _fromName);
                    message.To.Add(new MailAddress(studentEmail, studentName));
                    message.Subject = $"Reminder: {courseName} class today";
                    message.Body = $"Dear {studentName},\n\n" +
                                 $"This is a reminder that your class {courseName} ({courseCode}) " +
                                 $"is scheduled for today at {time} in room {room}.\n\n" +
                                 $"Please ensure you arrive on time.\n\n" +
                                 $"Best regards,\nClass Reminder System";

                    using (var client = new SmtpClient(_smtpServer, _smtpPort))
                    {
                        // Configure SMTP client settings here
                        await client.SendMailAsync(message);
                    }
                }
                */
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email reminder: {ex.Message}");
                // Log the error or handle it appropriately
            }
        }
    }
} 