using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace shard_db;
public class DeviceDbContext : DbContext
{
    public DeviceDbContext(DbContextOptions<DeviceDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SensorData>().HasAlternateKey(sd => new { sd.SensorId, sd.ReceivedTimestamp });
    }

    public DbSet<Device> Device { get; set; }
    public DbSet<Sensor> Sensor { get; set; }
    public DbSet<SensorData> SensorData { get; set; }
}

public class Device
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Required]
    public string Name { get; set; } = null!;

    public List<Sensor> Sensors { get; set; } = null!;
}

public class Sensor
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int DeviceId { get; set; }

    [StringLength(50)]
    [Required]
    public string Name { get; set; } = null!;

    [StringLength(10)]
    [Required]
    public string Units { get; set; } = null!;

    [ForeignKey("DeviceId")]
    public Device Device { get; set; } = null!;

    public List<SensorData> SensorDatas { get; set; } = null!;
}

public class SensorData
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int SensorId { get; set; }

    [Required]
    public DateTime ReceivedTimestamp { get; set; }

    [Required]
    public double Value { get; set; }

    [ForeignKey("SensorId")]
    public Sensor Sensor { get; set; } = null!;
}


