using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleTelemetry.Data.Context;
using VehicleTelemetry.Domain.Entities;

namespace VehicleTelemetry.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiclesController : ControllerBase
    {
        private readonly TelemetryDbContext _db;

        public VehiclesController(TelemetryDbContext db)
        {
            _db = db;
        }

        // GET: api/vehicles
        [HttpGet]
        public async Task<ActionResult<List<Vehicle>>> GetAll()
        {
            var vehicles = await _db.Vehicles
                .Include(v => v.TelemetryRecords)
                .ToListAsync();

            return Ok(vehicles);
        }

        // GET: api/vehicles/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Vehicle>> GetById(Guid id)
        {
            var vehicle = await _db.Vehicles
                .Include(v => v.TelemetryRecords)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vehicle is null)
                return NotFound();

            return Ok(vehicle);
        }

        // GET: api/vehicles/by-plate/{plateNumber}
        [HttpGet("by-plate/{plateNumber}")]
        public async Task<ActionResult<Vehicle>> GetByPlate(string plateNumber)
        {
            var vehicle = await _db.Vehicles
                .Include(v => v.TelemetryRecords)
                .FirstOrDefaultAsync(v => v.PlateNumber == plateNumber);

            if (vehicle is null)
                return NotFound();

            return Ok(vehicle);
        }

        public class CreateVehicleRequest
        {
            public string PlateNumber { get; set; } = null!;
            public string Manufacturer { get; set; } = null!;
            public string Model { get; set; } = null!;
            public int Year { get; set; }
            public string? DriverName { get; set; }
            public string? Description { get; set; }
        }

        // POST: api/vehicles
        [HttpPost]
        public async Task<ActionResult<Vehicle>> Create([FromBody] CreateVehicleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Plaka zaten var mı?
            var existing = await _db.Vehicles
                .FirstOrDefaultAsync(v => v.PlateNumber == request.PlateNumber);

            if (existing is not null)
            {
                return Conflict($"Vehicle with plate '{request.PlateNumber}' already exists.");
            }

            var vehicle = new Vehicle
            {
                PlateNumber = request.PlateNumber,
                Manufacturer = request.Manufacturer,
                Model = request.Model,
                Year = request.Year,
                DriverName = request.DriverName,
                Description = request.Description
            };

            _db.Vehicles.Add(vehicle);
            await _db.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = vehicle.Id },
                vehicle);
        }
    }
}
