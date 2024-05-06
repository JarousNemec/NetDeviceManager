using Lextm.SharpSnmpLib;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Snmp.Models;

namespace NetDeviceManager.Lib.Snmp.Interfaces;

public interface ISnmpService
{
    List<VariableModel>? GetSensorValue(SnmpSensor sensor, LoginProfile profile, PhysicalDevice device, Port port);
}