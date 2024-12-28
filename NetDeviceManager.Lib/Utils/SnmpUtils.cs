using Lextm.SharpSnmpLib;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.GlobalConstantsAndEnums;
using NetDeviceManager.Lib.Interfaces;

namespace NetDeviceManager.Lib.Utils;

public static class SnmpUtils
{
    public static VersionCode GetVersionCode(string version)
    {
        VersionCode snmpVersion;
        switch (version)
        {
            case "1":
                snmpVersion = VersionCode.V1;
                break;
            case "2":
                snmpVersion = VersionCode.V2;
                break;
            case "3":
                snmpVersion = VersionCode.V3;
                break;
            default:
                snmpVersion = VersionCode.V2;
                break;
        }

        return snmpVersion;
    }

    public static Port? GetSnmpPort(Guid deviceId, IDatabaseService databaseService)
    {
        var port = databaseService.GetPortsInPhysicalDevice(deviceId)
            .FirstOrDefault(x => x.Protocol == CommunicationProtocol.SNMP);

        return port;
    }
}