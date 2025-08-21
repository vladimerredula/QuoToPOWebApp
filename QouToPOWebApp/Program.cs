using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using NReco.Logging.File;
using QouToPOWebApp;
using QouToPOWebApp.Filters;
using QouToPOWebApp.Helpers;
using QouToPOWebApp.Models.MiscModels;
using QouToPOWebApp.Services;
using StackExchange.Redis;
using System.Net;
using UAParser;

var builder = WebApplication.CreateBuilder(args);

// Configure Services
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<BreadcrumbActionFilter>();
});

// Dependency Injection
builder.Services.AddScoped<PdfiumViewerService>();
builder.Services.AddScoped<PdfPigService>();
builder.Services.AddScoped<TabulaService>();
builder.Services.AddScoped<IBreadcrumbService, BreadcrumbService>();
builder.Services.AddScoped<BreadcrumbActionFilter>();
builder.Services.AddScoped(typeof(LogService<>));
builder.Services.AddScoped<TemplateService>();
builder.Services.AddSingleton<EncryptionHelper>();

builder.Services.AddHttpContextAccessor();

// Load settings
builder.Services.Configure<EncryptionSetting>(builder.Configuration.GetSection("Encryption"));
var poSetting = builder.Configuration.GetSection("PoSettings").Get<PoSetting>();
if (poSetting == null)
    throw new InvalidOperationException("PoSettings configuration section is missing or invalid.");
builder.Services.AddSingleton(poSetting);

// Database (MySQL)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

// Redis connection (shared for cache + DataProtection)
var redis = ConnectionMultiplexer.Connect("redis:6379");

// Caching (Redis)
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "redis:6379"; // Use container name in Docker network
});

// Authentication + Cookie Config
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "qtp.Auth";
        options.LoginPath = "/Access/Login";
        options.AccessDeniedPath = "/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;

        options.Events = new CookieAuthenticationEvents
        {
            OnSignedIn = async ctx =>
            {
                var db = ctx.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
                var userId = ctx.Principal.FindFirst("Personnelid")?.Value;

                if (int.TryParse(userId, out var personnelId))
                {
                    var ip = ctx.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                          ?? ctx.HttpContext.Connection.RemoteIpAddress?.ToString();
                    if (ip == "::1") ip = "127.0.0.1";

                    var userAgent = ctx.Request.Headers["User-Agent"].ToString(); var parser = Parser.GetDefault();
                    var clientInfo = parser.Parse(userAgent);
                    var browser = clientInfo.UA.Family;
                    var browserVersion = clientInfo.UA.ToString();
                    var os = clientInfo.OS.ToString();
                    var device = clientInfo.Device.ToString();
                    var userAgentString = $"{browser} ({browserVersion}) on {os} ({device})";

                    db.Sessions.Add(new Session
                    {
                        Session_ID = Guid.NewGuid().ToString(),
                        Personnel_ID = personnelId,
                        Ip_address = ip,
                        User_agent = userAgentString,
                        App_name = "QTP",
                        Signed_in_at = DateTime.Now,
                        Last_activity = DateTime.Now
                    });
                    await db.SaveChangesAsync();
                }
            },
            OnValidatePrincipal = async ctx =>
            {
                var db = ctx.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
                var userId = ctx.Principal.FindFirst("Personnelid")?.Value;

                if (int.TryParse(userId, out var personnelId))
                {
                    var session = await db.Sessions
                        .Where(s => s.Personnel_ID == personnelId && s.Signed_out_at == null)
                        .OrderByDescending(s => s.Signed_in_at)
                        .FirstOrDefaultAsync();

                    if (session != null)
                    {
                        session.Last_activity = DateTime.Now;
                        await db.SaveChangesAsync();
                    }
                }
            },
            OnSigningOut = async ctx =>
            {
                var db = ctx.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
                var userId = ctx.HttpContext.User.FindFirst("Personnelid")?.Value;

                if (int.TryParse(userId, out var personnelId))
                {
                    var session = await db.Sessions
                        .Where(s => s.Personnel_ID == personnelId && s.Signed_out_at == null)
                        .OrderByDescending(s => s.Signed_in_at)
                        .FirstOrDefaultAsync();

                    if (session != null)
                    {
                        session.Signed_out_at = DateTime.Now;
                        session.Last_activity = DateTime.Now;
                        await db.SaveChangesAsync();
                    }
                }
            }
        };
    });

builder.Services.AddAuthorization();

// Session
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "qtp.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.None; // Use Always if HTTPS is enforced
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

// Data Protection
builder.Services.AddDataProtection()
    .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys")
    .SetApplicationName("QuoToPoWebApp");

// Forwarded Headers (for reverse proxy)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownProxies.Add(IPAddress.Parse("192.168.161.111")); // Nginx IP
});

// File Logging (General + Error)
builder.Logging.AddFile(builder.Configuration.GetSection("Logging:File"), options =>
{
    string currentLogFileName = string.Empty;
    DateTime currentDate = DateTime.MinValue;

    options.FormatLogFileName = folder =>
    {
        var now = DateTime.UtcNow;
        if (currentDate.Date != now.Date || string.IsNullOrEmpty(currentLogFileName))
        {
            currentDate = now;
            string logPath = $"{now:yyyy}/{now:MM}/{now:dd}/{now:yyyy-MM-dd}.log";
            currentLogFileName = Path.Combine(folder, logPath);
            var directoryPath = Path.GetDirectoryName(currentLogFileName);
            if (!string.IsNullOrEmpty(directoryPath))
                Directory.CreateDirectory(directoryPath);
        }
        return currentLogFileName;
    };

    options.FormatLogEntry = msg =>
    {
        var sb = new System.Text.StringBuilder();
        sb.Append(DateTime.Now.ToString("o"));
        sb.Append($" Level:{msg.LogLevel}, {msg.LogName}, ");
        if (msg.Exception != null)
        {
            var trace = new System.Diagnostics.StackTrace(msg.Exception, true);
            var frame = trace.GetFrame(0);
            if (frame != null)
                sb.Append($"Line:{frame.GetFileLineNumber()}, ");
            sb.Append($"Exception:{msg.Exception.Message}");
        }
        else
        {
            sb.Append(msg.Message);
        }
        return sb.ToString();
    };

    options.FileSizeLimitBytes = 1 * 1024 * 1024;
    options.MaxRollingFiles = 3;
});

builder.Logging.AddFile(builder.Configuration.GetSection("Logging:File"), options =>
{
    string errorLogFileName = string.Empty;
    DateTime currentDate = DateTime.MinValue;

    options.FormatLogFileName = folder =>
    {
        var now = DateTime.UtcNow;
        if (currentDate.Date != now.Date || string.IsNullOrEmpty(errorLogFileName))
        {
            currentDate = now;
            string logPath = $"errors/{now:yyyy}/{now:MM}/{now:dd}/{now:yyyy-MM-dd}-errors.log";
            errorLogFileName = Path.Combine(folder, logPath);
            var directoryPath = Path.GetDirectoryName(errorLogFileName);
            if (!string.IsNullOrEmpty(directoryPath))
                Directory.CreateDirectory(directoryPath);
        }
        return errorLogFileName;
    };

    options.FilterLogEntry = msg =>
        msg.LogLevel == LogLevel.Error || msg.LogLevel == LogLevel.Critical;

    options.FileSizeLimitBytes = 1 * 1024 * 1024;
    options.MaxRollingFiles = 3;
});

// Bind to port 80 inside container
builder.WebHost.UseUrls("http://0.0.0.0:80");

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseForwardedHeaders();
//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Access}/{action=Index}/{id?}");

app.Run();
