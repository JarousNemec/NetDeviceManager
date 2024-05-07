using System.Globalization;
using System.Text.Json;
using Lextm.SharpSnmpLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetDeviceManager.Database;
using NetDeviceManager.Database.Services;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Snmp.Models;
using NetDeviceManager.ScheduledSnmpAgent.Helpers;

namespace NetDeviceManager.DbTests;

public class Tests
{
    private DbContextOptions<ApplicationDbContext> _options;

    public Tests()
    {
    }

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void DbConnection()
    {
        var connectionString = ConfigurationHelper.GetConfigurationString();
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        ApplicationDbContext database = new ApplicationDbContext(_options);
        Assert.True(database.Database.CanConnect());
    }

    [Test]
    public void AddSnmpRecords()
    {
        var connectionString = ConfigurationHelper.GetConfigurationString();
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        ApplicationDbContext database = new ApplicationDbContext(_options);
        var dbService = new DatabaseService(database);
        var icon = GetDeviceIcon();
        var device = GetDevice(icon.Id);
        var loginProfile = GetLoginProfile();
        var physicalDevice = GetPhysicalDevice(device.Id, loginProfile.Id);
        var sensor = GetSnmpSensor();
        var multiSensor = GetMultiSnmpSensor();
        var snmpSensorInPhysicalDevice = GetSnmpSensorInPhysicalDevice(physicalDevice.Id, sensor.Id);
        var snmpRecord = GetSnmpRecord(snmpSensorInPhysicalDevice.Id);
        var multisnmpSensorInPhysicalDevice = GetSnmpSensorInPhysicalDevice(physicalDevice.Id, multiSensor.Id);
        var multiSnmpRecord = GetMultiSnmpRecord(multisnmpSensorInPhysicalDevice.Id, multiSensor.StartIndex,
            multiSensor.EndIndex);

        var recordCount = dbService.GetRecordsCount();
        var iconId = dbService.AddDeviceIcon(icon);
        device.IconId = iconId;
        var deviceId = dbService.AddDevice(device);
        var loginId = dbService.AddLoginProfile(loginProfile);
        physicalDevice.DeviceId = deviceId;
        physicalDevice.LoginProfileId = loginId;
        var physicalDeviceId = dbService.AddPhysicalDevice(physicalDevice);
        var sensorId = dbService.AddSnmpSensor(sensor);
        var multiSensorId = dbService.AddSnmpSensor(multiSensor);
        snmpSensorInPhysicalDevice.PhysicalDeviceId = physicalDeviceId;
        snmpSensorInPhysicalDevice.SnmpSensorId = sensorId;
        var sensorInDeviceId = dbService.AddSnmpSensorToPhysicalDevice(snmpSensorInPhysicalDevice);
        snmpRecord.SensorInPhysicalDeviceId = sensorInDeviceId;
        dbService.AddSnmpRecord(snmpRecord);
        multisnmpSensorInPhysicalDevice.PhysicalDeviceId = physicalDeviceId;
        multisnmpSensorInPhysicalDevice.SnmpSensorId = multiSensorId;
        var multiSensorInDeviceId = dbService.AddSnmpSensorToPhysicalDevice(multisnmpSensorInPhysicalDevice);

        multiSnmpRecord.SensorInPhysicalDeviceId = multiSensorInDeviceId;
        dbService.AddSnmpRecord(multiSnmpRecord);

        var afterRecordCount = dbService.GetRecordsCount();
        Assert.AreEqual(2, (afterRecordCount - recordCount));
    }

    private SnmpSensorRecord GetMultiSnmpRecord(Guid snmpSensorInPhysicalDeviceId, int start, int end)
    {
        Random r = new Random();
        var recordTime = DateTime.Now;
        var data = new string[end - start + 1];
        for (int i = start; i <= end; i++)
        {
            data[i - start] = (r.Next(0, 2000) < 200 ? 2 : 1).ToString();
        }

        return new SnmpSensorRecord()
        {
            Id = Guid.NewGuid(),
            CapturedTime = recordTime,
            Data = JsonSerializer.Serialize(data),
            SensorInPhysicalDeviceId = snmpSensorInPhysicalDeviceId
        };
    }

