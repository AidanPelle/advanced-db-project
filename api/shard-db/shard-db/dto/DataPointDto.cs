namespace shard_db.dto;

public class DataPointDto
{
    public string SensorId { get; set; } = null!;
    public DateTime ReceivedTimestamp { get; set; }
    public double Value { get; set; }
}