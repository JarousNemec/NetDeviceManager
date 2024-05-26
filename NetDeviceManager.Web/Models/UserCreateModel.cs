using System.ComponentModel.DataAnnotations;

namespace NetDeviceManager.Web.Models;

public class UserCreateModel
{
    [Required] public string Username { get; set; }
    [Required] public string Password { get; set; }
}