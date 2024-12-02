using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace shard_db;
public class DeviceDbContext : DbContext
{
    public DeviceDbContext(DbContextOptions<DeviceDbContext> options, int SiteId)
        : base(options)
    {
        this.SiteId = SiteId;
    }

    public int SiteId;

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
    [MinLength(36)]
    [MaxLength(36)]
    public string Id { get; set; } = string.Empty;

    [StringLength(50)]
    [Required]
    public string Name { get; set; } = null!;

    [JsonIgnore]
    public List<Sensor> Sensors { get; set; } = null!;
}

public class Sensor
{
    [Key]
    [MinLength(36)]
    [MaxLength(36)]
    public string Id { get; set; } = string.Empty;

    [Required] 
    [MinLength(36)]
    [MaxLength(36)]
    public string DeviceId { get; set; } = string.Empty;

    [StringLength(50)]
    [Required]
    public string Name { get; set; } = null!;

    [StringLength(10)]
    [Required]
    public string Units { get; set; } = null!;

    [JsonIgnore]
    [ForeignKey("DeviceId")]
    public Device Device { get; set; } = null!;

    public List<SensorData> SensorDatas { get; set; } = [];
}

public class SensorData
{
    [Key]
    public int Id { get; set; }

    [Required] 
    public string SensorId { get; set; } = string.Empty;

    [Required]
    public DateTime ReceivedTimestamp { get; set; }

    [Required]
    public double Value { get; set; }

    // [ForeignKey("SensorId")]
    public Sensor Sensor { get; set; } = null!;
}


