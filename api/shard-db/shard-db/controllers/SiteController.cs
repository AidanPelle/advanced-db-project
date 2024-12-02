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
}