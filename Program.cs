using BLL;
using DAL;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseCookiePolicy();

// MUST be in this order
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();