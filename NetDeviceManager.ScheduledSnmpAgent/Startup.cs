using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace NetDeviceManager.ScheduledSnmpAgent;

public class Startup
{
    public IConfigurationRoot Configuration { get; set; }

    public Startup(IHostingEnvironment env)
    {
        // var builder = new ConfigurationBuilder().Add()
        //     .SetBasePath(env.ContentRootPath)
        //     .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        //
        // Configuration = builder.Build();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // services.AddMvc();
        //
        // // Add functionality to inject IOptions<T>
        // services.AddOptions();
        //
        // // Add our Config object so it can be injected
        // services.Configure<MyConfig>(Configuration.GetSection("MyConfig"));
    }
}