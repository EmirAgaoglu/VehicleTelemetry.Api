using VehicleTelemetry.Domain.Entities;

namespace VehicleTelemetry.Messaging.Abstractions
{
    public interface ITelemetryMessagePublisher
    {
        Task PublishAsync(TelemetryRecord record, CancellationToken cancellationToken = default);
    }
}
