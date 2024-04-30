using System.Net;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Security;
using NetDeviceManager.Lib.Snmp.Interfaces;

namespace NetDeviceManager.Lib.Snmp.Services;

public class SnmpService : ISnmpService
{
    private const int MESSANGER_GET_TIMEOUT = 10000;
    public List<Variable>? GetSensorValue(VersionCode version, string ip, int port, string community, string oid, string authPass = "", string privacyPass = "", string username = "")
    {
        if (version != VersionCode.V3)
            return ReadSensorV1V2(version, ip, port, community, oid);
        if (string.IsNullOrEmpty(authPass) || string.IsNullOrEmpty(privacyPass) || string.IsNullOrEmpty(username))
            return null;
        return ReadSensorV3(ip, port, oid, authPass, privacyPass, username);
    }

    private List<Variable> ReadSensorV1V2(VersionCode version, string ip, int port, string community, string oid)
    {
        var results = new List<Variable>();
        try
        {
            results = Messenger.Get(version,
                new IPEndPoint(IPAddress.Parse(ip), port),
                new OctetString(community),
                new List<Variable>
                    { new(new ObjectIdentifier(oid)) }, MESSANGER_GET_TIMEOUT).ToList();
        }
        catch (Exception e)
        {
            Console.Out.WriteLine(e.Message);
        }
        return results;
    }
    private List<Variable> ReadSensorV3(string ip, int port, string oid, string authPass, string privacyPass, string username)
    {
        var results = new List<Variable>();
        try
        {
            Discovery discovery = Messenger.GetNextDiscovery(SnmpType.GetRequestPdu);
            ReportMessage report = discovery.GetResponse(MESSANGER_GET_TIMEOUT, new IPEndPoint(IPAddress.Parse(ip), port));
            var auth = new SHA1AuthenticationProvider(new OctetString(authPass));
            var priv = new DESPrivacyProvider(new OctetString(privacyPass), auth);

            GetRequestMessage request = new GetRequestMessage(
                VersionCode.V3, 
                Messenger.NextMessageId,
                Messenger.NextRequestId, 
                new OctetString(username),
                new List<Variable> { new Variable(new ObjectIdentifier(oid)) }, 
                priv,
                Messenger.MaxMessageSize, report);
        
            ISnmpMessage reply = request.GetResponse(MESSANGER_GET_TIMEOUT, new IPEndPoint(IPAddress.Parse(ip), port));
        
            if (reply.Pdu().ErrorStatus.ToInt32() != 0)
            {
                throw ErrorException.Create(
                    "error in response",
                    IPAddress.Parse(ip),
                    reply);
            }
            results = reply.Pdu().Variables.ToList();
        }
        catch (Exception e)
        {
            Console.Out.WriteLine(e.Message);
        }


        return results;
    }
}