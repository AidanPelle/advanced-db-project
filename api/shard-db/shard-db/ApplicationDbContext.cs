using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace shard_db;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
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


    // Metadata for controlling data locations
    public DbSet<QueryLog> QueryLog { get; set; }
    public DbSet<Fragment> Fragment { get; set; }
    public DbSet<Site> Site { get; set; }
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

public class QueryLog
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int FragmentId { get; set; }

    [Required]
    public int SiteId { get; set; }

    [Required]
    public DateTime AccessDate { get; set; }

    [Required]
    public DATA_TYPE DataType { get; set; }

    [Required]
    public int DataVolume { get; set; }     // Amount of data transferred with the request, in bytes

    [ForeignKey("FragmentId")]
    public Fragment Fragment { get; set; } = null!;

    [ForeignKey("SiteId")]
    public Site Site { get; set; } = null!;
}

public class Fragment
{
    [Key]
    public int Id { get; set; }
    public int SiteId { get; set; }

    [ForeignKey("SiteId")]
    public Site site { get; set; } = null!;
}

public class Site
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Required]
    public string Name { get; set; } = null!;
}

public enum DATA_TYPE
{
    READ,
    WRITE
}