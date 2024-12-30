using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetDeviceManager.Database;
using NetDeviceManager.Lib.Facades;
using NetDeviceManager.Lib.GlobalConstantsAndEnums;
using NetDeviceManager.Lib.Helpers;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Services;
using NetDeviceManager.ScheduledSnmpAgent;
using Timer = System.Timers.Timer;

//todo zamyslet se nad novejsi knihovnou pro snmp

Console.WriteLine("Initializing...");
HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

var connectionString = SystemConfigurationHelper.GetConnectionString();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseNpgsql(connectionString);
        options.LogTo(Console.WriteLine, LogLevel.Warning);
    }
);

builder.Services.AddSingleton<Scheduler>();
builder.Services.AddSingleton<Timer>();

builder.Services.AddScoped<ISnmpService, SnmpServiceFacade>();
builder.Services.AddScoped<SnmpService>();

builder.Services.AddScoped<IDatabaseService, DatabaseServiceFacade>();
builder.Services.AddScoped<DatabaseService>();
builder.Logging.SetMinimumLevel(GlobalSettings.MinimalLoggingLevel);
Console.WriteLine("Initialized!");

var app = builder.Build();
var scheduler = app.Services.GetRequiredService<Scheduler>();
scheduler.Schedule();

Console.WriteLine("Running...");

app.Run();