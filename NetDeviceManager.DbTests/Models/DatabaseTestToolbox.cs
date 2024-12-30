using Microsoft.EntityFrameworkCore;
using NetDeviceManager.Database;
using NetDeviceManager.Lib.Interfaces;
using Testcontainers.PostgreSql;

namespace NetDeviceManager.DbTests.Models;

public class DatabaseTestToolbox : IAsyncDisposable
{
    public DbContextOptions<ApplicationDbContext> Options { get; set; }
    public PostgreSqlContainer PostgreSqlContainer { get; set; }
    public ApplicationDbContext TestedContext { get; set; }

    public IDatabaseService DatabaseService { get; set; }
    public IDeviceService DeviceService { get; set; }
    public IFileStorageService FileStorageService { get; set; }
    public ILoginProfileService LoginProfileService { get; set; }
    public IPortService PortService { get; set; }
    public ISettingsService SettingsService { get; set; }
    public ISnmpService SnmpService { get; set; }
    public ISyslogService SyslogService { get; set; }

    public async ValueTask DisposeAsync()
    {
        await PostgreSqlContainer.StopAsync();
        await PostgreSqlContainer.DisposeAsync();
        if (TestedContext != null)
            await TestedContext.DisposeAsync();
    }
}