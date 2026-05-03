var builder = WebApplication.CreateBuilder(args);

// Add MVC + Session
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

// Enable session BEFORE endpoints
app.UseSession();

// MVC routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();