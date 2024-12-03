using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using shard_db.dto;

namespace shard_db.services;

public class RandomReadService
{
    public class ReadFrequency
    {
        public Site RequestingSite { get; set; } = null!;
        public Site AssignedSite { get; set; } = null!;
        public Device Device { get; set; } = null!;
        public int FrequencyValue { get; set; }     // The time between writes for a device, in milliseconds
    }

    private List<ReadFrequency> frequencies = new List<ReadFrequency>();
    private readonly IServiceProvider _serviceProvider;
    public RandomReadService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public List<ReadFrequencyMatrixDto> GetReadFrequencies()
    {
        var matrix = new List<ReadFrequencyMatrixDto>();
        foreach (var frequency in frequencies)
        {
            var site = matrix.Where(l => l.SiteId == frequency.RequestingSite.Id)
                .FirstOrDefault(new ReadFrequencyMatrixDto
                {
                    SiteId = frequency.RequestingSite.Id, 
                    SiteName = frequency.RequestingSite.Name
                });
            
            var device = new DeviceReadFrequency { 
                DeviceId = frequency.Device.Id, 
                DeviceName = frequency.Device.Name, 
                FrequencyValue = frequency.FrequencyValue 
            };
            site.Frequencies.Add(device);
            if (!matrix.Contains(site))
            {
                matrix.Add(site);
            }
        }

        return matrix;
    }

    public void SetReadFrequency(string deviceId, int siteId, int frequencyValue)
    {
        var device = frequencies.First(f => f.Device.Id == deviceId && f.RequestingSite.Id == siteId);
        if (device != null)
        {
            device.FrequencyValue = frequencyValue;
        }
    }

    public async Task InitReads()
    {
        await SetReadFrequencies(); 

        foreach(var frequency in frequencies)
        {
            ReadLoop(frequency);
        }
    }

    private async Task SetReadFrequencies()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DatabaseManager>();
            foreach (var site in context.BookKeepingDbContext.Site.ToList())
            {
                foreach (var deviceContext in context.DeviceDbContexts)
                {
                    var list = await deviceContext.Device.Include(d => d.Sensors).ToListAsync();
                    var assignedSite = (await context.BookKeepingDbContext.Site.FindAsync(deviceContext.SiteId))!;
                    var formattedList = list.Select(l => new ReadFrequency { Device = l, RequestingSite = site, AssignedSite = assignedSite, FrequencyValue = 10000 });
                    frequencies.AddRange(formattedList);
                    // var formattedList = list.Select(l => new ReadFrequency
                    //     { Device = l, FrequencyValue = 1000, Context = deviceContext, Site = site});
                    // frequencies.AddRange(formattedList);
                }
            }
        }
    }

    private async void ReadLoop(ReadFrequency frequency)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var deviceDbOptionsBuilder = new DbContextOptionsBuilder<DeviceDbContext>();
            deviceDbOptionsBuilder.UseSqlite($"Data Source=./databases/{frequency.AssignedSite.Name}.db");
            var context = new DeviceDbContext(deviceDbOptionsBuilder.Options, frequency.AssignedSite.Id);
            var res = await context.Device.Where(d => d.Id == frequency.Device.Id)
                .Include(s => s.Sensors)
                .ThenInclude(sd => sd.SensorDatas)
                .ToListAsync();

            var bkDbContextOptionsBuilder = new DbContextOptionsBuilder<BookKeepingDbContext>();
            bkDbContextOptionsBuilder.UseSqlite("Data Source=./databases/BookKeeping.db");

            var bookKeepingDbContext = new BookKeepingDbContext(bkDbContextOptionsBuilder.Options);
            var entry = new QueryLog
            {
                DeviceId = frequency.Device.Id,
                SiteId = frequency.RequestingSite.Id,
                AccessDate = DateTime.UtcNow,
                DataType = DATA_TYPE.READ,
                DataVolume = JsonSerializer.SerializeToUtf8Bytes(res).Length
            };
            bookKeepingDbContext.Add(entry);
            bookKeepingDbContext.SaveChanges();

            // Console.WriteLine($"Reading For: {frequency.RequestingSite.Name} for {frequency.AssignedSite.Name}. Device: {frequency.Device.Name}");
            // Console.WriteLine($"{JsonSerializer.Serialize(res)}");
            await Task.Delay(frequency.FrequencyValue);
            ReadLoop(frequency);
        }
    }

}