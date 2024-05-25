namespace NetDeviceManager.Lib.Model;

public class SettingsModel
{
    public string DesiredSeverities { get; set; } = "[\"0\",\"1\",\"2\",\"3\",\"4\"]";
    public string ReportLogInterval { get; set; } = "0 59 23 ? * SUN *";
    public string ReportSensorInterval { get; set; } = "0 0/10 * 1/1 * ? *";
}