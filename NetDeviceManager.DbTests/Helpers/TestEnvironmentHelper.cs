using Microsoft.EntityFrameworkCore;
using NetDeviceManager.Database;
using NetDeviceManager.DbTests.Models;
using NetDeviceManager.Lib.Facades;
using NetDeviceManager.Lib.Services;
using Npgsql;
using Testcontainers.PostgreSql;

namespace NetDeviceManager.DbTests.Helpers;

public static class TestEnvironmentHelper
{
    public static async Task<DatabaseTestToolbox> SetUpDatabaseToolBox(bool createSchema = true)
    {
        var toolbox = new DatabaseTestToolbox();

        var connectionString = await SetUpPostgresSqlContainer(toolbox);
        if (createSchema)
        {
            await SetUpDatabaseSchema(connectionString);
            SetUpDatabaseContext(toolbox, connectionString);

            toolbox.DatabaseService = new DatabaseService(toolbox.TestedContext);
            toolbox.SettingsService = new SettingsService(toolbox.TestedContext);
            toolbox.DeviceService =
                new DeviceService(toolbox.TestedContext, toolbox.DatabaseService, toolbox.SettingsService);
            toolbox.FileStorageService = new FileStorageService();
            toolbox.LoginProfileService = new LoginProfileService(toolbox.TestedContext);
            toolbox.PortService = new PortService(toolbox.TestedContext);
            toolbox.SnmpService = new SnmpService(toolbox.TestedContext);
            toolbox.SyslogService = new SyslogService(toolbox.TestedContext);
        }

        return toolbox;
    }

    private static void SetUpDatabaseContext(DatabaseTestToolbox toolbox, string connectionString)
    {
        toolbox.Options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        toolbox.TestedContext = new ApplicationDbContext(toolbox.Options);
    }

    private static async Task SetUpDatabaseSchema(string connectionString)
    {
        using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "schema.sql");
        var schemaSql = await File.ReadAllTextAsync(filePath);

        // Create schema
        var createTableCmd2 = new NpgsqlCommand(schemaSql, connection);
        await createTableCmd2.ExecuteNonQueryAsync();
    }

    private static async Task<string> SetUpPostgresSqlContainer(DatabaseTestToolbox toolbox)
    {
        toolbox.PostgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .WithHostname("127.0.0.1")
            .WithDatabase("ManagerData")
            .WithUsername("manager")
            .WithPassword("Heslo1234.")
            .Build();

        await toolbox.PostgreSqlContainer.StartAsync();
        var connectionString = toolbox.PostgreSqlContainer.GetConnectionString();
        return connectionString;
    }
}