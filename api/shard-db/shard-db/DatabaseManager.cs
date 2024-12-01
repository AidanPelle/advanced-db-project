using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace shard_db;
public class DatabaseManager
{
    DatabaseManager()
    {

        
        

            var jsonData = File.ReadAllText("./AppConf.json");
            var sites = JsonSerializer.Deserialize<List<SiteDto>>(jsonData);

            var bkDbContextOptionsBuilder = new DbContextOptionsBuilder<BookKeepingDbContext>();
            bkDbContextOptionsBuilder.UseSqlite("Data Source=./BookKeeping.db");
            BookKeepingDbContext = new BookKeepingDbContext(bkDbContextOptionsBuilder.Options);
            // BookKeepingDbContext.Database.Migrate();
            
            foreach (var site in sites)
            {
                var deviceDbOptionsBuilder = new DbContextOptionsBuilder<DeviceDbContext>();
                deviceDbOptionsBuilder.UseSqlite($"Data Source={site.Name}.db");
                DeviceDbContexts.Add(new DeviceDbContext(deviceDbOptionsBuilder.Options));

                foreach (var deviceModel in site.Devices)
                {
                    var device = new Device();
                    
                    DeviceDbContexts.Last().Device
                }
            }
            
            
        
            // Add data to the database
            if (devices != null)
            {
                dbContext.Device.AddRange(devices);
                dbContext.SaveChanges();
            }
    }
    
    public BookKeepingDbContext BookKeepingDbContext { get; set; }
    public List<DeviceDbContext> DeviceDbContexts { get; set; } = new List<DeviceDbContext>();
}