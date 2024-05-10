using System.Net;
using System.Net.Sockets;
using System.Text;
using NetDeviceManager.SyslogServer.Models;

namespace NetDeviceManager.SyslogServer;

public class MessageProcessor
{
    private readonly ServerCache _cache;

    public MessageProcessor(ServerCache cache)
    {
        _cache = cache;
    }

    public void Run()
    {
        Console.WriteLine("Running processor...");
        try
        {
            while (true)
            {
                Thread.Sleep(1000);
                lock (_cache.ProcessorLock)
                {
                    if (_cache.MessagesQueue == null || _cache.MessagesQueue.Count < 1)
                    {
                        continue;
                    }

                    foreach (var Message in _cache.MessagesQueue)
                    {
                        Console.WriteLine($"Processing message.... ({Message.Ip}) : {Message.Message}");
                    }
                    _cache.MessagesQueue = new List<CacheMessageModel>();
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Run();
        }
    }
}