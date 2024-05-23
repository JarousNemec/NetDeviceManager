using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;

namespace NetDeviceManager.Lib.Model;

public class CreateDeviceIconModel
{
    [Required]public string Name { get; set; }
    public string? Description { get; set; }
    [Required]public IBrowserFile File { get; set; }
}