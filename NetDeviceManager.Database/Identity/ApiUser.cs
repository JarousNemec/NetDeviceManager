using Microsoft.AspNetCore.Identity;

namespace NetDeviceManager.Database.Identity;

public class ApiUser : ApplicationUser
{
    public string ApiKey { get; set; }
}