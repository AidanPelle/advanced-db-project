using shard_db;
using shard_db.services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<DatabaseManager>();

builder.Services.AddSingleton<RandomWriteService>();
builder.Services.AddSingleton<RandomReadService>();
builder.Services.AddSingleton<DeviceDisributionService>();

builder.Services.AddCors(options => options.AddPolicy("AllowAll", p => { p.AllowAnyOrigin(); p.AllowAnyMethod(); p.AllowAnyHeader().WithExposedHeaders("content-disposition"); })); //allow CORS services

var app = builder.Build();
app.UseCors();

app.MapControllers();

app.Run();