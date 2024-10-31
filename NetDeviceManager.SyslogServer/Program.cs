// See https://aka.ms/new-console-template for more information

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetDeviceManager.Database;
using NetDeviceManager.Lib.GlobalConstantsAndEnums;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Services;
using NetDeviceManager.SyslogServer;
using NetDeviceManager.SyslogServer.Helpers;

Console.WriteLine("Initializing...");
HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

var connectionString = ConfigurationHelper.GetConfigurationString();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseNpgsql(connectionString);
        options.LogTo(Console.WriteLine, LogLevel.Warning);
    }
);
builder.Services.AddSingleton<ServerCache>();
builder.Services.AddScoped<Server>();
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Logging.SetMinimumLevel(GlobalSettings.MinimalLoggingLevel);
Console.WriteLine("Initialized!");

var app = builder.Build();

Console.WriteLine("Starting receiver...");
var server = app.Services.GetRequiredService<Server>();
server.Run();

_ = app.RunAsync();