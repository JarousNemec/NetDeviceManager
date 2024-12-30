using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using NetDeviceManager.Database;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.DbTests.Helpers;
using NetDeviceManager.DbTests.Models;
using NetDeviceManager.Lib.Helpers;
using NetDeviceManager.Lib.Utils;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;
using Assert = NUnit.Framework.Assert;

namespace NetDeviceManager.DbTests;

public class LoginProfileEncryptionTest
{
    private DatabaseTestToolbox _toolbox;

    [SetUp]
    public async Task SetUp()
    {
        _toolbox = await TestEnvironmentHelper.SetUpDatabaseToolBox();
    }

    [TearDown]
    public async Task TearDown()
    {
        await _toolbox.DisposeAsync();
    }

    [Test]
    public void DesAlgorithmTest()
    {
        //given
        var data = "Hello World";
        var key = Guid.NewGuid().ToString();

        //when
        var encrypted = DesAlgorithm.Encrypt(data, key);
        Debug.WriteLine(encrypted);

        var decrypted = DesAlgorithm.Decrypt(encrypted, key);
        Debug.WriteLine(decrypted);

        //than
        Assert.That(decrypted, Is.EqualTo(data));
    }

    [Test]
    public void LoginProfileObfuscateTest()
    {
        //given
        var profile = new LoginProfile()
        {
            Id = Guid.NewGuid(),
            Description = "test",
            Name = "testName",
            SnmpUsername = "testSnmpUsername",
            SshPassword = "testSshPassword",
            SshUsername = "testSshUsername",
            SnmpAuthenticationPassword = "testSnmpAuthenticationPassword",
            SnmpPrivacyPassword = "testSnmpPrivacyPassword",
            SnmpSecurityName = "testSnmpSecurityName",
            CiscoPrivilagedModePassword = "testCiscoPrivilagedModePassword"
        };

        //when
        var encrypted = ObfuscationHelper.ObfuscateLoginProfile(profile);
        var decrypted = ObfuscationHelper.DeobfuscateLoginProfile(encrypted);

        Assert.That(decrypted.Id, Is.EqualTo(profile.Id));
        Assert.That(decrypted.Name, Is.EqualTo(profile.Name));
        Assert.That(decrypted.Description, Is.EqualTo(profile.Description));
        Assert.That(decrypted.SnmpUsername, Is.EqualTo(profile.SnmpUsername));
        Assert.That(decrypted.SnmpSecurityName, Is.EqualTo(profile.SnmpSecurityName));
        Assert.That(decrypted.SnmpAuthenticationPassword, Is.EqualTo(profile.SnmpAuthenticationPassword));
        Assert.That(decrypted.SnmpPrivacyPassword, Is.EqualTo(profile.SnmpPrivacyPassword));
        Assert.That(decrypted.SshUsername, Is.EqualTo(profile.SshUsername));
        Assert.That(decrypted.SshPassword, Is.EqualTo(profile.SshPassword));
        Assert.That(decrypted.CiscoPrivilagedModePassword, Is.EqualTo(profile.CiscoPrivilagedModePassword));
    }

    [Test]
    public void LoginProfileSaveAndLoadTest()
    {
        //given
        var profile = new LoginProfile()
        {
            Id = Guid.Empty,
            Description = "test",
            Name = "testName",
            SnmpUsername = "testSnmpUsername",
            SshPassword = "testSshPassword",
            SshUsername = "testSshUsername",
            SnmpAuthenticationPassword = "testSnmpAuthenticationPassword",
            SnmpPrivacyPassword = "testSnmpPrivacyPassword",
            SnmpSecurityName = "testSnmpSecurityName",
            CiscoPrivilagedModePassword = "testCiscoPrivilagedModePassword"
        };

        //when
        _toolbox.LoginProfileService.AddLoginProfile(profile);
        var queried = _toolbox.LoginProfileService.GetLoginProfile(profile.Id);

        Assert.That(queried, Is.Not.Null);
        Assert.That(queried.Id, Is.EqualTo(profile.Id));
        Assert.That(queried.Name, Is.EqualTo(profile.Name));
        Assert.That(queried.Description, Is.EqualTo(profile.Description));
        Assert.That(queried.SnmpUsername, Is.EqualTo(profile.SnmpUsername));
        Assert.That(queried.SnmpSecurityName, Is.EqualTo(profile.SnmpSecurityName));
        Assert.That(queried.SnmpAuthenticationPassword, Is.EqualTo(profile.SnmpAuthenticationPassword));
        Assert.That(queried.SnmpPrivacyPassword, Is.EqualTo(profile.SnmpPrivacyPassword));
        Assert.That(queried.SshUsername, Is.EqualTo(profile.SshUsername));
        Assert.That(queried.SshPassword, Is.EqualTo(profile.SshPassword));
        Assert.That(queried.CiscoPrivilagedModePassword, Is.EqualTo(profile.CiscoPrivilagedModePassword));
    }

    [Test]
    public void LoginProfileAndDeviceRelationWithEncryptionTest()
    {
        //given
        var profile = new LoginProfile()
        {
            Id = Guid.Empty,
            Description = "test",
            Name = "testName",
            SnmpUsername = "testSnmpUsername",
            SshPassword = "testSshPassword",
            SshUsername = "testSshUsername",
            SnmpAuthenticationPassword = "testSnmpAuthenticationPassword",
            SnmpPrivacyPassword = "testSnmpPrivacyPassword",
            SnmpSecurityName = "testSnmpSecurityName",
            CiscoPrivilagedModePassword = "testCiscoPrivilagedModePassword"
        };

        var device = new PhysicalDevice()
        {
            Id = Guid.Empty,
            Name = "testName",
            Platform = "testPlatform",
        };

        //when
        var profileId = _toolbox.LoginProfileService.AddLoginProfile(profile);
        var deviceId = _toolbox.DeviceService.AddPhysicalDevice(device);
        var relation = new LoginProfileToPhysicalDevice()
        {
            Id = Guid.NewGuid(),
            LoginProfileId = profileId,
            PhysicalDeviceId = deviceId,
        };
        _toolbox.LoginProfileService.AssignLoginProfileToPhysicalDevice(relation);
        var queried = _toolbox.LoginProfileService.GetPhysicalDeviceLoginProfileRelationships(device.Id)[0];

        Assert.That(queried, Is.Not.Null);
        Assert.That(queried.LoginProfile, Is.Not.Null);
        Assert.That(queried.LoginProfile.Id, Is.EqualTo(profile.Id));
        Assert.That(queried.LoginProfile.Name, Is.EqualTo(profile.Name));
        Assert.That(queried.LoginProfile.Description, Is.EqualTo(profile.Description));
        Assert.That(queried.LoginProfile.SnmpUsername, Is.EqualTo(profile.SnmpUsername));
        Assert.That(queried.LoginProfile.SnmpSecurityName, Is.EqualTo(profile.SnmpSecurityName));
        Assert.That(queried.LoginProfile.SnmpAuthenticationPassword, Is.EqualTo(profile.SnmpAuthenticationPassword));
        Assert.That(queried.LoginProfile.SnmpPrivacyPassword, Is.EqualTo(profile.SnmpPrivacyPassword));
        Assert.That(queried.LoginProfile.SshUsername, Is.EqualTo(profile.SshUsername));
        Assert.That(queried.LoginProfile.SshPassword, Is.EqualTo(profile.SshPassword));
        Assert.That(queried.LoginProfile.CiscoPrivilagedModePassword, Is.EqualTo(profile.CiscoPrivilagedModePassword));
    }
}