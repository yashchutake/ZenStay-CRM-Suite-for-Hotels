using ZenHotelManagement.WebApp.Services;
using ZenHotelManagement.WebApp.Services.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ZenHotelManagement.WebApp.IdentityDbContex;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient(); //To make http request to WebApi 
builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri("http://localhost:5000/");
});
builder.Services.AddSingleton<ICaptchaService, CaptchaService>();
builder.Services.AddControllersWithViews(); //this is service register for razor view
builder.Services.AddSession();
builder.Services.AddScoped<IServiceManager, ServiceManager>(); // Corrected line

// Register services
builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();
builder.Services.AddScoped<IReceptionistDashboardService, ReceptionistDashboardService>();
builder.Services.AddScoped<ICheckoutCalculationService, CheckoutCalculationService>();

builder.Services.AddDbContext<AppIdentityDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ZenIdentityConnection"));
});
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppIdentityDbContext>();
var app = builder.Build();
app.UseStaticFiles();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
 name: "default",
 pattern: "{controller=Account}/{action=Login}/{id?}");
await IdentitySeedData.SeedData(app);
app.Run();
