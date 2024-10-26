using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using shard_db;
using shard_db.services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options => {
    options.UseSqlite("Data Source=database.db");
});

builder.Services.AddSingleton<RandomWriteService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();

    if (!dbContext.Device.Any())
    {
        var jsonData = File.ReadAllText("./InitData.json");
        var devices = JsonSerializer.Deserialize<List<Device>>(jsonData);

        // Add data to the database
        if (devices != null)
        {
            dbContext.Device.AddRange(devices);
            dbContext.SaveChanges();
        }
    }
}

app.MapControllers();

app.Run();