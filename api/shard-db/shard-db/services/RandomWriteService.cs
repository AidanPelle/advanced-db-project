using Microsoft.EntityFrameworkCore;

namespace shard_db.services;

public class RandomWriteService
{
    public class WriteFrequency
    {
        public Site Site { get; set; } = null!;
        public Device Device { get; set; } = null!;
        public DeviceDbContext Context { get; set; } = null!;
        public int FrequencyValue { get; set; }     // The time between writes for a device, in milliseconds
    }

    private List<WriteFrequency> frequencies = new List<WriteFrequency>();
    private Random random = new Random();
    private readonly IServiceProvider _serviceProvider;
    public RandomWriteService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _ = InitWrites();
    }

    public void SetWriteFrequency(string deviceId, int frequencyValue)
    {
        var device = frequencies.First(f => f.Device.Id == deviceId);
        if (device != null)
        {
            device.FrequencyValue = frequencyValue;
        }
    }

    private async Task InitWrites()
    {
        await SetWriteFrequencies(); 

        foreach(var frequency in frequencies)
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
            var frequencies = new List<WriteFrequency>();
                foreach(var site in context.BookKeepingDbContext.Site) {
                    foreach(var deviceContext in context.DeviceDbContexts) {
                    var list = await deviceContext.Device.Include(d => d.Sensors).ToListAsync();
                    var formattedList = list.Select(l => new WriteFrequency{Device = l, Site = site, FrequencyValue = 1000, Context = deviceContext});
                    frequencies.AddRange(formattedList);
                }
            }
        }
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
            frequency.Context.Add(data);
            frequency.Context.SaveChanges();
        }
        await Task.Delay(frequency.FrequencyValue);
        WriteLoop(frequency);
    }

}