using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Model;
using NetDeviceManager.Lib.Snmp.Models;

namespace NetDeviceManager.Lib.Interfaces;

public interface ISnmpService
{
    #region CreateMethods

    public OperationResult UpsertSnmpSensor(SnmpSensor model, out Guid id);
    public OperationResult AssignSensorToDevice(SnmpSensorInPhysicalDevice model);

    #endregion

    #region ReadMethods

    List<SnmpVariableModel>? GetSensorValue(SnmpSensor sensor, LoginProfile profile, PhysicalDevice device, Port port);
    public int GetSnmpAlertsCount();
    
    public int GetDeviceSnmpAlertsCount(Guid id);
    public int GetSensorSnmpAlertsCount(Guid id);

    public List<SnmpSensor> GetSensorsInDevice(Guid deviceId);
    
    public SnmpSensor GetSensor(Guid id);

    #endregion

    #region UpdateMethods

    public OperationResult UpdateSnmpSensor(SnmpSensor model);

    #endregion

    #region DeleteMethods
    public OperationResult RemoveSensorFromDevice(SnmpSensorInPhysicalDevice model);

    #endregion
}