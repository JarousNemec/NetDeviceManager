using NetDeviceManager.Database.Interfaces;
using NetDeviceManager.SyslogServer.Helpers;

namespace NetDeviceManager.SyslogServer;

public class Server
{
    private readonly ServerCache _cache;
    private readonly int _receiverPort;

    public Server(ServerCache cache, IDatabaseService database)
    {
        _cache = cache;
        _receiverPort = int.Parse(database.GetConfigValue("SyslogUdpPort") ?? "514");
    }

    public void Run()
    {
        Console.WriteLine("Starting receiver...");

        var receiver = new MessageReceiver(_cache, _receiverPort);
        Thread receiverThread = new Thread(receiver.Run);
        receiverThread.Start();

        var connectionString = ConfigurationHelper.GetConfigurationString();
        if (connectionString != null)
        {
            Console.WriteLine("Starting processor...");
            var processor = new MessageProcessor(_cache, connectionString);
            Thread processorThread = new Thread(processor.Run);
            processorThread.Start();
        }

        Console.WriteLine("Running...");
    }
}