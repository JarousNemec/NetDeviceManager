using System.Globalization;
using System.Text.RegularExpressions;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.GlobalConstantsAndEnums;

namespace NetDeviceManager.Lib.Utils;

public static class SyslogUtil
{
    
    public static SyslogFormat IdentifySyslogFormat(string message)
    {
        SyslogFormat syslogFormat = SyslogFormat.Unknown;

        if (Regex.IsMatch(message,RegexPatterns.TIMESTAMP_RFC3164_PATTERN) && Regex.IsMatch(message, RegexPatterns.CISCO_SYSLOG_HEADER_PATTERN))
            syslogFormat = SyslogFormat.Cisco;
        else if (Regex.IsMatch(message, RegexPatterns.TIMESTAMP_RFC3164_PATTERN))
            syslogFormat = SyslogFormat.BSD;
        else if (Regex.IsMatch(message, RegexPatterns.TIMESTAMP_ISO_PATTERN))
            syslogFormat = SyslogFormat.IETF;

        return syslogFormat;
    }

    public static DateTime GetSyslogTimestamp(string message, SyslogFormat syslogFormat)
    {
        var m_rfc3164 = Regex.Match(message, RegexPatterns.TIMESTAMP_RFC3164_PATTERN);
        var m_iso = Regex.Match(message, RegexPatterns.TIMESTAMP_ISO_PATTERN);
        string value = m_iso.Success ? m_iso.Value : (m_rfc3164.Success ? m_rfc3164.Value : DateTime.MinValue.ToString(CultureInfo.InvariantCulture));
        if(syslogFormat == SyslogFormat.Cisco || syslogFormat == SyslogFormat.BSD)
            return ParseRfc3164Timestamp(value);
        _ = DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt);
        return dt;
        
    }

    public static int GetSyslogPriority(string message)
    {
        string value = Regex.Match(message, RegexPatterns.PRIORITY_PATTERN).Groups[1].Value;
        int priority = int.MinValue;
        int.TryParse(value, out priority);
        return priority;
    }

    public static SyslogSeverity ParseCiscoSyslogSeverity(string message)
    {
        string value = Regex.Match(message, RegexPatterns.PRIORITY_PATTERN).Groups[1].Value;
        SyslogSeverity severity = SyslogSeverity.Undefined;
        _ = Enum.TryParse(value, out severity);
        return severity;
    }
    
    public static DateTime ParseRfc3164Timestamp(string timestamp)
    {
        // Assumes the timestamp is in the current year
        string year = DateTime.Now.Year.ToString();
        string format = "MMM dd HH:mm:ss yyyy";

        // Create the complete date string by appending the year
        string fullTimestamp = timestamp + " " + year;

        // Parse the date string into a DateTime object
        if (DateTime.TryParseExact(fullTimestamp, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
        {
            return result;
        }
        return DateTime.MinValue;
    }
    
    public static List<SyslogRecord> FilterOlderSyslogs(List<SyslogRecord> syslogs, DateTime sinceDate, int days)
    {
        return syslogs.Where(x => x.ProcessedDate.Ticks < sinceDate.AddDays(-days).Ticks).ToList();
    }
}