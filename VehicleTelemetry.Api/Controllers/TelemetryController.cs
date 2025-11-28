using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleTelemetry.Data.Context;
using VehicleTelemetry.Domain.Dtos;
using VehicleTelemetry.Domain.Entities;
using VehicleTelemetry.Messaging.Abstractions;

namespace VehicleTelemetry.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelemetryController : ControllerBase
    {
        private readonly TelemetryDbContext _db;
        private readonly ITelemetryMessagePublisher _publisher;
        private readonly ILogger<TelemetryController> _logger;

        public TelemetryController(
            TelemetryDbContext db,
            ITelemetryMessagePublisher publisher,
            ILogger<TelemetryController> logger)
        {
            _db = db;
            _publisher = publisher;
            _logger = logger;
        }

        // POST: api/telemetry
        [HttpPost]
        public async Task<IActionResult> Ingest([FromBody] TelemetryInputDto input, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1) Aracı plakaya göre bul
            var vehicle = await _db.Vehicles
                .FirstOrDefaultAsync(v => v.PlateNumber == input.PlateNumber, cancellationToken);

            // Yoksa otomatik oluştur
            if (vehicle is null)
            {
                vehicle = new Vehicle
                {
                    PlateNumber = input.PlateNumber,
                    Manufacturer = "Unknown",
                    Model = "Unknown",
                    Year = DateTime.UtcNow.Year,
                    Description = "Created automatically by TelemetryController"
                };

                _db.Vehicles.Add(vehicle);
                await _db.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Vehicle with plate {Plate} created automatically.", input.PlateNumber);
            }

            // 2) Telemetry kaydı oluştur
            var record = new TelemetryRecord
            {
                VehicleId = vehicle.Id,
                TimestampUtc = input.TimestampUtc ?? DateTime.UtcNow,
                SpeedKph = input.SpeedKph,
                Latitude = input.Latitude,
                Longitude = input.Longitude,
                FuelLevelPercent = input.FuelLevelPercent,
                EngineTemperatureC = input.EngineTemperatureC
            };

            _db.TelemetryRecords.Add(record);
            await _db.SaveChangesAsync(cancellationToken);

            // 3) Messaging katmanına gönder (Kinesis / fake vs.)
            await _publisher.PublishAsync(record, cancellationToken);

            _logger.LogInformation(
                "Telemetry ingested for vehicle {Plate} (VehicleId={VehicleId}, Speed={SpeedKph})",
                vehicle.PlateNumber,
                vehicle.Id,
                record.SpeedKph);

            return Accepted(new
            {
                VehicleId = vehicle.Id,
                vehicle.PlateNumber,
                TelemetryId = record.Id,
                record.TimestampUtc,
                record.SpeedKph
            });
        }
    }
}
