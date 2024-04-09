namespace NetDeviceManager.Lib.GlobalConstantsAndEnums;

public enum SyslogFacility
{
    Kernel = 0,
    UserLevel = 1,
    MailSystem = 2,
    SystemDaemons = 3,
    SecurityAuthorizationMessages1 = 4,
    MessagesBySyslogd = 5,
    LinePrinterSubsystem = 6,
    NetworkNewsSubsystem = 7,
    UUCPSubsystem = 8,
    ClockDaemon1 = 9,
    SecurityAuthorizationMessages2 = 10,
    FTPDaemon = 11,
    NTPSubsystem = 12,
    LogAudit = 13,
    LogAlert = 14,
    ClockDaemon2 = 15,
    LocalUse0 = 16,
    LocalUse1 = 17,
    LocalUse2 = 18,
    LocalUse3 = 19,
    LocalUse4 = 20,
    LocalUse5 = 21,
    LocalUse6 = 22,
    LocalUse7 = 23,
}

public static class SyslogFacilityMapper
{
    public static Dictionary<SyslogFacility, string> FacilityToLabels { get; private set; }
    public static Dictionary<string, SyslogFacility> LabelsToFacility { get; private set; }

    static SyslogFacilityMapper()
    {
        FacilityToLabels = new Dictionary<SyslogFacility, string>();
        LabelsToFacility = new Dictionary<string, SyslogFacility>();
        
        Array enumValueArray = Enum.GetValues(typeof(SyslogFacility));  
        foreach (int enumValue in enumValueArray)
        {
            string? label = Enum.GetName(typeof(SyslogFacility), enumValue);
            if(string.IsNullOrEmpty(label))
                continue;
            
            FacilityToLabels.Add((SyslogFacility)enumValue, label);
            LabelsToFacility.Add(label, (SyslogFacility)enumValue);
        }  
    }
}