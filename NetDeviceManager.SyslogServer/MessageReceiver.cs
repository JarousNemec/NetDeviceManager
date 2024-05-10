using System.Net;
using System.Net.Sockets;
using System.Text;
using NetDeviceManager.SyslogServer.Helpers;
using NetDeviceManager.SyslogServer.Models;

namespace NetDeviceManager.SyslogServer;

public class MessageReceiver
{
    private readonly ServerCache _cache;

    public MessageReceiver(ServerCache cache)
    {
        _cache = cache;
    }

    public void Run()
    {
        Console.WriteLine("Running receiver...");
        var port = int.Parse(ConfigurationHelper.GetValue("UdpPort") ?? "514");
        var udpListener = new UdpClient(port);

        try
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, port);
            string receivedData;
            byte[] receivedBytes;

            while (true)
            {
                receivedBytes = udpListener.Receive(ref endpoint);
                receivedData = Encoding.ASCII.GetString(receivedBytes, 0, receivedBytes.Length);
                Console.WriteLine($"{DateTime.Now} {receivedData}");
                
                lock (_cache.ProcessorLock)
                {
                    _cache.MessagesQueue.Add(new CacheMessageModel(){Message = receivedData, Ip = endpoint.Address.ToString()});
                }
            }
        }
        catch (Exception e)
        {
            udpListener.Close();
            Console.WriteLine(e.Message);
            Run();
        }
    }
}