using System.Security.Cryptography;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Utils;

namespace NetDeviceManager.Lib.Helpers;

public static class ObfuscationHelper
{
    public static LoginProfile ObfuscateLoginProfile(LoginProfile loginProfile)
    {
        var output = new LoginProfile
        {
            Id = loginProfile.Id,
            Name = DesAlgorithm.Encrypt(loginProfile.Name, loginProfile.Id.ToString())
        };
        if (loginProfile.Description != null)
            output.Description = DesAlgorithm.Encrypt(loginProfile.Description, loginProfile.Id.ToString());
        if (loginProfile.SshUsername != null)
            output.SshUsername = DesAlgorithm.Encrypt(loginProfile.SshUsername, loginProfile.Id.ToString());
        if (loginProfile.SshPassword != null)
            output.SshPassword = DesAlgorithm.Encrypt(loginProfile.SshPassword, loginProfile.Id.ToString());
        if (loginProfile.SnmpUsername != null)
            output.SnmpUsername = DesAlgorithm.Encrypt(loginProfile.SnmpUsername, loginProfile.Id.ToString());
        if (loginProfile.SnmpAuthenticationPassword != null)
            output.SnmpAuthenticationPassword =
                DesAlgorithm.Encrypt(loginProfile.SnmpAuthenticationPassword, loginProfile.Id.ToString());
        if (loginProfile.SnmpPrivacyPassword != null)
            output.SnmpPrivacyPassword =
                DesAlgorithm.Encrypt(loginProfile.SnmpPrivacyPassword, loginProfile.Id.ToString());
        if (loginProfile.SnmpSecurityName != null)
            output.SnmpSecurityName =
                DesAlgorithm.Encrypt(loginProfile.SnmpSecurityName, loginProfile.Id.ToString());
        if (loginProfile.CiscoPrivilagedModePassword != null)
            output.CiscoPrivilagedModePassword = DesAlgorithm.Encrypt(loginProfile.CiscoPrivilagedModePassword,
                loginProfile.Id.ToString());
        
        return output;
    }

    public static LoginProfile DeobfuscateLoginProfile(LoginProfile loginProfile)
    {
        var output = new LoginProfile()
        {
            Id = loginProfile.Id,
            Name = DesAlgorithm.Decrypt(loginProfile.Name, loginProfile.Id.ToString())
        };
        if (loginProfile.Description != null)
            output.Description = DesAlgorithm.Decrypt(loginProfile.Description, loginProfile.Id.ToString());
        if (loginProfile.SshUsername != null)
            output.SshUsername = DesAlgorithm.Decrypt(loginProfile.SshUsername, loginProfile.Id.ToString());
        if (loginProfile.SshPassword != null)
            output.SshPassword = DesAlgorithm.Decrypt(loginProfile.SshPassword, loginProfile.Id.ToString());
        if (loginProfile.SnmpUsername != null)
            output.SnmpUsername = DesAlgorithm.Decrypt(loginProfile.SnmpUsername, loginProfile.Id.ToString());
        if (loginProfile.SnmpAuthenticationPassword != null)
            output.SnmpAuthenticationPassword =
                DesAlgorithm.Decrypt(loginProfile.SnmpAuthenticationPassword, loginProfile.Id.ToString());
        if (loginProfile.SnmpPrivacyPassword != null)
            output.SnmpPrivacyPassword =
                DesAlgorithm.Decrypt(loginProfile.SnmpPrivacyPassword, loginProfile.Id.ToString());
        if (loginProfile.SnmpSecurityName != null)
            output.SnmpSecurityName =
                DesAlgorithm.Decrypt(loginProfile.SnmpSecurityName, loginProfile.Id.ToString());
        if (loginProfile.CiscoPrivilagedModePassword != null)
            output.CiscoPrivilagedModePassword = DesAlgorithm.Decrypt(loginProfile.CiscoPrivilagedModePassword,
                loginProfile.Id.ToString());
        return output;
    }
}