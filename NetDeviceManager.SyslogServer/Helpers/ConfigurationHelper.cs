using System.Configuration;
using NetDeviceManager.Database;

namespace NetDeviceManager.SyslogServer.Helpers;

public static class ConfigurationHelper
{

    public static string? GetConfigurationString()
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (environmentName == "development")
            return ConfigurationManager.AppSettings["DefaultConnection.Development"];
        return ConfigurationManager.AppSettings["DefaultConnection"];
        // return Environment.GetEnvironmentVariable("POSTGRES_CONNSTRING");
    }

    public static string? GetValue(string key)
    {
        return ConfigurationManager.AppSettings[key];
    }
}