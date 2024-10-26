namespace shard_db.dto;

public class DeviceDto
{
    public int? Id { get; set; }
    public string Name { get; set; } = null!;
    public List<DeviceSensorDto> Sensors { get; set; } = null!;
}