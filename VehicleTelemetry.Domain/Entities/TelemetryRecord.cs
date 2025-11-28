namespace VehicleTelemetry.Domain.Entities
{
    public class TelemetryRecord
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;

        public DateTime TimestampUtc { get; set; }

        // Temel telemetri alanları
        public double SpeedKph { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public double? FuelLevelPercent { get; set; }
        public double? EngineTemperatureC { get; set; }

        // Gerekirse ham payload saklayabileceğimiz alan
        public string? RawPayload { get; set; }
    }
}
