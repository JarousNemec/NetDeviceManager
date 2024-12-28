using Microsoft.EntityFrameworkCore;
using NetDeviceManager.Database;
using NetDeviceManager.Database.Tables;

using Npgsql;
using Testcontainers.PostgreSql;
using Lextm.SharpSnmpLib;
using Microsoft.Extensions.Hosting;
using Moq;
using NetDeviceManager.Lib.Services;

namespace NetDeviceManager.DbTests;

public class DatabaseServiceTest
{
    private DbContextOptions<ApplicationDbContext> _options;
    private PostgreSqlContainer _postgresContainer;
    private ApplicationDbContext _tested;

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

        var connectionString = _postgresContainer.GetConnectionString();
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        ApplicationDbContext database = new ApplicationDbContext(_options);
        bool canConnect = database.Database.CanConnect();
        Assert.That(canConnect);

        using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "schema.sql");
        var schemaSql = await File.ReadAllTextAsync(filePath);

        // Create schema
        var createTableCmd2 = new NpgsqlCommand(schemaSql, connection);
        await createTableCmd2.ExecuteNonQueryAsync();

        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        _tested = new ApplicationDbContext(_options);
    }

    [TearDown]
    public async Task TearDown()
    {
        _postgresContainer.StopAsync();
        _postgresContainer.DisposeAsync();
        _tested.Dispose();
    }

    [Test]
    public void AddSnmpRecord_ShouldAddRecordAndReturnId()
    {
        
        Guid id = Guid.NewGuid();
        
        
        // Arrange
        var snmpRecord = new SnmpSensor { Id = id,
            Name = "Test"};
        
        // Act
        var returnedId = _tested.SnmpSensors.Add(snmpRecord).Entity.Id;
        

        // Assert
        Assert.That(returnedId, Is.EqualTo(id));

    }
    
    [Test]
    public void TestDatabaseService()
    {
        
        // Arrange
        var databaseService = new DatabaseService(_tested);
        var portService = new PortService(_tested);
        var smtpService = new SnmpService(databaseService, new SettingsService(databaseService));

        var mockEnvironment = new Mock<IHostEnvironment>();
        var fileStorageService = new FileStorageService(databaseService, mockEnvironment.Object);
        
        var deviceService = new DeviceService(databaseService, fileStorageService, portService);

        SnmpSensor sensor = new SnmpSensor
        {
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
        Guid sensorId = Guid.NewGuid();
        var result = smtpService.UpsertSnmpSensor(sensor, out sensorId);

        PhysicalDevice device = new PhysicalDevice
        {
            Id = Guid.NewGuid(),
            Name = "Router-X1000",
            Description = "Main network router",
            MacAddress = "00:1A:2B:3C:4D:5E",
            IconId = null,  // Assuming no icon initially
            Platform = "Cisco IOS",
            Version = "15.1",
            Capabilities = "Routing, Security, QoS",
        };
        
        
        Guid deviceId = Guid.NewGuid();
        var res = deviceService.UpsertPhysicalDevice(device, out deviceId);
        
        // Act
        var id = databaseService.AddSnmpSensorToPhysicalDeviceById(sensorId, sensor, deviceId, device);

        // Assert
        var count = databaseService.GetDeviceSensorsCount(deviceId);
        Assert.That(count, Is.EqualTo(1));
        
    }
    
}