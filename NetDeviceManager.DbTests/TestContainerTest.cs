using Microsoft.EntityFrameworkCore;
using NetDeviceManager.Database;
using NetDeviceManager.ScheduledSnmpAgent.Helpers;
using Npgsql;
using Testcontainers.PostgreSql;
using NUnit.Framework;

namespace NetDeviceManager.DbTests;

[TestFixture]
public class PostgresTests
{
    private PostgreSqlContainer _postgresContainer;
    private DbContextOptions<ApplicationDbContext> _options;


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
    public async Task CreateSchemaAndCheckAmountOfTablesTest ()
    {
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

        string schemaName = "public"; // Adjust this to your schema name
        // SQL query to count the number of tables in the specified schema
        string query = @"
            SELECT COUNT(*)
            FROM information_schema.tables
            WHERE table_schema = @SchemaName AND table_type = 'BASE TABLE'";


        using (var command = new NpgsqlCommand(query, connection))
        {
            // Set the parameter value
            command.Parameters.AddWithValue("@SchemaName", schemaName);

            // Execute the query and get the result
            var tableCount = await command.ExecuteScalarAsync();
            
            Assert.That(tableCount, Is.EqualTo(25));
            
            Console.WriteLine($"Number of tables in schema '{schemaName}': {tableCount}");
        }
    }
}

