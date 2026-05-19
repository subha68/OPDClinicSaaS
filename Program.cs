using Microsoft.AspNetCore.Authentication.Cookies;
using QuestPDF.Infrastructure;
using BLL;
using DAL;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSession();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });
    
// MVC
builder.Services.AddControllersWithViews();

// Session config (IMPORTANT)
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});

builder.Services.AddScoped<UserDAL>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ClinicDAL>();
builder.Services.AddScoped<ClinicService>();
builder.Services.AddScoped<RoleDAL>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<SessionHelper>();
builder.Services.AddScoped<DepartmentDAL>();
builder.Services.AddScoped<DepartmentService>();
builder.Services.AddScoped<DoctorDAL>();
builder.Services.AddScoped<DoctorService>();
builder.Services.AddScoped<MenuDAL>();
builder.Services.AddScoped<MenuService>();
builder.Services.AddScoped<AppointmentSlotDAL>();
builder.Services.AddScoped<AppointmentSlotService>();
builder.Services.AddScoped<DoctorScheduleDAL>();
builder.Services.AddScoped<DoctorScheduleService>();
builder.Services.AddScoped<AppointmentDAL>();
builder.Services.AddScoped<AppointmentService>();
builder.Services.AddScoped<ConsultationDAL>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseCookiePolicy();

// MUST be in this order
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();