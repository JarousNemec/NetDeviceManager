using Aspose.Zip;
using Microsoft.Extensions.Logging;

namespace NetDeviceManager.Lib.Utils;

public static class FileUtil
{
    public static string ArchiveDirectory(string destination, string target)
    {
        try
        {
            using (FileStream zipFile = File.Open(destination, FileMode.Create))
            {
                using (Archive archive = new Archive())
                {
                    DirectoryInfo dirToArchive = new DirectoryInfo(target);
                    archive.CreateEntries(dirToArchive);
                    archive.Save(zipFile);
                }
            }
        }
        catch (Exception e)
        {
            return e.Message;
        }

        return string.Empty;
    }
}