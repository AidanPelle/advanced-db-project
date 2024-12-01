using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shard_db.dto;

namespace shard_db.controllers;


[Route("[controller]")]
[EnableCors("AllowAll")]
[ApiController]
public class SensorDataController(DatabaseManager context) : ControllerBase
{
    [HttpGet("all")]
    public async Task<ActionResult<List<SensorData>>> GetAllSensorData()
    {
        var data = await context.DeviceDbContexts[0].SensorData.ToListAsync();
        return Ok(data);
    }

    [HttpGet("{sensorId}")]
    public async Task<ActionResult> GetSensorDataBySensorId(string sensorId)
    {
        var data = await context.DeviceDbContexts[0].SensorData.Where(sd => sd.SensorId == sensorId).ToListAsync();
        return Ok(data);
    }
    
    public async Task<ActionResult> GetDataPointById(string id)
    {
        var data = await context.DeviceDbContexts[0].SensorData.FindAsync(id);
        if (data == null)
        {
            return NotFound();
        }
        
        return Ok(data);
    }

    [HttpPost]
    public async Task<ActionResult> CreateSensorData([FromBody] DataPointDto dataPoint)
    {
        var sensorData = new SensorData
        {
            SensorId = dataPoint.SensorId,
            ReceivedTimestamp = dataPoint.ReceivedTimestamp,
            Value = dataPoint.Value
        };
        
        await context.DeviceDbContexts[0].SensorData.AddAsync(sensorData);
        await context.DeviceDbContexts[0].SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetDataPointById), new { id = sensorData.Id }, "CreateSensorData");
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateSensorData(string id, [FromBody] DataPointDto dataPoint)
    {
        var sensorData = await context.DeviceDbContexts[0].SensorData.FindAsync(id);

        if (sensorData == null)
        {
            return NotFound();
        }
        
        sensorData.ReceivedTimestamp = dataPoint.ReceivedTimestamp;
        sensorData.Value = dataPoint.Value;
        
        await context.DeviceDbContexts[0].SaveChangesAsync();
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSensorData(string id)
    {
        var sensorData = await context.DeviceDbContexts[0].SensorData.FindAsync(id);

        if (sensorData == null)
        {
            return NotFound();
        }
        
        context.DeviceDbContexts[0].SensorData.Remove(sensorData);
        await context.DeviceDbContexts[0].SaveChangesAsync();
        return NoContent();
    }

    /*END_USER_CODE*/
}