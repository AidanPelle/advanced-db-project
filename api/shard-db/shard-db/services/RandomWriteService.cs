using Microsoft.EntityFrameworkCore;

namespace shard_db.services;

public class RandomWriteService
{
    public class WriteFrequency
    {
        public Device Device { get; set; } = null!;
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

    public void SetWriteFrequency(int deviceId, int frequencyValue)
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
            var devices = await context.DeviceDbContexts[0].Device.Include(d => d.Sensors).ToListAsync();
            frequencies = devices.Select(d => new WriteFrequency { Device = d, FrequencyValue = 1000 }).ToList();
        }
    }

    private async void WriteLoop(WriteFrequency frequency)
    {
        var sensorData = new List<SensorData>();
        foreach (var sensor in frequency.Device.Sensors)
        {
            var data = new SensorData
            {
                SensorId = sensor.Id,
                ReceivedTimestamp = DateTime.UtcNow,
                Value = random.NextDouble() * 1000,
            };
            sensorData.Add(data);
        }
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DatabaseManager>();
            await context.DeviceDbContexts[0].AddRangeAsync(sensorData);
            await context.DeviceDbContexts[0].SaveChangesAsync();
        }
        await Task.Delay(frequency.FrequencyValue);
        WriteLoop(frequency);
    }

}