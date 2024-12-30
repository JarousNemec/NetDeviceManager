using NetDeviceManager.Database.Models;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Model;
using NetDeviceManager.Lib.Snmp.Models;

namespace NetDeviceManager.Lib.Interfaces;

public interface ISnmpService
{

     OperationResult UpsertSnmpSensor(SnmpSensor model, out Guid id);
     

    string? GetSensorValue(SnmpSensor sensor, List<LoginProfile> profiles, PhysicalDevice device, Port? port);
     int GetSnmpAlertsCount();

     void UpdateSnmpSensor(SnmpSensor model);
     int GetDeviceSnmpAlertsCount(Guid id);
     int GetSensorSnmpAlertsCount(Guid id);
     bool IsSensorOfDeviceOk(Guid deviceId, Guid sensorId);

    List<SnmpAlertModel> GetAlerts();

    void RemoveAlert(Guid id);
     
     
     Guid AddSnmpRecord(SnmpSensorRecord record);

     Guid AddSnmpSensor(SnmpSensor sensor);

     List<SnmpSensorRecord> GetLastSnmpRecords(int count);

     SnmpSensorRecord? GetLastDeviceRecord(Guid id);

     List<SnmpSensorRecord> GetSnmpRecordsWithFilter(SnmpRecordFilterModel model, int count = -1);
    
     List<SnmpSensor> GetAllSensors();

     int GetSensorUsagesCount(Guid id);
    
     OperationResult DeleteSnmpSensor(Guid id);
     bool IsAnySensorInDevice(Guid id);
}