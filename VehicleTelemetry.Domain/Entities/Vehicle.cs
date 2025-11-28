namespace VehicleTelemetry.Domain.Entities
{
    public class Vehicle
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string PlateNumber { get; set; } = null!; // Plaka
        public string Manufacturer { get; set; } = null!; // Marka
        public string Model { get; set; } = null!;        // Model
        public int Year { get; set; }                     // Model yılı

        public string? DriverName { get; set; }           // Şoför adı (opsiyonel)
        public string? Description { get; set; }          // Not/ açıklama

        // İleride TelemetryRecord ile ilişki kuracağız
        public ICollection<TelemetryRecord> TelemetryRecords { get; set; }
            = new List<TelemetryRecord>();
    }
}
