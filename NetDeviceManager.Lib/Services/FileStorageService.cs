using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Hosting;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Model;

namespace NetDeviceManager.Lib.Services;

public class FileStorageService : IFileStorageService
{
    private const string WEB_STORAGE_PATH = "webdata";
    private readonly IDatabaseService _database;
    private readonly IHostEnvironment _env;

    public FileStorageService(IDatabaseService database, IHostEnvironment env)
    {
        _database = database;
        _env = env;
    }

    public OperationResult SaveIconFile(Guid iconId, IBrowserFile file)
    {
        var pathdir = Path.Combine(_env.ContentRootPath, WEB_STORAGE_PATH);
        var pathfile = Path.Combine(pathdir, $"{iconId}.{file.Name.Split('.').Last()}");
        try
        {
            if (!Directory.Exists(pathdir))
            {
                Directory.CreateDirectory(pathdir);
            }
            using (var stream = File.Create(pathfile))
            {
                file.OpenReadStream(2000000000).CopyToAsync(stream);
                stream.Flush();
            }
        }
        catch (Exception e)
        {
            return new OperationResult() { IsSuccessful = false, Message = e.Message };
        }

        return new OperationResult();
    }
}