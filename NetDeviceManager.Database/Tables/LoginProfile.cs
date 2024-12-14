using System.ComponentModel.DataAnnotations;

namespace NetDeviceManager.Database.Tables;

public class LoginProfile
{
    public Guid Id { get; set; }
    [Required]public string Name { get; set; }
    public string? Description { get; set; }
    public string? SnmpUsername { get; set; }
    public string? SnmpSecurityName { get; set; }
    public string? SnmpAuthenticationPassword { get; set; }
    public string? SnmpPrivacyPassword { get; set; }

    public string? SshUsername { get; set; }
    public string? SshPassword { get; set; }
    public string? CiscoPrivilagedModePassword { get; set; }
}