using System.Text.Json;
using Lextm.SharpSnmpLib;
using Microsoft.EntityFrameworkCore;
using NetDeviceManager.Database;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Services;
using NetDeviceManager.ScheduledSnmpAgent.Helpers;
using Testcontainers.PostgreSql;


namespace NetDeviceManager.DbTests;

public class Tests
{
    private DbContextOptions<ApplicationDbContext> _options;
    private PostgreSqlContainer _postgresContainer;

    [SetUp]
    public async Task SetUp()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .WithHostname("127.0.0.1")
            .WithDatabase("ManagerData")
            .WithUsername("manager")
            .WithPassword("Heslo1234.")
            .Build();

        await _postgresContainer.StartAsync();
    }

    [TearDown]
    public async Task TearDown()
    {
        _postgresContainer.StopAsync();
        _postgresContainer.DisposeAsync();
    }

    [Test]
    public void DbConnection()
    {
        var connectionString = _postgresContainer.GetConnectionString();
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        ApplicationDbContext database = new ApplicationDbContext(_options);
        bool canConnect = database.Database.CanConnect();
        Assert.That(canConnect);
    }

    // [Test]
    // public void AddSnmpRecords()
    // {
    //     var connectionString = ConfigurationHelper.GetConfigurationString();
    //     _options = new DbContextOptionsBuilder<ApplicationDbContext>()
    //         .UseNpgsql(connectionString)
    //         .Options;
    //     ApplicationDbContext database = new ApplicationDbContext(_options);
    //     var dbService = new DatabaseService(database);
    //     var icon = GetDeviceIcon();
    //     var device = GetDevice(icon.Id);
    //     var loginProfile = GetLoginProfile();
    //     var physicalDevice = GetPhysicalDevice(device.Id, loginProfile.Id);
    //     var sensor = GetSnmpSensor();
    //     var multiSensor = GetMultiSnmpSensor();
    //     var snmpSensorInPhysicalDevice = GetSnmpSensorInPhysicalDevice(physicalDevice.Id, sensor.Id);
    //     var snmpRecord = GetSnmpRecord(physicalDevice.Id, sensor.Id);
    //     var multisnmpSensorInPhysicalDevice = GetSnmpSensorInPhysicalDevice(physicalDevice.Id, multiSensor.Id);
    //     var multiSnmpRecord = GetMultiSnmpRecord(physicalDevice.Id, sensor.Id, multiSensor.StartIndex,
    //         multiSensor.EndIndex);
    //
    //     var recordCount = dbService.GetRecordsCount();
    //     var iconId = dbService.AddDeviceIcon(icon);
    //     device.IconId = iconId;
    //     var deviceId = dbService.AddDevice(device);
    //     var loginId = dbService.AddLoginProfile(loginProfile);
    //     physicalDevice.DeviceId = deviceId;
    //     physicalDevice.LoginProfileId = loginId;
    //     var physicalDeviceId = dbService.AddPhysicalDevice(physicalDevice);
    //     var sensorId = dbService.AddSnmpSensor(sensor);
    //     var multiSensorId = dbService.AddSnmpSensor(multiSensor);
    //     snmpSensorInPhysicalDevice.PhysicalDeviceId = physicalDeviceId;
    //     snmpSensorInPhysicalDevice.SnmpSensorId = sensorId;
    //     var sensorInDeviceId = dbService.AddSnmpSensorToPhysicalDevice(snmpSensorInPhysicalDevice);
    //     snmpRecord.SensorId = sensorId;
    //     snmpRecord.PhysicalDeviceId = physicalDeviceId;
    //     dbService.AddSnmpRecord(snmpRecord);
    //     multisnmpSensorInPhysicalDevice.PhysicalDeviceId = physicalDeviceId;
    //     multisnmpSensorInPhysicalDevice.SnmpSensorId = multiSensorId;
    //     var multiSensorInDeviceId = dbService.AddSnmpSensorToPhysicalDevice(multisnmpSensorInPhysicalDevice);
    //     multiSnmpRecord.SensorId = sensorId;
    //     multiSnmpRecord.PhysicalDeviceId = physicalDeviceId;
    //     dbService.AddSnmpRecord(multiSnmpRecord);
    //
    //     var afterRecordCount = dbService.GetRecordsCount();
    //     Assert.AreEqual(2, (afterRecordCount - recordCount));
    // }

    private SnmpSensorRecord GetMultiSnmpRecord(Guid deviceId, Guid sensorId, int start, int end)
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
            PhysicalDeviceId = deviceId,
            SensorId = sensorId
        };
    }

    private SnmpSensorRecord GetSnmpRecord(Guid deviceId, Guid sensorId)
    {
        Random r = new Random();
        return new SnmpSensorRecord
        {
            Id = Guid.NewGuid(),
            Data = JsonSerializer.Serialize(new[] { r.Next(1, 50).ToString() }),
            CapturedTime = DateTime.Now,
            PhysicalDeviceId = deviceId,
            SensorId = sensorId
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

    private PhysicalDevice GetPhysicalDevice()
    {
        Random r = new Random();
        var firstOctet = r.Next(5, 254);
        var secondOctet = r.Next(5, 254);
        var thirdctet = r.Next(5, 254);
        var fourthOctet = r.Next(5, 254);
        return new PhysicalDevice
        {
            Id = Guid.NewGuid(),
            Name = "Rtest.cisco.com",
            Description = null,
            MacAddress = null,
            Platform = "Cisco 2801"
        };
    }

    private LoginProfile GetLoginProfile()
    {
        return new LoginProfile
        {
            Id = Guid.NewGuid(),
            Name = "test"
        };
    }

    private DeviceIcon GetDeviceIcon()
    {
        return new DeviceIcon()
        {
            Id = Guid.NewGuid(), Name = $"TempIcon_{Guid.NewGuid()}"
        };
    }
}