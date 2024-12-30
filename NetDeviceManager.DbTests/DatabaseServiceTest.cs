using Microsoft.EntityFrameworkCore;
using NetDeviceManager.Database;
using NetDeviceManager.Database.Tables;
using Npgsql;
using Testcontainers.PostgreSql;
using Lextm.SharpSnmpLib;
using Microsoft.Extensions.Hosting;
using Moq;
using NetDeviceManager.DbTests.Helpers;
using NetDeviceManager.DbTests.Models;
using NetDeviceManager.Lib.Services;

namespace NetDeviceManager.DbTests;

public class DatabaseServiceTest
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
    public void AddSnmpRecord_ShouldAddRecordAndReturnId()
    {
        Guid id = Guid.NewGuid();


        // Arrange
        var snmpRecord = new SnmpSensor
        {
            Id = id,
            Name = "Test"
        };

        // Act
        var returnedId = _toolbox.TestedContext.SnmpSensors.Add(snmpRecord).Entity.Id;


        // Assert
        Assert.That(returnedId, Is.EqualTo(id));
    }

    [Test]
    public void TestDatabaseService()
    {
        SnmpSensor sensor = new SnmpSensor
        {
            Id = Guid.Empty,
            Name = "Temperature Sensor",
            Description = "Monitors the temperature of the server room.",
            Oid = "1.3.6.1.2.1.1.1.0",
            SnmpVersion = VersionCode.V2,
            CommunityString = "public",
            IsMulti = false,
            OidFilling = ".",
            StartIndex = 0,
            EndIndex = 0,
            SnmpSensorInDevices = new List<SnmpSensorInPhysicalDevice>()
        };
        var sensorResult = _toolbox.SnmpService.UpsertSnmpSensor(sensor, out Guid sensorId);

        PhysicalDevice device = new PhysicalDevice
        {
            Id = Guid.Empty,
            Name = "Router-X1000",
            Description = "Main network router",
            MacAddress = "00:1A:2B:3C:4D:5E",
            IconId = null, // Assuming no icon initially
            Platform = "Cisco IOS",
            Version = "15.1",
            Capabilities = "Routing, Security, QoS",
        };
        var deviceResult = _toolbox.DeviceService.UpsertPhysicalDevice(device, out Guid deviceId);

        if (!deviceResult.IsSuccessful || !sensorResult.IsSuccessful)
            Assert.Fail();

        // Act
        var relation = new SnmpSensorInPhysicalDevice()
        {
            Id = Guid.Empty,
            PhysicalDeviceId = deviceId,
            SnmpSensorId = sensorId
        };
        var id = _toolbox.DeviceService.AddSnmpSensorToPhysicalDevice(relation);

        // Assert
        var count = _toolbox.DeviceService.GetPhysicalDeviceSensorsCount(deviceId);
        Assert.That(count, Is.EqualTo(1));
    }
}