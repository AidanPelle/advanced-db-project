using Microsoft.EntityFrameworkCore;

namespace shard_db.services;

public class DeviceDisributionService
{
    public DeviceDisributionService(DatabaseManager context, IServiceProvider serviceProvider)
    {
        _context = context.BookKeepingDbContext;
        _serviceProvider = serviceProvider;
        _ = WriteLoop(30_000);
    }
    private BookKeepingDbContext _context;
    private readonly IServiceProvider _serviceProvider;

    private async Task WriteLoop(int frequency)
    {
        Console.WriteLine("Starting Distribution Check");

        // For each device, we need to know percentage of data requested from each site.
        // If there any sites are requesting more data than the current assigned site, move the device to the highest data assigned site
        var pastThirtySeconds = DateTime.UtcNow.AddSeconds(-30);

        var deviceDict = new Dictionary<string, Dictionary<int, int>>();
        var queryLogs = await _context.QueryLog.Where(q => q.AccessDate > pastThirtySeconds).ToListAsync();
        foreach(var logItem in queryLogs)
        {
            if (deviceDict.ContainsKey(logItem.DeviceId))
            {
                if (deviceDict[logItem.DeviceId].ContainsKey(logItem.SiteId))
                {
                    deviceDict[logItem.DeviceId][logItem.SiteId] = deviceDict[logItem.DeviceId][logItem.SiteId] + logItem.DataVolume;
                }
                else {
                    deviceDict[logItem.DeviceId].Add(logItem.SiteId, logItem.DataVolume);
                }
            } else
            {
                var childDict = new Dictionary<int, int>
                {
                    { logItem.SiteId, logItem.DataVolume }
                };
                deviceDict.Add(logItem.DeviceId, childDict);
            }
        }

        foreach(var device in deviceDict)
        {
            // Key value pair of site ID and total data used by that site
            var largestSite = new KeyValuePair<int, int>(0, 0);
            foreach(var site in device.Value)
            {
                if (site.Value > largestSite.Value)
                    largestSite = site;
            };

            // If site wasn't read to during this timelapse, we skip
            if (largestSite.Value == 0)
                continue;
            
            var assignedSiteId = (await _context.SiteDevice.FirstAsync(sd => sd.DeviceId == device.Key)).SiteId;

            if (largestSite.Key == assignedSiteId)
                continue;

            // If the assigned site exists in the map (therefore has data), and the largest written site is not greater than the assigned site,
            // we will not have to move the data.
            if (device.Value.ContainsKey(assignedSiteId) && device.Value[assignedSiteId] * 1.5 > largestSite.Value)
            {
                continue;
            }

            // In this case, either the current assigned site has had no data written to it,
            // or some other site has written at least 50% more data. As a result, we need to move all data for this site,
            // and update the frequency mappings in RandomWriteService and RandomReadService
            Console.WriteLine($"Writing Device: {device.Key}, From Site: {assignedSiteId}, To Site: {largestSite.Key}");
            await MoveDeviceData(device.Key, assignedSiteId, largestSite.Key);
        }

        await Task.Delay(frequency);
        _ = WriteLoop(frequency);
    }

    private async Task MoveDeviceData(string deviceId, int fromSiteId, int toSiteId)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var fromSiteName = await _context.Site.FindAsync(fromSiteId);
            var toSiteName = await _context.Site.FindAsync(toSiteId);


            var fromSiteOptionsBuilder = new DbContextOptionsBuilder<DeviceDbContext>();
                fromSiteOptionsBuilder.UseSqlite($"Data Source=./databases/{fromSiteName}.db");
            var fromContext = new DeviceDbContext(fromSiteOptionsBuilder.Options, fromSiteId);


            var toSiteOptionsBuilder = new DbContextOptionsBuilder<DeviceDbContext>();
                toSiteOptionsBuilder.UseSqlite($"Data Source=./databases/{toSiteName}.db");
            var toContext = new DeviceDbContext(toSiteOptionsBuilder.Options, toSiteId);


            Device device = (await fromContext.Device.Include(d => d.Sensors).ThenInclude(s => s.SensorDatas).FirstOrDefaultAsync(d => d.Id == deviceId))!;
            toContext.Add(device);
        }
    }
}
