using System.IO.Compression;
using Aspose.Zip;
using Microsoft.Extensions.Logging;

namespace NetDeviceManager.Lib.Utils;

public static class FileUtil
{
    public static void ZipDirectory(string sourceDir, string zipPath)
    {
        if (Directory.Exists(sourceDir))
        {
            try
            {
                // Vytvoření nebo otevření zip souboru
                using (FileStream zipToOpen = new FileStream(zipPath, FileMode.Create))
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(sourceDir);
                    // Rekurzivní přidání souborů a složek do zip archivu
                    AddDirectoryToArchive(archive, dirInfo, dirInfo.FullName);
                }
                Console.WriteLine("Složka byla úspěšně zazipována.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Došlo k chybě při zazipování složky: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Zdrojová složka neexistuje.");
        }
    }

    private static void AddDirectoryToArchive(ZipArchive archive, DirectoryInfo dirInfo, string basePath)
    {
        // Přidání všech souborů v aktuální složce
        foreach (FileInfo file in dirInfo.GetFiles())
        {
            string entryName = Path.GetRelativePath(basePath, file.FullName);
            archive.CreateEntryFromFile(file.FullName, entryName);
        }

        // Rekurzivní přidání podsložek
        foreach (DirectoryInfo subDir in dirInfo.GetDirectories())
        {
            AddDirectoryToArchive(archive, subDir, basePath);
        }
    }
}