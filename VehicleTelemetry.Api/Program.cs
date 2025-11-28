using Microsoft.EntityFrameworkCore;
using VehicleTelemetry.Data.Context;
using VehicleTelemetry.Data.Repositories;
using VehicleTelemetry.Messaging;
using VehicleTelemetry.Messaging.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// EF Core DbContext
builder.Services.AddDbContext<TelemetryDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("TelemetryDatabase");
    options.UseSqlServer(connectionString);
});

// Repository DI
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<ITelemetryRepository, TelemetryRepository>();

// Messaging DI
builder.Services.AddScoped<ITelemetryMessagePublisher, KinesisTelemetryMessagePublisher>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
