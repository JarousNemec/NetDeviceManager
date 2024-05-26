using System.Text.Json;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Model;

namespace NetDeviceManager.Lib.Services;

public class SettingsService
{
    private readonly IDatabaseService _databaseService;

    private int[] DESIRED_SEVERITIES = new[] { 0, 1, 2, 3, 4 };
    private const string DESIRED_SEVERITIES_JSON = "[0, 1, 2, 3, 4]";
    private const string REPORT_LOG_INTERVAL = "0 59 23 ? * SUN *";
private const string REPORT_SENSOR_INTERVAL  = "0 0/10 * 1/1 * ? *";

private const string DESIRED_SEVERITIES_KEY = "DesiredSeverities";
private const string REPORT_LOG_INTERVAL_KEY = "ReportLogInterval";
private const string REPORT_SENSOR_INTERVAL_KEY  = "ReportSensorInterval";
    public SettingsService(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public SettingsModel GetSettings()
    {
        var settings = new SettingsModel();
        var jsonSeverities = _databaseService.GetConfigValue(DESIRED_SEVERITIES_KEY);
        if (jsonSeverities != null)
        {
            var severities = JsonSerializer.Deserialize<int[]>(jsonSeverities);
            settings.DesiredSeverities = severities ?? DESIRED_SEVERITIES;
        }
        else
        {
            settings.DesiredSeverities = DESIRED_SEVERITIES;
        }

        settings.ReportLogInterval = _databaseService.GetConfigValue(REPORT_LOG_INTERVAL_KEY) ?? REPORT_LOG_INTERVAL;
        settings.ReportSensorInterval = _databaseService.GetConfigValue(REPORT_SENSOR_INTERVAL_KEY) ?? REPORT_SENSOR_INTERVAL;
        return settings;
    }
    
    public SettingsUpdateModel GetSettingsUpdateModel()
    {
        var settings = new SettingsUpdateModel();
        settings.DesiredSeverities = _databaseService.GetConfigValue(DESIRED_SEVERITIES_KEY) ?? DESIRED_SEVERITIES_JSON;
        settings.ReportLogInterval = _databaseService.GetConfigValue(REPORT_LOG_INTERVAL_KEY) ?? REPORT_LOG_INTERVAL;
        settings.ReportSensorInterval = _databaseService.GetConfigValue(REPORT_SENSOR_INTERVAL_KEY) ?? REPORT_SENSOR_INTERVAL;
        return settings;
    }

    public void UpdateSettings(SettingsUpdateModel settings)
    {
        _databaseService.SetConfigValue(DESIRED_SEVERITIES_KEY, settings.DesiredSeverities);
        _databaseService.SetConfigValue(REPORT_LOG_INTERVAL_KEY, settings.ReportLogInterval);
        _databaseService.SetConfigValue(REPORT_SENSOR_INTERVAL_KEY, settings.ReportSensorInterval);
    }
}