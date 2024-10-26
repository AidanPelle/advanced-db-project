using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace shard_db.controllers;

[Route("[controller]")]
[ApiController]
public class DevicesController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet("{deviceId}")]
    public async Task<ActionResult<Device>> GetDevice(int deviceId)
    {
        var device = await context.Device.FindAsync(deviceId);

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
    public async Task<ActionResult<Device>> CreateDevice(Device device)
    {
        context.Device.Add(device);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDevice), new { deviceId = device.Id }, device);
    }

    [HttpPut("{deviceId:int}")]
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