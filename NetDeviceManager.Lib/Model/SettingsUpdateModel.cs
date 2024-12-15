namespace NetDeviceManager.Lib.Model;

public class SettingsUpdateModel
{
    public string DesiredSeverities { get; set; } = string.Empty;
    public string ReportLogInterval { get; set; } = string.Empty;
    public string ReportSensorInterval { get; set; } = string.Empty;
}