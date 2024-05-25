using NetDeviceManager.Lib.Model;

namespace NetDeviceManager.Lib.Services;

public class SettingsService
{
    private readonly DatabaseService _databaseService;

    public SettingsService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public SettingsModel GetSettings()
    {
        var settings = new SettingsModel();
        var DesiredSeverities = _databaseService.GetConfigValue("DesiredSeverities");
        var ReportLogInterval = _databaseService.GetConfigValue("ReportLogInterval");
        var ReportSensorInterval = _databaseService.GetConfigValue("ReportSensorInterval");

        if (expr)
        {
            
        }
        
        return settings;
    }
}