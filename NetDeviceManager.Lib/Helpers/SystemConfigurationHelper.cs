using System.Configuration;
namespace NetDeviceManager.Lib.Helpers;

public static class SystemConfigurationHelper
{
    public static string? GetConnectionString()
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (environmentName == "development")
            return ConfigurationManager.AppSettings["DefaultConnection.Development"];
        return ConfigurationManager.AppSettings["DefaultConnection"];
    }
    
    public static string? GetPath()
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (environmentName == "development")
            return ConfigurationManager.AppSettings["path.Development"];
        return ConfigurationManager.AppSettings["path"];
    }
    public static string? GetValue(string key)
    {
        return ConfigurationManager.AppSettings[key];
    }
}