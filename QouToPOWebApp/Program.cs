using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using QouToPOWebApp;
using QouToPOWebApp.Filters;
using QouToPOWebApp.Models.MiscModels;
using QouToPOWebApp.Services;
using System.Net;

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

builder.Services.AddHttpContextAccessor();

// Load settings
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

// Authentication + Cookie Config
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "qtp.Auth";
        options.LoginPath = "/Access/Login";
        options.AccessDeniedPath = "/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
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
    .PersistKeysToFileSystem(new DirectoryInfo("/app/DataProtection-Keys"))
    .SetApplicationName("QuoToPoWebApp");

// Forwarded Headers (for reverse proxy)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownProxies.Add(IPAddress.Parse("192.168.161.111")); // Nginx IP
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
