using VehicleTelemetry.Domain.Entities;

namespace VehicleTelemetry.Data.Repositories
{
    public interface IVehicleRepository
    {
        Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Vehicle?> GetByPlateAsync(string plateNumber, CancellationToken cancellationToken = default);
        Task<List<Vehicle>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<Vehicle> AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
