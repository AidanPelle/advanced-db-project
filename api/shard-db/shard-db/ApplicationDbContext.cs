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

    // Metadata for controlling data locations
    public DbSet<QueryLog> QueryLog { get; set; }
    public DbSet<Site> Site { get; set; }
    public DbSet<SiteDevice> SiteDevice { get; set; }
}

// TOP LEVEL DATABASE
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


    [ForeignKey("SiteId")]
    public Site Site { get; set; } = null!;
}

public class Site
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Required]
    public string Name { get; set; } = null!;
}

// Contains where each device is, in that a site represents a database
public class SiteDevice
{
    public int SiteId { get; set; }
    public int DeviceId { get; set; }
}

public enum DATA_TYPE
{
    READ,
    WRITE
}