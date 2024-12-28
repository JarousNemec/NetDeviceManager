using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using NetDeviceManager.Database;
using NetDeviceManager.Database.Tables;
using NetDeviceManager.Lib.Interfaces;
using NetDeviceManager.Lib.Model;
using NetDeviceManager.Lib.Utils;

namespace NetDeviceManager.Lib.Services;

public class PortService(ApplicationDbContext _database) : IPortService
{

    public OperationResult UpdatePortsAndDeviceRelations(List<Port> ports, Guid deviceId)
    {
        var currentRelations = GetPortInPhysicalDeviceRelations(deviceId);
        var toAdd = new List<PhysicalDeviceHasPort>();
        var toRemove = new List<PhysicalDeviceHasPort>();
        foreach (var port in ports)
        {
            if (currentRelations.All(x => x.Port.Number != port.Number))
            {
                toAdd.Add(new PhysicalDeviceHasPort(){PortId = port.Id, DeviceId = deviceId});
            }
        }

        foreach (var relation in currentRelations)
        {
            if (ports.All(x => x.Number != relation.Port.Number))
            {
                toRemove.Add(relation);
            }
        }

        try
        {
            foreach (var relationToAdd in toAdd)
            {
                AddPortToPhysicalDevice(relationToAdd);
            }

            foreach (var relationToRemove in toRemove)
            {
                RemovePortFromDevice(relationToRemove.Id);
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            return new OperationResult(){IsSuccessful = false, Message = e.Message };
        }
        
        
        return new OperationResult();
    }

    public Guid UpsertPort(Port port)
    {
        var existingPort = _database.Ports.FirstOrDefault(x => x.Id == port.Id);
        if (existingPort == null)
        {
            port.Id = DatabaseUtil.GenerateId();
            _database.Ports.Update(port);
            _database.SaveChanges();
            return port.Id;
        }

        _database.Ports.Add(port);
        _database.SaveChanges();
        return port.Id;
    }

    public Guid AddPortToPhysicalDevice(PhysicalDeviceHasPort physicalDeviceHasPort)
    {
        var id = DatabaseUtil.GenerateId();
        physicalDeviceHasPort.Id = id;
        _database.PhysicalDevicesHavePorts.Add(physicalDeviceHasPort);
        _database.SaveChanges();
        return id;
    }

    public Guid? GetPortDeviceRelationId(Guid portId, Guid deviceId)
    {
        return _database.PhysicalDevicesHavePorts.FirstOrDefault(x => x.Id == portId && x.DeviceId == deviceId)?.Id;
    }

    public List<Port> GetPortsInPhysicalDevice(Guid deviceId)
    {
        return _database.PhysicalDevicesHavePorts.AsNoTracking().Where(x => x.DeviceId == deviceId).Include(x => x.Port)
            .Select(x => x.Port).ToList();
    }

    public List<PhysicalDeviceHasPort> GetPortInPhysicalDeviceRelations(Guid deviceId)
    {
        return _database.PhysicalDevicesHavePorts
            .AsNoTracking()
            .Where(x => x.DeviceId == deviceId)
            .Include(x => x.Port)
            .ToList();
    }

    public List<Port> GetPortsInSystem()
    {
        return _database.Ports.AsNoTracking().ToList();
    }

    public OperationResult RemovePortFromDevice(Guid id)
    {
        var record = _database.PhysicalDevicesHavePorts.FirstOrDefault(x => x.Id == id);
        if (record != null)
        {
            _database.PhysicalDevicesHavePorts.Remove(record);
            _database.SaveChanges();
            return new OperationResult();
        }

        return new OperationResult() { IsSuccessful = false, Message = "Cannot remove port" };
    }

    public OperationResult RemovePort(Guid id)
    {
        var port = _database.Ports.FirstOrDefault(x => x.Id == id);
        if (port != null)
        {
            if (_database.PhysicalDevicesHavePorts.Any(x => x.PortId == port.Id))
            {
                var relations = _database.PhysicalDevicesHavePorts.Where(x => x.PortId == port.Id).ToList();
                _database.PhysicalDevicesHavePorts.RemoveRange(relations);
                _database.SaveChanges();
            }
            _database.Ports.Remove(port);
            _database.SaveChanges();
            return new OperationResult();
        }

        return new OperationResult() { IsSuccessful = false, Message = "Unknown id" };
    }

    public bool PortAndDeviceRelationExists(Guid portId, Guid deviceId)
    {
        var existing =
            _database.PhysicalDevicesHavePorts.AsNoTracking()
                .FirstOrDefault(x => x.DeviceId == deviceId && x.PortId == portId);
        if (existing == null)
        {
            return false;
        }
        return true;
    }
}