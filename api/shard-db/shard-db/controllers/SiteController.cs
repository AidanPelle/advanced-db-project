using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shard_db.services;
using shard_db.dto;

namespace shard_db.controllers;

[Route("[controller]")]
[EnableCors("AllowAll")]
[ApiController]
public class SitesController(DatabaseManager context, RandomWriteService randomWriteService, RandomReadService randomReadService) : ControllerBase
{
    [HttpGet("all")]
    public async Task<ActionResult<List<Site>>> GetSites()
    {
        var sites = await context.BookKeepingDbContext.Site.ToListAsync();
        return Ok(sites);
    }

    [HttpGet("write-matrix")]
    public ActionResult<List<WriteFrequencyMatrixDto>> GetWriteMatrix()
    {
        var frequencyMatrixDtos = randomWriteService.GetWriteFrequencies();
        return Ok(frequencyMatrixDtos);
    }

    [HttpPost("write-matrix")]
    public ActionResult SetWriteFrequency([FromBody] UpdateFrequencyValDto frequencyValDto)
    {
        randomWriteService.SetWriteFrequency(frequencyValDto.DeviceId, frequencyValDto.SiteId, frequencyValDto.Value);
        return NoContent();
    }
    
    [HttpGet("read-matrix")]
    public ActionResult<List<ReadFrequencyMatrixDto>> GetReadMatrix()
    {
        var frequencyMatrixDtos = randomReadService.GetReadFrequencies();
        return Ok(frequencyMatrixDtos);
    }
    
    [HttpPost("read-matrix")]
    public ActionResult SetReadFrequency([FromBody] UpdateFrequencyValDto frequencyValDto)
    {
        randomReadService.SetReadFrequency(frequencyValDto.DeviceId, frequencyValDto.SiteId, frequencyValDto.Value);
        return NoContent();
    }

    [HttpGet("device-distribution")]
    public async Task<ActionResult> GetDeviceDistribution()
    {
        var sites = await context.BookKeepingDbContext.Site.ToListAsync();
        var siteDistributions = new List<DeviceSiteDistributionDto>();
        foreach (var site in sites)
        {
            var deviceContext = context.DeviceDbContexts.Find(dc => dc.SiteId == site.Id)!;
            siteDistributions.Add(new DeviceSiteDistributionDto
            {
                SiteName = site.Name,
                DeviceNames = await deviceContext.Device.Select(d => d.Name).ToListAsync()
            });
        }
        return Ok(siteDistributions);
    }

    [HttpGet("device-site-usage")]
    public async Task<ActionResult> GetTopDevices([FromQuery] int offset)
    {
        var pastThirtySeconds = DateTime.UtcNow.AddMilliseconds(-offset * 1000);

        var devices = context.DeviceDbContexts.SelectMany(d => d.Device.ToList()).ToList();
        var deviceSiteUsages = new List<DeviceSiteDataUsageDto>();
        foreach (var device in devices)
        {
            var deviceDict = new Dictionary<int, SiteDataUsageDto>();
            var queryLogs = await context.BookKeepingDbContext.QueryLog
                .Where(q => q.DeviceId == device.Id && q.AccessDate > pastThirtySeconds)
                .Include(s => s.Site)
                .ToListAsync();
            
            var deviceSiteUsage = new DeviceSiteDataUsageDto();
            deviceSiteUsage.DeviceName = device.Name;
            foreach (var logItem in queryLogs)
            {
                if (deviceDict.ContainsKey(logItem.SiteId))
                {
                    if (logItem.DataType == DATA_TYPE.READ)
                    {
                        deviceDict[logItem.SiteId].ReadUsage += logItem.DataVolume;
                        // deviceDict[logItem.DeviceId][logItem.SiteId] = 
                    }
                    else
                    {
                        deviceDict[logItem.SiteId].WriteUsage += logItem.DataVolume;
                    }
                }
                else
                {
                    deviceDict.Add(logItem.SiteId, new SiteDataUsageDto
                    {
                        SiteName = logItem.Site.Name,
                        ReadUsage = logItem.DataType == DATA_TYPE.READ ? logItem.DataVolume : 0,
                        WriteUsage = logItem.DataType == DATA_TYPE.WRITE ? logItem.DataVolume : 0,
                    });
                }
            }

            
            foreach (var kvp in deviceDict)
            {
                deviceSiteUsage.siteUsage.Add(kvp.Value);
            }
            deviceSiteUsages.Add(deviceSiteUsage);
        }
        
        
        return Ok(deviceSiteUsages);
    }

    class DeviceSiteDataUsageDto
    {
        public string DeviceName { get; set; } = null!;
        public List<SiteDataUsageDto> siteUsage { get; set; } = [];
    }

    class SiteDataUsageDto
    {
        public string SiteName { get; set; } = null!;
        public int ReadUsage { get; set; }
        public int WriteUsage { get; set; }
    }

    class DeviceSiteDistributionDto
    {
        public string SiteName { get; set; } = null!;
        public List<string> DeviceNames { get; set; } = [];
    }

    class DeviceLoadBySiteDto
    {
        public string DeviceName { get; set; } = null!;
        public List<SiteLoadDto> Sites { get; set; } = [];
    }

    class SiteLoadDto
    {
        public string SiteName { get; set; } = null!;
        public double Load { get; set; }
    }
}