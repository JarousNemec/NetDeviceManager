using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetDeviceManager.Database;
using NetDeviceManager.Database.Interfaces;
using NetDeviceManager.Database.Services;
using NetDeviceManager.Lib.Services;
using NetDeviceManager.Lib.Snmp.Interfaces;
using NetDeviceManager.ScheduledSnmpAgent;
using NetDeviceManager.ScheduledSnmpAgent.Helpers;
using Timer = System.Timers.Timer;

Console.WriteLine("Initializing...");
HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

var connectionString = ConfigurationHelper.GetConfigurationString();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddSingleton<Scheduler>();
builder.Services.AddSingleton<Timer>();
builder.Services.AddScoped<ISnmpService, SnmpService>();
builder.Services.AddSingleton<IDatabaseService, DatabaseService>();

Console.WriteLine("Initialized!");

var app = builder.Build();
var scheduler = app.Services.GetRequiredService<Scheduler>();
scheduler.Schedule();

Console.WriteLine("Running...");

app.Run();