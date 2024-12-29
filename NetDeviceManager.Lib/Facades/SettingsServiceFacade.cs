using Microsoft.Extensions.Logging;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Model;
using NetDeviceManager.Lib.Services;

namespace NetDeviceManager.Lib.Facades;

public class SettingsServiceFacade (SettingsService settingsService, ILogger<SettingsService> logger) : ISettingsService
{
    public void SetConfigValue(string key, string value)
    {
        settingsService.SetConfigValue(key, value);
        logger.LogInformation($"SetConfigValue: {key}={value}");
    }

    public string? GetConfigValue(string key)
    {
        var result = settingsService.GetConfigValue(key);
        logger.LogInformation($"GetConfigValue: {key}={result}");
        return result;
    }

    public void UpdateSettings(SettingsUpdateModel settings)
    {
        settingsService.UpdateSettings(settings);
        logger.LogInformation($"UpdateSettings: DesiredSeverities: {settings.DesiredSeverities}, ReportLogInterval: {settings.ReportLogInterval}, ReportSensorInterval: {settings.ReportSensorInterval}");
    }

    public SettingsUpdateModel GetSettingsUpdateModel()
    {
        var result = settingsService.GetSettingsUpdateModel();
        logger.LogInformation($"GetSettingsUpdateModel: DesiredSeverities: {result.DesiredSeverities}, ReportLogInterval: {result.ReportLogInterval}, ReportSensorInterval: {result.ReportSensorInterval}");
        return result;
    }

    public SettingsModel GetSettings()
    {
        var result = settingsService.GetSettings();
        logger.LogInformation($"GetSettings: {result}");
        return result;
    }
}