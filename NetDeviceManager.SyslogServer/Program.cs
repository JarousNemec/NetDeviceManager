// See https://aka.ms/new-console-template for more information

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetDeviceManager.Database;
using NetDeviceManager.Database.Interfaces;
using NetDeviceManager.Database.Services;
using NetDeviceManager.SyslogServer;
using NetDeviceManager.SyslogServer.Helpers;

Console.WriteLine("Initializing...");
HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

var connectionString = ConfigurationHelper.GetConfigurationString();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddSingleton<MessageReceiver>();
builder.Services.AddSingleton<MessageProcessor>();
builder.Services.AddSingleton<ServerCache>();
builder.Services.AddSingleton<IDatabaseService, DatabaseService>();

Console.WriteLine("Initialized!");

var app = builder.Build();

Console.WriteLine("Starting receiver...");
var receiver = app.Services.GetRequiredService<MessageReceiver>();
_ = Task.Run(() => receiver.Run());

Console.WriteLine("Starting processor...");
var processor = app.Services.GetRequiredService<MessageProcessor>();
_ = Task.Run(() => processor.Run());

Console.WriteLine("Running...");

app.Run();