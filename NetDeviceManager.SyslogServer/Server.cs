using NetDeviceManager.Lib.Helpers;

namespace NetDeviceManager.SyslogServer;

public class Server : IDisposable
{
    private readonly ServerCache _cache;
    private Thread _processorThread;
    private Thread _receiverThread;
    private bool _stopping = false;
    public Server(ServerCache cache)
    {
        _cache = cache;

        var connectionString = SystemConfigurationHelper.GetConnectionString();
        if (connectionString == null)
            return;

        var receiver = new MessageReceiver(_cache, int.Parse(SystemConfigurationHelper.GetValue("UdpPort")!));
        receiver.OnCrash += s =>
        {
            if(_stopping)
                return;
            if (_receiverThread.IsAlive)
                _receiverThread.Interrupt();
            _receiverThread = new Thread(receiver.Run);
            _receiverThread.Start();
        };
        _receiverThread = new Thread(receiver.Run);

        var processor = new MessageProcessor(_cache, connectionString);
        processor.OnCrash += s =>
        {
            if(_stopping)
                return;
            if (_processorThread.IsAlive)
                _processorThread.Interrupt();
            _processorThread = new Thread(processor.Run);
            _processorThread.Start();
        };
        _processorThread = new Thread(processor.Run);
    }

    public void Run()
    {
        if(_stopping)
            return;
        if (_processorThread != null && _receiverThread != null)
        {
            Console.WriteLine("Starting receiver...");
            _receiverThread.Start();

            Console.WriteLine("Starting processor...");
            _processorThread.Start();
        }

        Console.WriteLine("Running...");
    }

    public void Dispose()
    {
        _stopping = true;
        _processorThread?.Interrupt();
        _receiverThread?.Interrupt();
    }
}