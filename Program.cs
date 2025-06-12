using Microsoft.AspNetCore.DataProtection;
using FR_HKVision.Services;
using FR_HKVision.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache(); // Required for session storage
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Ensure the session cookie is not accessible via JavaScript
    options.Cookie.IsEssential = true; // Ensure session works without user consent
});

// Add HttpClient factory
builder.Services.AddHttpClient();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Oracle DB settings
builder.Services.Configure<OracleDBConfig>(builder.Configuration.GetSection("OracleDBSettings"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<OracleDBConfig>>().Value);

// Add email and reminder services
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddHostedService<ReminderService>();

//builder.Services.AddDataProtection()
//                .PersistKeysToFileSystem(new System.IO.DirectoryInfo(@"/wwwroot/DataProtectionKey/key.txt"));

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new System.IO.DirectoryInfo(@"\wwwroot\DataProtectionKey\"))  // Specify a directory for the keys
    .SetApplicationName("MyApp");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
