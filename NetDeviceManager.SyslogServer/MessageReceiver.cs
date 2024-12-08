using System.Net;
using System.Net.Sockets;
using System.Text;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.SyslogServer.Models;

namespace NetDeviceManager.SyslogServer;
//config keys: SyslogUdpPort
public class MessageReceiver
{
    private readonly ServerCache _cache;
    private readonly IDatabaseService _database;
    private readonly int _port;

    public delegate void CrashDelegate(string m);
    public event CrashDelegate? OnCrash;
    
    public MessageReceiver(ServerCache cache, int port)
    {
        _cache = cache;
        _port = port;

    }

    public void Run()
    {
        Console.WriteLine("Running receiver...");
        var udpListener = new UdpClient(_port);

        try
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, _port);
            string receivedData;
            byte[] receivedBytes;

            while (true)
            {
                receivedBytes = udpListener.Receive(ref endpoint);
                receivedData = Encoding.ASCII.GetString(receivedBytes, 0, receivedBytes.Length);
                Console.WriteLine($"From ip: {endpoint.Address.ToString()} in {DateTime.Now} received: {receivedData}");
                
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
            Thread.Sleep(3000);
            Console.WriteLine("New atempt to run receiver...");
            OnCrash?.Invoke(e.Message);
        }
    }
}