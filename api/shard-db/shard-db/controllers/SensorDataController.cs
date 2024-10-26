using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shard_db.dto;

namespace shard_db.controllers;


[Route("[controller]")]
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

    [HttpPost("{id:int}")]
    public async Task<ActionResult> CreateSensorData([FromBody] DataPointDto dataPoint)
    {
        var sd = new SensorData
        {
            SensorId = dataPoint.SensorId,
            ReceivedTimestamp = dataPoint.ReceivedTimestamp,
            Value = dataPoint.Value
        };

        // TODO: Implement method to create new sensor data
        return CreatedAtAction(nameof(GetSensorDataBySensorId), new { id = 1 }, "CreateSensorData");
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateSensorData(int id, [FromBody] object sensorData)
    {
        // TODO: Implement method to update existing sensor data
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSensorData(int id)
    {
        // TODO: Implement method to delete sensor data by ID
        return NoContent();
    }

    /*END_USER_CODE*/
}