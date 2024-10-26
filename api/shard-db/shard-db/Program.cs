using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => {
    options.UseSqlite("Data Source=database.db");
});

var app = builder.Build();

app.UseHttpsRedirection();

app.Run();