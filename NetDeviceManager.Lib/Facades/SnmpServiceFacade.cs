using Microsoft.Extensions.Logging;
using NetDeviceManager.Database.Models;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Model;
using NetDeviceManager.Lib.Services;

namespace NetDeviceManager.Lib.Facades;

public class SnmpServiceFacade(SnmpService snmpService, ILogger<SnmpService> logger) : ISnmpService
{
    public OperationResult UpsertSnmpSensor(SnmpSensor model, out Guid id)
    {
        var result = snmpService.UpsertSnmpSensor(model, out id);
        logger.LogInformation($"Upserted snmp sensor with id: {id}");
        return result;
    }

    

    public string? GetSensorValue(SnmpSensor sensor, List<LoginProfile> profiles, PhysicalDevice device, Port? port)
    {
        var result = snmpService.GetSensorValue(sensor, profiles, device, port);
        logger.LogInformation(
            $"{(result != null ? "Successfully" : "Non successfully")} downloaded value of sensor id: {sensor.Id}, loginProfiles: {string.Join(", ", profiles.Select(x => x.Name))}, belongs to device id: {device.Id} on port: {port.Number}");
        return result;
    }

    public int GetSnmpAlertsCount()
    {
        var result = snmpService.GetSnmpAlertsCount();
        logger.LogInformation($"Snmp alert count is: {result}");
        return result;
    }

    public void UpdateSnmpSensor(SnmpSensor model)
    {
        snmpService.UpdateSnmpSensor(model);
        logger.LogInformation($"Updated snmp sensor with id: {model.Id}");
    }

    public int GetDeviceSnmpAlertsCount(Guid id)
    {
        var result = snmpService.GetDeviceSnmpAlertsCount(id);
        logger.LogInformation($"Snmp alert count is: {result} for device id: {id}");
        return result;
    }

    public int GetSensorSnmpAlertsCount(Guid id)
    {
        var result = snmpService.GetSensorSnmpAlertsCount(id);
        logger.LogInformation($"Snmp alert count is: {result} for sensor id: {id}");
        return result;
    }

    public bool IsSensorOfDeviceOk(Guid deviceId, Guid sensorId)
    {
        var result = snmpService.IsSensorOfDeviceOk(deviceId, sensorId);
        logger.LogInformation($"Sensor of device id: {deviceId}, sensor id: {sensorId} is ok: {result.ToString()}");
        return result;
    }

    public List<SnmpAlertModel> GetAlerts()
    {
        var result = snmpService.GetAlerts();
        logger.LogInformation($"Getting alerts: {result}");
        return result;
    }

    public void RemoveAlert(Guid id)
    {
        snmpService.RemoveAlert(id);
        logger.LogInformation($"Removed alert with id: {id}");
    }

    

    public Guid AddSnmpRecord(SnmpSensorRecord record)
    {
        var result = snmpService.AddSnmpRecord(record);
        logger.LogInformation($"Added snmp sensor record with id: {record.Id}");
        return result;
    }

    public Guid AddSnmpSensor(SnmpSensor sensor)
    {
        var result = snmpService.AddSnmpSensor(sensor);
        logger.LogInformation($"Added snmp sensor with id: {sensor.Id}");
        return result;
    }

    public List<SnmpSensorRecord> GetLastSnmpRecords(int count)
    {
        var result = snmpService.GetLastSnmpRecords(count);
        logger.LogInformation($"Got last {result.Count} snmp records");
        return result;
    }

    public SnmpSensorRecord? GetLastDeviceRecord(Guid id)
    {
        var result = snmpService.GetLastDeviceRecord(id);
        logger.LogInformation(result != null ? $"Got last device record with id: {result.Id}":$"Cannot get last device record for device id: {id}");
        return result;
    }

    public List<SnmpSensorRecord> GetSnmpRecordsWithFilter(SnmpRecordFilterModel model, int count = -1)
    {
        var result = snmpService.GetSnmpRecordsWithFilter(model, count);
        logger.LogInformation($"Got {result.Count} Snmp records for filter: deviceName - {model.DeviceName}, sensorName - {model.SensorName}, Oid - {model.Oid}");
        return result;
    }

    public List<SnmpSensor> GetAllSensors()
    {
        var result = snmpService.GetAllSensors();
        logger.LogInformation($"Got all Snmp sensors count: {result.Count}");
        return result;
    }

    public int GetSensorUsagesCount(Guid id)
    {
        var result = snmpService.GetSensorUsagesCount(id);
        logger.LogInformation($"Sensor with id: {id} is used count: {result}");
        return result;
    }

    public OperationResult DeleteSnmpSensor(Guid id)
    {
        var result = snmpService.DeleteSnmpSensor(id);
        logger.LogInformation($"{(result.IsSuccessful ? "Removed" : $"Error: {result.Message} - Failed to remove")} sensor with id: {id}");
        return result;
    }

    public bool IsAnySensorInDevice(Guid id)
    {
        var result = snmpService.IsAnySensorInDevice(id);
        logger.LogInformation($"To device with id: {id} {(result?"is assigned some sensor":"isn't assigned any sensor")}");
        return result;
    }
}