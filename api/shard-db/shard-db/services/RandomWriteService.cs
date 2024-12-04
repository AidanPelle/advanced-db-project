using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace shard_db.services;

public class RandomWriteService
{
    public class WriteFrequency
    {
        public Device Device { get; set; } = null!;
        public Site RequestingSite { get; set; } = null!;
        public Site AssignedSite { get; set; } = null!;
        public int FrequencyValue { get; set; }     // The time between writes for a device, in milliseconds
    }

    private List<WriteFrequency> frequencies = [];
    private Random random = new Random();
    private readonly IServiceProvider _serviceProvider;
    public RandomWriteService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void SetWriteFrequency(string deviceId, int siteId, int frequencyValue)
    {
        var device = frequencies.First(f => f.Device.Id == deviceId && f.RequestingSite.Id == siteId);
        if (device != null)
        {
            device.FrequencyValue = frequencyValue;
        }
    }

    public async Task InitWrites()
    {
        await SetWriteFrequencies();

        foreach (var frequency in frequencies)
        {
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
                    var assignedSite = context.BookKeepingDbContext.Site.Find(deviceContext.SiteId)!;
                    var formattedList = list.Select(l => new WriteFrequency { Device = l, RequestingSite = site, AssignedSite = assignedSite, FrequencyValue = 5000 });
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
            var site = list.Where(l => l.SiteId == frequencyDto.RequestingSite.Id).FirstOrDefault(new WriteFrequencyMatrixDto { SiteId = frequencyDto.RequestingSite.Id, SiteName = frequencyDto.RequestingSite.Name });
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
        try {
            foreach (var sensor in frequency.Device.Sensors) {
                var data = new SensorData {
                    SensorId = sensor.Id,
                    ReceivedTimestamp = DateTime.UtcNow,
                    Value = random.NextDouble() * 1000,
                };
                using (var scope = _serviceProvider.CreateScope()) {
                    try {
                        var deviceDbOptionsBuilder = new DbContextOptionsBuilder<DeviceDbContext>();
                        deviceDbOptionsBuilder.UseSqlite($"Data Source=./databases/{frequency.AssignedSite.Name}.db");
                        var context = new DeviceDbContext(deviceDbOptionsBuilder.Options, frequency.AssignedSite.Id);
                        context.Add(data);

                        await context.SaveChangesAsync();

                        var bkDbContextOptionsBuilder = new DbContextOptionsBuilder<BookKeepingDbContext>();
                        bkDbContextOptionsBuilder.UseSqlite("Data Source=./databases/BookKeeping.db");

                        var bookKeepingDbContext = new BookKeepingDbContext(bkDbContextOptionsBuilder.Options);
                        bookKeepingDbContext.Add(new QueryLog {
                            DeviceId = frequency.Device.Id,
                            SiteId = frequency.RequestingSite.Id,
                            AccessDate = DateTime.UtcNow,
                            DataType = DATA_TYPE.WRITE,
                            DataVolume = JsonSerializer.SerializeToUtf8Bytes(data).Length
                        });
                        bookKeepingDbContext.SaveChanges();
                        // Console.WriteLine($"Site Requesting: {frequency.RequestingSite.Name}, Device: {frequency.Device.Name}, Sensor: {sensor.Name}");
                    }
                    catch (Exception error) {
                        Console.WriteLine(error);
                    }

                }
            }
        }
        catch (InvalidOperationException ex) {
            Console.WriteLine(ex);
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