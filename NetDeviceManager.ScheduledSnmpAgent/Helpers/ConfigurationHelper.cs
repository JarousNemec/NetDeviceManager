using Microsoft.Extensions.Configuration;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace NetDeviceManager.ScheduledSnmpAgent.Helpers;

public static class ConfigurationHelper
{

    public static string? GetConfigurationString()
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (environmentName == "development")
            return ConfigurationManager.AppSettings["DefaultConnection.Development"];
        return ConfigurationManager.AppSettings["DefaultConnection"];
    }

    public static string? GetValue(string key)
    {
        return ConfigurationManager.AppSettings[key];
    }
}