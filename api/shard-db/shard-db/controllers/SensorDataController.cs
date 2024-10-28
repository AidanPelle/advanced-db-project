using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shard_db.dto;

namespace shard_db.controllers;


[Route("[controller]")]
[EnableCors("AllowAll")]
[ApiController]
public class SensorDataController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet("all")]
    public async Task<ActionResult<List<SensorData>>> GetAllSensorData()
    {
        var data = await context.SensorData.ToListAsync();
        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetSensorDataBySensorId(int sensorId)
    {
        var data = await context.SensorData.Where(sd => sd.SensorId == sensorId).ToListAsync();
        return Ok(data);
    }
    
    public async Task<ActionResult> GetDataPointById(int id)
    {
        var data = await context.SensorData.FindAsync(id);
        if (data == null)
        {
            return NotFound();
        }
        
        return Ok(data);
    }

    [HttpPost("{id:int}")]
    public async Task<ActionResult> CreateSensorData([FromBody] DataPointDto dataPoint)
    {
        var sensorData = new SensorData
        {
            SensorId = dataPoint.SensorId,
            ReceivedTimestamp = dataPoint.ReceivedTimestamp,
            Value = dataPoint.Value
        };
        
        await context.SensorData.AddAsync(sensorData);
        await context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetDataPointById), new { id = sensorData.Id }, "CreateSensorData");
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateSensorData(int id, [FromBody] DataPointDto dataPoint)
    {
        var sensorData = await context.SensorData.FindAsync(id);

        if (sensorData == null)
        {
            return NotFound();
        }
        
        sensorData.ReceivedTimestamp = dataPoint.ReceivedTimestamp;
        sensorData.Value = dataPoint.Value;
        
        await context.SaveChangesAsync();
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSensorData(int id)
    {
        var sensorData = await context.SensorData.FindAsync(id);

        if (sensorData == null)
        {
            return NotFound();
        }
        
        context.SensorData.Remove(sensorData);
        await context.SaveChangesAsync();
        return NoContent();
    }

    /*END_USER_CODE*/
}