    private SnmpSensorRecord GetSnmpRecord(Guid snmpSensorInPhysicalDeviceId)
    {
        Random r = new Random();
        return new SnmpSensorRecord
        {
            Id = Guid.NewGuid(),
            Data = JsonSerializer.Serialize(new[] { r.Next(1, 50).ToString() }),
            CapturedTime = DateTime.Now,
            SensorInPhysicalDeviceId = snmpSensorInPhysicalDeviceId
        };
    }

    private SnmpSensorInPhysicalDevice GetSnmpSensorInPhysicalDevice(Guid deviceId, Guid sensorId)
    {
        return new SnmpSensorInPhysicalDevice
        {
            Id = Guid.NewGuid(),
            PhysicalDeviceId = deviceId,
            SnmpSensorId = sensorId
        };
    }

    private SnmpSensor GetSnmpSensor()
    {
        Random r = new Random();
        return new SnmpSensor
        {
            Id = Guid.NewGuid(),
            Name = $"tempsensor-{Guid.NewGuid()}",
            Description = null,
            Oid =
                $".1.3.6.1.4.1.9.9.87.{r.Next(0, 99)}.{r.Next(0, 99)}.{r.Next(0, 99)}.{r.Next(0, 99)}.{r.Next(0, 99)}",
            SnmpVersion = VersionCode.V2,
            CommunityString = "public",
            IsMulti = false,
            StartIndex = 0,
            EndIndex = 0,
        };
    }

    private SnmpSensor GetMultiSnmpSensor()
    {
        Random r = new Random();
        return new SnmpSensor
        {
            Id = Guid.NewGuid(),
            Name = $"tempsensor-{Guid.NewGuid()}",
            Description = null,
            Oid =
                $".1.3.6.1.4.1.9.9.87.{r.Next(0, 99)}.{r.Next(0, 99)}.{r.Next(0, 99)}.{r.Next(0, 99)}.{r.Next(0, 99)}",
            SnmpVersion = VersionCode.V2,
            CommunityString = "public",
            IsMulti = true,
            StartIndex = 0,
            EndIndex = 15,
        };
    }

    private PhysicalDevice GetPhysicalDevice(Guid deviceId, Guid profileId)
    {
        Random r = new Random();
        var firstOctet = r.Next(5, 254);
        var secondOctet = r.Next(5, 254);
        var thirdctet = r.Next(5, 254);
        var fourthOctet = r.Next(5, 254);
        return new PhysicalDevice
        {
            Id = Guid.NewGuid(),
            Name = "test",
            Description = null,
            IpAddress = $"{firstOctet}.{secondOctet}.{thirdctet}.{fourthOctet}",
            MacAddress = null,
            DeviceId = deviceId,
            LoginProfileId = profileId
        };
    }

    private LoginProfile GetLoginProfile()
    {
        return new LoginProfile
        {
            Id = Guid.NewGuid(),
            Name = "test",
            Description = null,
            Username = null,
            Password = null,
            ConnString = null,
            Key = null,
            SecurityName = null,
            AuthenticationPassword = null,
            PrivacyPassword = null
        };
    }

    private DeviceIcon GetDeviceIcon()
    {
        return new DeviceIcon()
        {
            Id = Guid.NewGuid(), Name = $"TempIcon_{Guid.NewGuid()}", FilePath = ""
        };
    }

    private Device GetDevice(Guid iconId)
    {
        return new Device
        {
            Id = Guid.NewGuid(),
            Name = "best tisco switch",
            Model = "best",
            Description = null,
            Brand = "tisco",
            IconId = iconId
        };
    }
}