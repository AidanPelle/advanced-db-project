using Microsoft.EntityFrameworkCore;

namespace shard_db.services;

public class RandomWriteService
{
    public class WriteFrequency
    {
        public Site Site { get; set; } = null!;
        public Device Device { get; set; } = null!;
        public int FrequencyValue { get; set; }     // The time between writes for a device, in milliseconds
    }

    private List<WriteFrequency> frequencies = [];
    private Random random = new Random();
    private readonly IServiceProvider _serviceProvider;
    public RandomWriteService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _ = InitWrites();
    }

    public void SetWriteFrequency(string deviceId, int siteId, int frequencyValue)
    {
        var device = frequencies.First(f => f.Device.Id == deviceId && f.Site.Id == siteId);
        if (device != null)
        {
            device.FrequencyValue = frequencyValue;
        }
    }

    private async Task InitWrites()
    {
        await SetWriteFrequencies();

        foreach (var frequency in frequencies)
        {
            Console.WriteLine("tes2t");
            WriteLoop(frequency);
        }
    }

    private async Task SetWriteFrequencies()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DatabaseManager>();
            foreach (var site in context.BookKeepingDbContext.Site)
            {
                foreach (var deviceContext in context.DeviceDbContexts)
                {
                    var list = await deviceContext.Device.Include(d => d.Sensors).ToListAsync();
                    var formattedList = list.Select(l => new WriteFrequency { Device = l, Site = site, FrequencyValue = 10000 });
                    frequencies.AddRange(formattedList);
                }
            }
        }
    }

    public List<WriteFrequencyMatrixDto> GetWriteFrequencies()
    {
        var list = new List<WriteFrequencyMatrixDto>();
        foreach (var frequencyDto in frequencies)
        {
            var site = list.Where(l => l.SiteId == frequencyDto.Site.Id).FirstOrDefault(new WriteFrequencyMatrixDto { SiteId = frequencyDto.Site.Id, SiteName = frequencyDto.Site.Name });
            var device = new WriteFrequencyDevice { DeviceId = frequencyDto.Device.Id, DeviceName = frequencyDto.Device.Name, FrequencyValue = frequencyDto.FrequencyValue };
            site.Frequencies.Add(device);
            if (!list.Contains(site))
            {
                list.Add(site);
            }
        }
        return list;
    }

    private async void WriteLoop(WriteFrequency frequency)
    {
        foreach (var sensor in frequency.Device.Sensors)
        {
            var data = new SensorData
            {
                SensorId = sensor.Id,
                ReceivedTimestamp = DateTime.UtcNow,
                Value = random.NextDouble() * 1000,
            };
            using (var scope = _serviceProvider.CreateScope())
            {
                var deviceDbOptionsBuilder = new DbContextOptionsBuilder<DeviceDbContext>();
                deviceDbOptionsBuilder.UseSqlite($"Data Source=./databases/{frequency.Site.Name}.db");
                var context = new DeviceDbContext(deviceDbOptionsBuilder.Options, frequency.Site.Id);
                context.Add(data);
                try {
                    await context.SaveChangesAsync();
                }
                catch (Exception error) {
                    Console.WriteLine(error);
                }
                
            }
        }
        await Task.Delay(frequency.FrequencyValue);
        WriteLoop(frequency);
    }

}

public class WriteFrequencyMatrixDto
{
    public int SiteId { get; set; }
    public string SiteName { get; set; } = null!;
    public List<WriteFrequencyDevice> Frequencies { get; set; } = [];
}

public class WriteFrequencyDevice
{
    public string DeviceId { get; set; } = null!;
    public string DeviceName { get; set; } = null!;
    public int FrequencyValue { get; set; }
}