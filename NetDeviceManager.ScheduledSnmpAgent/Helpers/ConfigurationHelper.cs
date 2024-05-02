using Microsoft.Extensions.Configuration;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace NetDeviceManager.ScheduledSnmpAgent.Helpers;

public static class ConfigurationHelper
{
    private static IConfigurationRoot _configuration;

    static ConfigurationHelper()
    {
        // var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        // Console.Out.WriteLine(Directory.GetCurrentDirectory());
        // var confBuilder = new ConfigurationBuilder()
        //     .SetBasePath(Directory.GetCurrentDirectory())
        //     .AddJsonFile($"appsettings.json", true, true)
        //     .AddJsonFile($"appsettings.{environmentName}.json", true, true)
        //     .AddEnvironmentVariables();
        // _configuration = confBuilder.Build();
    }

    public static string? GetConfigurationString()
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (environmentName == "development")
            return ConfigurationManager.AppSettings["DefaultConnection.Development"];
        return ConfigurationManager.AppSettings["DefaultConnection"];
        // return _configuration.GetConnectionString(name);
    }

    public static string? GetValue(string key)
    {
        return ConfigurationManager.AppSettings[key];
        // return _configuration.GetValue<string>(key);
    }
}