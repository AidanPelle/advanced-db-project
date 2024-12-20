﻿using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shard_db.services;
using shard_db.dto;

namespace shard_db.controllers;

[Route("[controller]")]
[EnableCors("AllowAll")]
[ApiController]
public class DevicesController(DatabaseManager context) : ControllerBase
{
    [HttpGet("{deviceId}")]
    public async Task<ActionResult<DeviceDto>> GetDevice(string deviceId)
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
    public ActionResult<List<DeviceDto>> GetDevices()
    {
        var devices = context.DeviceDbContexts.SelectMany(d => d.Device.ToList()).ToList();

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

    [HttpPatch("{deviceId}")]
    public async Task<ActionResult> UpdateDevice(string deviceId, Device updatedDevice)
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

    [HttpDelete("{deviceId}")]
    public async Task<ActionResult> DeleteDevice(string deviceId)
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