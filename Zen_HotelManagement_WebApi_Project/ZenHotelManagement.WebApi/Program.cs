using Microsoft.AspNetCore.HttpOverrides;
using NLog;
using ZenHotelManagement.Contracts;
using ZenHotelManagement.WebApi.Extensions;



var builder = WebApplication.CreateBuilder(args);

LogManager.Setup().LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
// Add services to the container.
builder.Services.AddAutoMapper(typeof(Program));

// Register the background service for cab driver status updates
 builder.Services.AddHostedService<ZenHotelManagement.Service.CabDriverStatusBackgroundService>();

builder.Services.AddControllers().AddApplicationPart(typeof(ZenHotelManagement.Presentation.AssemblyReference).Assembly);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.

var logger = app.Services.GetRequiredService<ILoggerManager>();

app.ConfigureExceptionHandler(logger);
if (app.Environment.IsProduction())
    app.UseHsts();


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.All });

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
