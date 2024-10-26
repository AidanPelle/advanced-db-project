using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shard_db.services;
using shard_db.dto;

namespace shard_db.controllers;

[Route("[controller]")]
[ApiController]
public class DevicesController(ApplicationDbContext context, RandomWriteService randomWriteService) : ControllerBase
{
    [HttpGet("{deviceId}")]
    public async Task<ActionResult<Device>> GetDevice(int deviceId)
    {
        var device = await context.Device
            .Include(x => x.Sensors)
            .FirstOrDefaultAsync(x => x.Id == deviceId);

        if (device == null)
        {
            return NotFound();
        }

        return Ok(device);
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<Device>>> GetDevices()
    {
        var devices = await context.Device.ToListAsync();

        return Ok(devices);
    }

    [HttpPost("")]
    public async Task<ActionResult<Device>> CreateDevice(DeviceDto device)
    {
        var deviceEntity = new Device
        {
            Name = device.Name,
            Sensors = device.Sensors.Select(sensor => new Sensor { Name = sensor.Name, Units = sensor.Units }).ToList()
        };
        
        await context.Device.AddAsync(deviceEntity);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDevice), new { deviceId = deviceEntity.Id }, device);
    }

    [HttpPatch("{deviceId:int}")]
    public async Task<ActionResult> UpdateDevice(int deviceId, Device updatedDevice)
    {
        var device = await context.Device.FindAsync(deviceId);

        if (device == null)
        {
            return NotFound();
        }

        device.Name = updatedDevice.Name;
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{deviceId:int}/frequency/{frequencyValue:int}")]
    public async Task<ActionResult> UpdateDeviceFrequency(int deviceId, int frequencyValue)
    {
        randomWriteService.SetWriteFrequency(deviceId, frequencyValue);

        return NoContent();
    }

    [HttpDelete("{deviceId:int}")]
    public async Task<ActionResult> DeleteDevice(int deviceId)
    {
        var device = await context.Device.FindAsync(deviceId);

        if (device == null)
        {
            return NotFound();
        } 

        context.Device.Remove(device);
        await context.SaveChangesAsync();

        return NoContent();
    }
}