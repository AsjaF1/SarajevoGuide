using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SarajevoGuide.Data;
using System.Globalization;
using System.Net.Mail;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Add DbContext using MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 32)) // replace with your MySQL version
    ));

// Add Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// Email configuration
var emailConfig = builder.Configuration.GetSection("EmailConfiguration");
builder.Services.Configure<EmailConfiguration>(emailConfig);
builder.Services.AddTransient<IEmailSender, DummyEmailSender>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/RegistrovaniKorisniks/Login";
    options.AccessDeniedPath = "/Home/AccessDenied";
});

var supportedCultures = new[] { new CultureInfo("en-US") };

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await CreateRolesAndAdminAsync(services);
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

async Task CreateRolesAndAdminAsync(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string[] roleNames = { "Admin", "User" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    var adminEmail = "admin@example.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}

public class EmailConfiguration
{
    public string From { get; set; }
    public string SmtpServer { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}

public class DummyEmailSender : IEmailSender
{
    private readonly ILogger<DummyEmailSender> _logger;
    public DummyEmailSender(ILogger<DummyEmailSender> logger) => _logger = logger;

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        Console.WriteLine($"Email to {email}");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"Message: {htmlMessage}");
        return Task.CompletedTask;
    }
}
