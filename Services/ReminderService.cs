using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using FR_HKVision.Models;

namespace FR_HKVision.Services
{
    public class ReminderService : IHostedService, IDisposable
    {
        private readonly ILogger<ReminderService> _logger;
        private readonly IServiceProvider _services;
        private Timer _timer;

        public ReminderService(ILogger<ReminderService> logger, IServiceProvider services)
        {
            _logger = logger;
            _services = services;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Reminder Service is starting.");

            // Check for classes every minute
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            _logger.LogInformation("Checking for upcoming classes...");

            try
            {
                using (var scope = _services.CreateScope())
                {
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    var dbConfig = scope.ServiceProvider.GetRequiredService<OracleDBConfig>();

                    // Get all classes for today
                    var todayClasses = OracleConnectionClass.GetAllCourseCodes(
                        dbConfig.UserId,
                        dbConfig.Password,
                        dbConfig.DataSource
                    );

                    foreach (var programInfo in todayClasses)
                    {
                        // Parse class time
                        if (DateTime.TryParse(programInfo.TIME_FROM, out DateTime classTime))
                        {
                            // If class is starting in 30 minutes
                            var timeUntilClass = classTime - DateTime.Now;
                            if (timeUntilClass.TotalMinutes <= 30 && timeUntilClass.TotalMinutes > 25)
                            {
                                // Get students for this class
                                var students = OracleConnectionClass.studentListByCoursecode(
                                    dbConfig.UserId,
                                    dbConfig.Password,
                                    dbConfig.DataSource,
                                    programInfo.COURSECODE
                                );

                                // Send reminder to each student
                                foreach (var student in students)
                                {
                                    // In a real application, you would get the email from the student record
                                    string studentEmail = $"{student.StudentNumber}@example.com"; // Dummy email

                                    await emailService.SendClassReminderAsync(
                                        studentEmail,
                                        student.StudentName,
                                        programInfo.COURSENAME,
                                        programInfo.COURSECODE,
                                        programInfo.ROOM,
                                        $"{programInfo.TIME_FROM}-{programInfo.TIME_TO}"
                                    );
                                }

                                _logger.LogInformation($"Sent reminders for class {programInfo.COURSECODE} starting at {classTime}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending class reminders");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Reminder Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
} 