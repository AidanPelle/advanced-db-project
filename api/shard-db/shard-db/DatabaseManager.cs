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
        bkDbContextOptionsBuilder.EnableSensitiveDataLogging();
        
        BookKeepingDbContext = new BookKeepingDbContext(bkDbContextOptionsBuilder.Options);
        BookKeepingDbContext.Database.EnsureDeleted();
        BookKeepingDbContext.Database.EnsureCreated();
        // BookKeepingDbContext.Database.Migrate();
            
        foreach (var siteModel in sites)
        {
            var site = new Site();
            site.Name = siteModel.Name;
            BookKeepingDbContext.Add(site);
            BookKeepingDbContext.SaveChanges();
            
            var deviceDbOptionsBuilder = new DbContextOptionsBuilder<DeviceDbContext>();
            deviceDbOptionsBuilder.UseSqlite($"Data Source=./databases/{siteModel.Name}.db");
            var deviceContext = new DeviceDbContext(deviceDbOptionsBuilder.Options, site.Id);
            deviceContext.Database.EnsureDeleted();
            deviceContext.Database.EnsureCreated();
            DeviceDbContexts.Add(deviceContext);

            foreach (var deviceModel in siteModel.Devices)
            {
                var device = new Device();
                device.Id = Guid.NewGuid().ToString();
                device.Name = deviceModel.Name;
                var context = DeviceDbContexts.Last();
                context.Device.Add(device);
                    
                foreach (var sensorModel in deviceModel.Sensors)
                {
                    var sensor = new Sensor
                    {
                        Id = Guid.NewGuid().ToString(), 
                        Name = sensorModel.Name, 
                        DeviceId = device.Id, 
                        Units = sensorModel.Units
                    };
                    context.Sensor.Add(sensor);
                }
                deviceContext.SaveChanges();
                
                var siteDevice = new SiteDevice();
                siteDevice.SiteId = site.Id;
                siteDevice.DeviceId = device.Id;
                BookKeepingDbContext.Add(siteDevice);
                BookKeepingDbContext.SaveChanges();
            }
        }
    }
    
    public BookKeepingDbContext BookKeepingDbContext { get; set; }
    public List<DeviceDbContext> DeviceDbContexts { get; set; } = new List<DeviceDbContext>();
}