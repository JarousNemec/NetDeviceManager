using FluentScheduler;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetDeviceManager.Database;
using NetDeviceManager.ScheduledSnmpAgent;
using Timer = System.Timers.Timer;

Console.WriteLine("Initializing...");
HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var confBuilder = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.json", true, true)
    .AddJsonFile($"appsettings.{environmentName}.json", true, true)
    .AddEnvironmentVariables();
var configuration = confBuilder.Build();
var connectionString = configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddSingleton<Scheduler>();
builder.Services.AddSingleton<Timer>();

// builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
Console.WriteLine("Initialized!");

var app = builder.Build();
var scheduler = app.Services.GetRequiredService<Scheduler>();
scheduler.Schedule();
Console.WriteLine("Running...");

app.Run();