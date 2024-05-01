using Microsoft.AspNetCore.Server.Kestrel.Core;
using NetDeviceManager.Lib.GlobalConstantsAndEnums;

namespace NetDeviceManager.Database.Tables;

public class Port
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Number { get; set; }
    public CommunicationProtocol Protocol { get; set; }
}