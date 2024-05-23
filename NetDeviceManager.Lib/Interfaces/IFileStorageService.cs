using Microsoft.AspNetCore.Components.Forms;
using NetDeviceManager.Lib.Model;

namespace NetDeviceManager.Lib.Interfaces;

public interface IFileStorageService
{
    Task<OperationResult> SaveIconFile(Guid iconId, IBrowserFile file);
}