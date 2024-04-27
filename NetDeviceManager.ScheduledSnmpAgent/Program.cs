using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NetDeviceManager.ScheduledSnmpAgent;

Console.WriteLine("Initializing...");
Setup();
Console.WriteLine("Initialized!");

void Setup()
{
    var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    var builder = new ConfigurationBuilder()
        .AddJsonFile($"appsettings.json", true, true)
        .AddJsonFile($"appsettings.{environmentName}.json", true, true)
        .AddEnvironmentVariables();
    var configuration = builder.Build();
    var myConnString= configuration.GetConnectionString("DefaultConnection");
}