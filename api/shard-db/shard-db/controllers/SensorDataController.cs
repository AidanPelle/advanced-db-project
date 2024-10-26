using Microsoft.AspNetCore.Mvc;

namespace shard_db.controllers;

[ApiController]
[Route("[controller]")]
public class SensorDataController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet("/all")]
    public async Task<ActionResult<List<SensorData>>> GetAllSensorData()
    {
        var data = context.SensorData.ToList();
        return Ok(data);
    }

    [HttpGet("/{id:int}")]
    public async Task<ActionResult> GetSensorDataBySensorId(int sensorId)
    {
        // TODO: Make this lookup by sensor Id. Currently doesn't do that.
        var data = await context.SensorData.FindAsync(sensorId);

        if (data == null)
        {
            return NotFound();
        }

        return Ok(data);
    }

    [HttpPost("/{id:int}")]
    public async Task<ActionResult> CreateSensorData([FromBody] object sensorData)
    {
        // TODO: Implement method to create new sensor data
        return CreatedAtAction(nameof(GetSensorDataBySensorId), new { id = 1 }, "CreateSensorData");
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateSensorData(int id, [FromBody] object sensorData)
    {
        // TODO: Implement method to update existing sensor data
        return NoContent();
    }

    [HttpDelete("/{id}")]
    public async Task<ActionResult> DeleteSensorData(int id)
    {
        // TODO: Implement method to delete sensor data by ID
        return NoContent();
    }

    /*END_USER_CODE*/
}