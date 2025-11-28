using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleTelemetry.Data.Context;
using VehicleTelemetry.Domain.Entities;

namespace VehicleTelemetry.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DebugController : ControllerBase
    {
        private readonly TelemetryDbContext _db;
        private readonly IConfiguration _config;

        public DebugController(TelemetryDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        // 1) Şu an hangi connection string'i kullandığımızı görmek için
        [HttpGet("connection")]
        public IActionResult GetConnection()
        {
            var cs = _config.GetConnectionString("TelemetryDatabase");
            return Ok(new { ConnectionString = cs });
        }

        // 2) DB'ye test aracı ekleyen endpoint
        [HttpPost("seed-vehicle")]
        public async Task<IActionResult> SeedVehicle()
        {
            var v = new Vehicle
            {
                PlateNumber = "06DEBUG01",
                Manufacturer = "Debug",
                Model = "Test",
                Year = DateTime.UtcNow.Year,
                Description = "Seed from DebugController"
            };

            _db.Vehicles.Add(v);
            await _db.SaveChangesAsync();

            return Ok(new { v.Id, v.PlateNumber });
        }

        // 3) Kaç araç / kaç telemetry kaydı var görmek için
        [HttpGet("counts")]
        public async Task<IActionResult> Counts()
        {
            var vehicleCount = await _db.Vehicles.CountAsync();
            var telemetryCount = await _db.TelemetryRecords.CountAsync();

            return Ok(new { vehicleCount, telemetryCount });
        }
    }
}
