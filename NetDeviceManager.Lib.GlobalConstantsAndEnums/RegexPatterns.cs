namespace NetDeviceManager.Lib.GlobalConstantsAndEnums;

public static class RegexPatterns
{
    public const string CISCO_SYSLOG_HEADER_PATTERN = @"(%\w+-(\d)-\w+)";
    public const string TIMESTAMP_RFC3164_PATTERN = @"((Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec) [ ]?\d{1,2} \d{1,2}:\d{1,2}:\d{1,2})";
    public const string TIMESTAMP_ISO_PATTERN = @"(\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d{1,6})?(Z|([+-]\d{2}:\d{2})))";
    public const string PRIORITY_PATTERN = @"<(\d+)>";
}