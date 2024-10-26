using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Define DbSets for your entities
    public DbSet<Device> Device { get; set; }
    public DbSet<Sensor> Sensor { get; set; }
    public DbSet<SensorData> SensorData { get; set; }
}

public class Device
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}

public class Sensor
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public string Name { get; set; } = null!;
    public string Units { get; set; } = null!;
}

public class SensorData
{
    public int Id { get; set; }
    public int SensorId { get; set; }
    public DateTime ReceivedTimestamp { get; set; }
    public double Value { get; set; }
}

public class QueryLog
{
    public int FragmentId { get; set; }
    public int SiteId { get; set; }
    public DateTime AccessDate { get; set; }
    public DATA_TYPE DataType { get; set; }
    public int DataVolume { get; set; }     // Amount of data transferred with the request, in bytes
}

public enum DATA_TYPE
{
    READ,
    WRITE
}