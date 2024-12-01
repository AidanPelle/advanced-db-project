public class SiteDto {
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public List<DeviceConfDto> Devices { get; set; } = null!;
}

public class DeviceConfDto {
    public string Name { get; set; } = null!;
    public int UUID { get; set;}
    public List<SensorDto> Sensors { get; set; } = null!;
}

public class SensorDto {
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Units { get; set; } = null!;
}
