using Microsoft.EntityFrameworkCore;
using NetDeviceManager.Database;
using NetDeviceManager.DbTests.Helpers;
using NetDeviceManager.DbTests.Models;
using Npgsql;
using Testcontainers.PostgreSql;

namespace NetDeviceManager.DbTests;
public class PostgresTests
{
    private DatabaseTestToolbox _toolbox;

    [SetUp]
    public async Task SetUp()
    {
        _toolbox = await TestEnvironmentHelper.SetUpDatabaseToolBox(false);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _toolbox.DisposeAsync();
    }
    
    [Test]
    public void DbConnection()
    {
        var connectionString = _toolbox.PostgreSqlContainer.GetConnectionString();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        ApplicationDbContext database = new ApplicationDbContext(options);
        bool canConnect = database.Database.CanConnect();
        Assert.That(canConnect);
    }

    [Test]
    public async Task CreateSchemaAndCheckAmountOfTablesTest ()
    {
        // given
        var connectionString = _toolbox.PostgreSqlContainer.GetConnectionString();
        using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        // when
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


        // then 
        await using var command = new NpgsqlCommand(query, connection);
        // Set the parameter value
        command.Parameters.AddWithValue("@SchemaName", schemaName);

        // Execute the query and get the result
        var tableCount = await command.ExecuteScalarAsync();
            
        Assert.That(tableCount, Is.EqualTo(25));
            
        Console.WriteLine($"Number of tables in schema '{schemaName}': {tableCount}");
    }
}

