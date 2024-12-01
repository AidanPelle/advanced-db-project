using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shard_db.services;
using shard_db.dto;

namespace shard_db.controllers;

[Route("[controller]")]
[EnableCors("AllowAll")]
[ApiController]
public class DevicesController(DatabaseManager context, RandomWriteService randomWriteService) : ControllerBase
{
    [HttpGet("{deviceId}")]
    public async Task<ActionResult<DeviceDto>> GetDevice(int deviceId)
    {
        var device = await context.DeviceDbContexts[0].Device
            .Include(x => x.Sensors)
            .FirstOrDefaultAsync(x => x.Id == deviceId);
        if (device == null)
        {
            return NotFound();
        }

        var dto = new DeviceDto
        {
            Id = device.Id,
            Name = device.Name,
            Sensors = device.Sensors.Select(s => new DeviceSensorDto { Name = s.Name, Units = s.Units }).ToList()
        };
        
        return Ok(dto);
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<DeviceDto>>> GetDevices()
    {
        var devices = await context.DeviceDbContexts[0].Device.ToListAsync();

        return Ok(devices);
    }

    [HttpPost("")]
    public async Task<ActionResult<DeviceDto>> CreateDevice(DeviceDto device)
    {
        var deviceEntity = new Device
        {
            Name = device.Name,
            Sensors = device.Sensors.Select(sensor => new Sensor { Name = sensor.Name, Units = sensor.Units }).ToList()
        };
        
        await context.DeviceDbContexts[0].Device.AddAsync(deviceEntity);
        await context.DeviceDbContexts[0].SaveChangesAsync();

        return CreatedAtAction(nameof(GetDevice), new { deviceId = deviceEntity.Id }, device);
    }

    [HttpPatch("{deviceId:int}")]
    public async Task<ActionResult> UpdateDevice(int deviceId, Device updatedDevice)
    {
        var device = await context.DeviceDbContexts[0].Device.FindAsync(deviceId);

        if (device == null)
        {
            return NotFound();
        }

        device.Name = updatedDevice.Name;
        await context.DeviceDbContexts[0].SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{deviceId:int}/frequency/{frequencyValue:int}")]
    public ActionResult UpdateDeviceFrequency(int deviceId, int frequencyValue)
    {
        randomWriteService.SetWriteFrequency(deviceId, frequencyValue);

        return NoContent();
    }

    [HttpDelete("{deviceId:int}")]
    public async Task<ActionResult> DeleteDevice(int deviceId)
    {
        var device = await context.DeviceDbContexts[0].Device.FindAsync(deviceId);

        if (device == null)
        {
            return NotFound();
        } 

        context.DeviceDbContexts[0].Device.Remove(device);
        await context.DeviceDbContexts[0].SaveChangesAsync();

        return NoContent();
    }
}