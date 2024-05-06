

using System.Configuration;
using System.Reflection;

namespace NetDeviceManager.ScheduledSnmpAgent.Helpers;

public static class ConfigurationHelper
{

    public static string? GetConfigurationString()
    {
        // var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        // var config = ConfigurationManager.OpenExeConfiguration(Assembly.GetCallingAssembly().Location);
        // if (environmentName.ToLower() == "development")
        //     return ConfigurationManager.AppSettings["DefaultConnection.Development"];
        // return ConfigurationManager.AppSettings["DefaultConnection"];
        return Environment.GetEnvironmentVariable("POSTGRES_CONNSTRING");
    }

    public static string? GetValue(string key)
    {
        return ConfigurationManager.AppSettings[key];
    }
}