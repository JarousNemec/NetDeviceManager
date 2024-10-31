using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.SyslogServer.Helpers;

namespace NetDeviceManager.SyslogServer;

public class Server
{
    private readonly ServerCache _cache;

    public Server(ServerCache cache)
    {
        _cache = cache;
    }

    public void Run()
    {
        Console.WriteLine("Starting receiver...");

        var receiver = new MessageReceiver(_cache, 10514);
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