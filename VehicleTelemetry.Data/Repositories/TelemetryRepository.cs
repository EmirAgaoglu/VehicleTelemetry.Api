using VehicleTelemetry.Data.Context;
using VehicleTelemetry.Domain.Entities;

namespace VehicleTelemetry.Data.Repositories
{
    public class TelemetryRepository : ITelemetryRepository
    {
        private readonly TelemetryDbContext _context;

        public TelemetryRepository(TelemetryDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TelemetryRecord record, CancellationToken cancellationToken = default)
        {
            await _context.TelemetryRecords.AddAsync(record, cancellationToken);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
