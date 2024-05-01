using Lextm.SharpSnmpLib;

namespace NetDeviceManager.Lib.Snmp.Interfaces;

public interface ISnmpService
{
    List<Variable>? GetSensorValue(VersionCode version, string ip, int port, string community, string oid,
        string authPass = "", string privacyPass = "", string securityName = "");
}