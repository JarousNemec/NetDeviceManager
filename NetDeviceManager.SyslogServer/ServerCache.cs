using NetDeviceManager.SyslogServer.Models;

namespace NetDeviceManager.SyslogServer;

public class ServerCache
{
    public List<CacheMessageModel> MessagesQueue { get; set; }
    public readonly object ProcessorLock = new object();
    

    public ServerCache()
    {
        MessagesQueue = new List<CacheMessageModel>();
    }
}