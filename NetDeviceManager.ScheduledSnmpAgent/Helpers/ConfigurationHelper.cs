using Microsoft.Extensions.Configuration;

namespace NetDeviceManager.ScheduledSnmpAgent.Helpers;

public static class ConfigurationHelper
{
    private static IConfigurationRoot _configuration;
    static ConfigurationHelper()
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var confBuilder = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json", true, true)
            .AddJsonFile($"appsettings.{environmentName}.json", true, true)
            .AddEnvironmentVariables();
        _configuration = confBuilder.Build();
    }

    public static string? GetConfigurationString(string name)
    {
        return _configuration.GetConnectionString(name);
    }

    public static string? GetValue(string key)
    {
        return _configuration.GetValue<string>(key);
    }
}