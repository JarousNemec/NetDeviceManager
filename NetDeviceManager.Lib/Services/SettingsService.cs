using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetDeviceManager.Database;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Model;
using NetDeviceManager.Lib.Utils;

namespace NetDeviceManager.Lib.Services;

public class SettingsService : ISettingsService
{
    private readonly ApplicationDbContext _database;

    private int[] DESIRED_SEVERITIES = new[] { 0, 1, 2, 3, 4 };
    private const string DESIRED_SEVERITIES_JSON = "[0, 1, 2, 3, 4]";
    private const string REPORT_LOG_INTERVAL = "0 59 23 ? * SUN *";
    private const string REPORT_SENSOR_INTERVAL = "0 0/10 * 1/1 * ? *";

    private const string DESIRED_SEVERITIES_KEY = "DesiredSeverities";
    private const string REPORT_LOG_INTERVAL_KEY = "ReportLogInterval";
    private const string REPORT_SENSOR_INTERVAL_KEY = "ReportSensorInterval";

    public SettingsService( ApplicationDbContext database)
    {
        _database = database;
    }

    public SettingsModel GetSettings()
    {
        var settings = new SettingsModel();
        var jsonSeverities = GetConfigValue(DESIRED_SEVERITIES_KEY);
        if (jsonSeverities != null)
        {
            var severities = JsonSerializer.Deserialize<int[]>(jsonSeverities);
            settings.DesiredSeverities = severities ?? DESIRED_SEVERITIES;
        }
        else
        {
            settings.DesiredSeverities = DESIRED_SEVERITIES;
        }

        settings.ReportLogInterval = GetConfigValue(REPORT_LOG_INTERVAL_KEY) ?? REPORT_LOG_INTERVAL;
        settings.ReportSensorInterval = GetConfigValue(REPORT_SENSOR_INTERVAL_KEY) ?? REPORT_SENSOR_INTERVAL;
        return settings;
    }

    public SettingsUpdateModel GetSettingsUpdateModel()
    {
        var settings = new SettingsUpdateModel();
        settings.DesiredSeverities = GetConfigValue(DESIRED_SEVERITIES_KEY) ?? DESIRED_SEVERITIES_JSON;
        settings.ReportLogInterval = GetConfigValue(REPORT_LOG_INTERVAL_KEY) ?? REPORT_LOG_INTERVAL;
        settings.ReportSensorInterval = GetConfigValue(REPORT_SENSOR_INTERVAL_KEY) ?? REPORT_SENSOR_INTERVAL;
        return settings;
    }

    public void UpdateSettings(SettingsUpdateModel settings)
    {
        SetConfigValue(DESIRED_SEVERITIES_KEY, settings.DesiredSeverities);
        SetConfigValue(REPORT_LOG_INTERVAL_KEY, settings.ReportLogInterval);
        SetConfigValue(REPORT_SENSOR_INTERVAL_KEY, settings.ReportSensorInterval);
    }

    public void SetConfigValue(string key, string value)
    {
        var record = _database.Settings.FirstOrDefault(x => x.Key == key);
        if (record != null)
        {
            record.Value = value;
            _database.Settings.Update(record);
            _database.SaveChanges();
            return;
        }

        var newRecord = new Setting
        {
            Id = DatabaseUtil.GenerateId(),
            Key = key,
            Value = value
        };
        _database.Settings.Add(newRecord);
        _database.SaveChanges();
    }

    public string? GetConfigValue(string key)
    {
        return _database.Settings.AsNoTracking().FirstOrDefault(x => x.Key == key)?.Value;
    }
}