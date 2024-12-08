using System.Text;
using Aspose.Zip;
using Microsoft.Extensions.Logging;
using NetDeviceManager.Database.Tables;

namespace NetDeviceManager.Lib.Utils;

public static class FileUtil
{
    public static void ArchiveDirectory(string destination, string source)
    {
        try
        {
            using (FileStream zipFile = File.Open(destination, FileMode.Create))
            {
                using (Archive archive = new Archive())
                {
                    DirectoryInfo dirToArchive = new DirectoryInfo(source);
                    archive.CreateEntries(dirToArchive);
                    archive.Save(zipFile);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public static string PrepareDiskForReportingLogs(string rootPath, string date)
    {
        if (!Directory.Exists(rootPath))
            Directory.CreateDirectory(rootPath);
        var currentdatepath = Path.Combine(rootPath, date);
        return currentdatepath;
    }

    public static string GetArchivePath(string destinationDirectory, string name)
    {
        if (!Directory.Exists(destinationDirectory))
            Directory.CreateDirectory(destinationDirectory);
        string zipPath = Path.Combine(destinationDirectory, $"{name}.zip");
        return zipPath;
    }

    public static string PrepareReportEnvironment(string deviceIdentificator, string currentdatepath, string filename)
    {
        var devicedirpath = Path.Combine(currentdatepath, deviceIdentificator);
        if (!Directory.Exists(devicedirpath))
            Directory.CreateDirectory(devicedirpath);
        var syslogpath = Path.Combine(devicedirpath, filename);
        return syslogpath;
    }

    public static async Task WriteSyslogsToFile(string syslogpath, List<SyslogRecord> syslogs, bool withIp = false)
    {
        var builder = new StringBuilder();
        foreach (var log in syslogs)
        {
            builder.AppendLine($"{(withIp ? log.Ip + ' '+'-'+' ' : string.Empty)}{log.ProcessedDate.ToString("MM.dd.yyyy HH-mm-ss")} - {log
                .CompletMessage}");
        }

        var data = builder.ToString();
        await File.WriteAllTextAsync(syslogpath, data);
    }
}