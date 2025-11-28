using Microsoft.EntityFrameworkCore;
using VehicleTelemetry.Data.Context;
using VehicleTelemetry.Domain.Entities;

namespace VehicleTelemetry.Data.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly TelemetryDbContext _context;

        public VehicleRepository(TelemetryDbContext context)
        {
            _context = context;
        }

        public async Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Vehicles
                .Include(v => v.TelemetryRecords)
                .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
        }

        public async Task<Vehicle?> GetByPlateAsync(string plateNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Vehicles
                .Include(v => v.TelemetryRecords)
                .FirstOrDefaultAsync(v => v.PlateNumber == plateNumber, cancellationToken);
        }

        public async Task<List<Vehicle>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Vehicles
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Vehicle> AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
        {
            await _context.Vehicles.AddAsync(vehicle, cancellationToken);
            return vehicle;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
