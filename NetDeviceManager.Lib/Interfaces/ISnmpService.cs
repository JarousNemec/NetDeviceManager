using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Model;
using NetDeviceManager.Lib.Snmp.Models;

namespace NetDeviceManager.Lib.Interfaces;

public interface ISnmpService
{
    #region CreateMethods

     OperationResult UpsertSnmpSensor(SnmpSensor model, out Guid id);
     OperationResult AssignSensorToDevice(CorrectDataPattern model);

    #endregion

    #region ReadMethods

    string? GetSensorValue(SnmpSensor sensor, List<LoginProfile> profiles, PhysicalDevice device, Port? port);
     int GetSnmpAlertsCount();
    
     int GetDeviceSnmpAlertsCount(Guid id);
     int GetSensorSnmpAlertsCount(Guid id);

     bool IsSensorOfDeviceOk(Guid deviceId, Guid sensorId);

    List<SnmpSensor> GetSensorsInDevice(Guid deviceId);
    
    SnmpSensor GetSensor(Guid id);

    List<SnmpAlertModel> GetAlerts();

    #endregion

    #region UpdateMethods

     OperationResult UpdateSnmpSensor(SnmpSensor model);

    #endregion

    #region DeleteMethods

    void RemoveAlert(Guid id);
     OperationResult RemoveSensorFromDevice(SnmpSensorInPhysicalDevice model);

    #endregion
}