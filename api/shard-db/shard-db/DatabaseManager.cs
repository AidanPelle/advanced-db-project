using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace shard_db;
public class DatabaseManager
{
    DatabaseManager()
    {
            
    }
    
    public BookKeepingDbContext BookKeepingDbContext { get; set; }
    public List<DeviceDbContext> DeviceDbContexts { get; set; }
}