using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace shard_db;
public class DatabaseManager
{
    public DatabaseManager()
    {
            var jsonData = File.ReadAllText("./AppConf.json");
            var deSerializeOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var sites = JsonSerializer.Deserialize<List<SiteDto>>(jsonData, deSerializeOptions)!;

            var bkDbContextOptionsBuilder = new DbContextOptionsBuilder<BookKeepingDbContext>();
            bkDbContextOptionsBuilder.UseSqlite("Data Source=./databases/BookKeeping.db");
            BookKeepingDbContext = new BookKeepingDbContext(bkDbContextOptionsBuilder.Options);
            BookKeepingDbContext.Database.EnsureCreated();
            // BookKeepingDbContext.Database.Migrate();
            
            foreach (var site in sites)
            {
                var deviceDbOptionsBuilder = new DbContextOptionsBuilder<DeviceDbContext>();
                deviceDbOptionsBuilder.UseSqlite($"Data Source=./databases/{site.Name}.db");
                var deviceContext = new DeviceDbContext(deviceDbOptionsBuilder.Options);
                deviceContext.Database.EnsureCreated();
                DeviceDbContexts.Add(deviceContext);

                foreach (var deviceModel in site.Devices)
                {
                    var device = new Device();
                    
                    // DeviceDbContexts.Last().Device
                }
            }
    }
    
    public BookKeepingDbContext BookKeepingDbContext { get; set; }
    public List<DeviceDbContext> DeviceDbContexts { get; set; } = new List<DeviceDbContext>();
}