namespace shard_db.dto;

public class DataPointDto
{
    public int SensorId { get; set; }
    public DateTime ReceivedTimestamp { get; set; }
    public double Value { get; set; }
}