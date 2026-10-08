using Microsoft.AspNetCore.Authentication.Cookies;
using QuestPDF.Infrastructure;
using Microsoft.AspNetCore.Http.Features;
using BLL;
using DAL;
using Helpers;
using Filters;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<FormOptions>(options =>
{
    options.ValueCountLimit = 10000;
});

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });
    
// MVC
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<FirstLoginFilter>();
});

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

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<SessionHelper>();
builder.Services.AddScoped<UserDAL>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ClinicDAL>();
builder.Services.AddScoped<ClinicService>();
builder.Services.AddScoped<RoleDAL>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<DepartmentDAL>();
builder.Services.AddScoped<DepartmentService>();
builder.Services.AddScoped<DoctorDAL>();
builder.Services.AddScoped<DoctorService>();
builder.Services.AddScoped<DoctorScheduleDAL>();
builder.Services.AddScoped<DoctorScheduleService>();
builder.Services.AddScoped<FeatureDAL>();
builder.Services.AddScoped<FeatureService>();
builder.Services.AddScoped<MenuDAL>();
builder.Services.AddScoped<MenuService>();
builder.Services.AddScoped<PatientDAL>();
builder.Services.AddScoped<PatientService>();
builder.Services.AddScoped<DoctorScheduleDAL>();
builder.Services.AddScoped<DoctorScheduleService>();
builder.Services.AddScoped<AppointmentDAL>();
builder.Services.AddScoped<AppointmentService>();
builder.Services.AddScoped<ConsultationDAL>();
builder.Services.AddScoped<ConsultationService>();
builder.Services.AddScoped<BillDAL>();
builder.Services.AddScoped<BillLineDAL>();
builder.Services.AddScoped<BillService>();
builder.Services.AddScoped<PaymentDAL>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<RefundDAL>();
builder.Services.AddScoped<RefundService>();
builder.Services.AddScoped<BillingChargeDAL>();
builder.Services.AddScoped<BillingChargeService>();
builder.Services.AddScoped<VisitDAL>();
builder.Services.AddScoped<VisitService>();
builder.Services.AddScoped<MedicineDAL>();
builder.Services.AddScoped<MedicineService>();
builder.Services.AddScoped<PrescriptionTemplateDAL>();
builder.Services.AddScoped<PrescriptionTemplateService>();
builder.Services.AddScoped<VitalDefinitionDAL>();
builder.Services.AddScoped<VitalDefinitionService>();
builder.Services.AddScoped<VitalDAL>();
builder.Services.AddScoped<VitalService>();
builder.Services.AddSingleton<CalculationEngine>();
builder.Services.AddScoped<TestDefinitionDAL>();
builder.Services.AddScoped<TestDefinitionService>();
builder.Services.AddScoped<TestResultDAL>();
builder.Services.AddScoped<TestResultService>();
builder.Services.AddScoped<ConsultationVitalDAL>();
builder.Services.AddScoped<ConsultationTestResultDAL>();

builder.Services.AddScoped<ReportDAL>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<ReportLookupRegistry>();
builder.Services.AddScoped<ReportSpecificDAL>();

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