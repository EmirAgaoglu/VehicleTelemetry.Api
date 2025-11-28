namespace VehicleTelemetry.Domain.Dtos
{
    public class TelemetryInputDto
    {
        public string PlateNumber { get; set; } = null!;

        public double SpeedKph { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public double? FuelLevelPercent { get; set; }
        public double? EngineTemperatureC { get; set; }

        // Araç zamanı gönderebilir, göndermezse server Now kullanırız
        public DateTime? TimestampUtc { get; set; }
    }
}
