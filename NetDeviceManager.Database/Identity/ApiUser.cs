using Microsoft.AspNetCore.Identity;
using NetDeviceManager.Web.Data;

namespace NetDeviceManager.Database.Identity;

public class ApiUser : ApplicationUser
{
    public string ApiKey { get; set; }
}