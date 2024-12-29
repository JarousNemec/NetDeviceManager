using Microsoft.AspNetCore.Components.Forms;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Model;

namespace NetDeviceManager.Lib.Services;

public class FileStorageService : IFileStorageService
{
    private const string WEB_STORAGE_PATH = "webdata";

    public async Task<OperationResult> SaveIconFile(Guid iconId, IBrowserFile file)
    {
        var pathdir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, WEB_STORAGE_PATH);
        var pathfile = Path.Combine(pathdir, $"{iconId}.{file.Name.Split('.').Last()}");
        try
        {
            if (!Directory.Exists(pathdir))
            {
                Directory.CreateDirectory(pathdir);
            }
            using (var stream = File.Create(pathfile))
            {
                await file.OpenReadStream(2000000000).CopyToAsync(stream);
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