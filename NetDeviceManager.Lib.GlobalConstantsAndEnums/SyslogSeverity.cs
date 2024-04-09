namespace NetDeviceManager.Lib.GlobalConstantsAndEnums;

public enum SyslogSeverity
{
    Emergency = 0,
    Alert = 1,
    Critical = 2,
    Error = 3,
    Warning = 4,
    Notice = 5,
    Informational = 6,
    Debug = 7,
}

public static class SyslogSeverityMapper
{
    public static Dictionary<SyslogSeverity, string> SeverityToLabels { get; private set; }
    public static Dictionary<string, SyslogSeverity> LabelsToSeverity { get; private set; }

    static SyslogSeverityMapper()
    {
        SeverityToLabels = new Dictionary<SyslogSeverity, string>();
        LabelsToSeverity = new Dictionary<string, SyslogSeverity>();
        
        Array enumValueArray = Enum.GetValues(typeof(SyslogSeverity));  
        foreach (int enumValue in enumValueArray)
        {
            string? label = Enum.GetName(typeof(SyslogSeverity), enumValue);
            if(string.IsNullOrEmpty(label))
                continue;
            
            SeverityToLabels.Add((SyslogSeverity)enumValue, label);
            LabelsToSeverity.Add(label, (SyslogSeverity)enumValue);
        }  
    }
}