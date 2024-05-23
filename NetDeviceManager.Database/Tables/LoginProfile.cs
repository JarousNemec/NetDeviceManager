using System.ComponentModel.DataAnnotations;

namespace NetDeviceManager.Database.Tables;

public class LoginProfile
{
    public Guid Id { get; set; }
    [Required]public string Name { get; set; }
    public string? Description { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? ConnString { get; set; }
    public string? Key { get; set; }
    public string? SecurityName { get; set; }
    public string? AuthenticationPassword { get; set; }
    public string? PrivacyPassword { get; set; }
}