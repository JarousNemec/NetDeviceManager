using System.ComponentModel.DataAnnotations;
using NetDeviceManager.Lib.GlobalConstantsAndEnums;

namespace NetDeviceManager.Database.Tables;

public class Port
{
    public Guid Id { get; set; }
    [Required]public int Number { get; set; }
    [Required]public CommunicationProtocol Protocol { get; set; }
}