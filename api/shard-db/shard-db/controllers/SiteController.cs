using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shard_db.services;
using shard_db.dto;

namespace shard_db.controllers;

[Route("[controller]")]
[EnableCors("AllowAll")]
[ApiController]
public class SitesController(DatabaseManager context) : ControllerBase
{
    [HttpGet("all")]
    public async Task<ActionResult<List<Site>>> GetSites()
    {
        var sites = await context.BookKeepingDbContext.Site.ToListAsync();
        return Ok(sites);
    }
}