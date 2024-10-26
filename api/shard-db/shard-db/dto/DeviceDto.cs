namespace shard_db.dto;

public class DeviceDto
{
    public string Name { get; set; } = null!;
    public List<DeviceSensorDto> Sensors { get; set; } = null!;
}