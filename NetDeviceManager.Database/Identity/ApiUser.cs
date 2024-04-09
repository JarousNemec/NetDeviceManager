using Microsoft.AspNetCore.Identity;

namespace NetDeviceManager.Database.Identity;

public class ApiUser : IdentityUser
{
    public string ApiKey { get; set; }
}