using VehicleTelemetry.Domain.Entities;

namespace VehicleTelemetry.Data.Repositories
{
    public interface ITelemetryRepository
    {
        Task AddAsync(TelemetryRecord record, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
