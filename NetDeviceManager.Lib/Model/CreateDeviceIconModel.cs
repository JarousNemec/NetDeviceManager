using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;

namespace NetDeviceManager.Lib.Model;

public class CreateDeviceIconModel
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public IBrowserFile File { get; set; }
}