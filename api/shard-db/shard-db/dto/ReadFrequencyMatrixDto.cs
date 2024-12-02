namespace shard_db.dto;

public class DeviceReadFrequency
{
    public string DeviceId { get; set; } = null!;
    public string DeviceName { get; set; } = null!;
    public int FrequencyValue { get; set; }
}

public class ReadFrequencyMatrixDto
{
    public int SiteId { get; set; }
    public string SiteName { get; set; } = null!;
    public List<DeviceReadFrequency> Frequencies { get; set; } = null!;
}