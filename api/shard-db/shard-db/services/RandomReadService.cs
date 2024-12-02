using Microsoft.EntityFrameworkCore;
using shard_db.dto;

namespace shard_db.services;

public class RandomReadService
{
    public class ReadFrequency
    {
        public Site Site { get; set; } = null!;
        public Device Device { get; set; } = null!;
        public DeviceDbContext Context { get; set; } = null!;
        public int FrequencyValue { get; set; }     // The time between writes for a device, in milliseconds
    }

    private List<ReadFrequency> frequencies = new List<ReadFrequency>();
    private Random random = new Random();
    private readonly IServiceProvider _serviceProvider;
    public RandomReadService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _ = InitReads();
    }

    public async Task<List<ReadFrequencyMatrixDto>> GetReadFrequencies()
    {
        var matrix = new List<ReadFrequencyMatrixDto>();
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DatabaseManager>();
            var sites = await context.BookKeepingDbContext.Site.ToListAsync();
            foreach (var site in sites)
            {
                var siteFreq = new ReadFrequencyMatrixDto();
                siteFreq.SiteId = site.Id;
                siteFreq.SiteName = site.Name;
                var frequenciesForSite = frequencies.Where(f => f.Site.Id == site.Id).ToList();
                var deviceReadFrequencies = new List<DeviceReadFrequency>();
                foreach (var frequency in frequenciesForSite)
                {
                    deviceReadFrequencies.Add(new DeviceReadFrequency
                    {
                        DeviceId = frequency.Device.Id,
                        DeviceName = frequency.Device.Name,
                        Frequency = frequency.FrequencyValue
                    });
                }

                siteFreq.DeviceReadFrequencies = deviceReadFrequencies;
                matrix.Add(siteFreq);
            }
        }

        return matrix;
    }

    public void SetReadFrequency(string deviceId, int frequencyValue)
    {
        var device = frequencies.First(f => f.Device.Id == deviceId);
        if (device != null)
        {
            device.FrequencyValue = frequencyValue;
        }
    }

    private async Task InitReads()
    {
        await SetReadFrequencies(); 

        foreach(var frequency in frequencies)
        {
            Console.WriteLine("tes2t");
            ReadLoop(frequency);
        }
    }

    private async Task SetReadFrequencies()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DatabaseManager>();
            var sites = await context.BookKeepingDbContext.Site.ToListAsync();
            var frequencies = new List<ReadFrequency>();
            foreach (var site in sites)
            {
                foreach (var deviceContext in context.DeviceDbContexts)
                {
                    var list = await deviceContext.Device.Include(d => d.Sensors).ToListAsync();
                    var formattedList = list.Select(l => new ReadFrequency
                        { Device = l, FrequencyValue = 1000, Context = deviceContext });
                    frequencies.AddRange(formattedList);
                }
            }
        }
    }

    private async void ReadLoop(ReadFrequency frequency)
    {
        await frequency.Context.Device.Where(d => d.Id == frequency.Device.Id)
            .Include(s => s.Sensors)
            .ThenInclude(sd => sd.SensorDatas)
            .ToListAsync();
        
        await Task.Delay(frequency.FrequencyValue);
        ReadLoop(frequency);
    }

}