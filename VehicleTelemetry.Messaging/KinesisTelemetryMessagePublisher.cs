using System.Text;
using System.Text.Json;
using Amazon;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VehicleTelemetry.Domain.Entities;
using VehicleTelemetry.Messaging.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace VehicleTelemetry.Messaging
{
    public class KinesisTelemetryMessagePublisher : ITelemetryMessagePublisher
    {
        private readonly ILogger<KinesisTelemetryMessagePublisher> _logger;
        private readonly IAmazonKinesis _kinesisClient;
        private readonly string _streamName;

        public KinesisTelemetryMessagePublisher(
            ILogger<KinesisTelemetryMessagePublisher> logger,
            IConfiguration configuration)
        {
            _logger = logger;

            var region = configuration["AWS:Region"] ?? "eu-central-1";
            _streamName = configuration["AWS:KinesisStreamName"] ?? "vehicle-telemetry-stream";

            // Not: AccessKey/SecretKey yoksa, AWS default credential chain'i kullanır
            _kinesisClient = new AmazonKinesisClient(RegionEndpoint.GetBySystemName(region));
        }

        public async Task PublishAsync(TelemetryRecord record, CancellationToken cancellationToken = default)
        {
            try
            {
                var payload = JsonSerializer.Serialize(new
                {
                    record.VehicleId,
                    record.TimestampUtc,
                    record.SpeedKph,
                    record.Latitude,
                    record.Longitude,
                    record.FuelLevelPercent,
                    record.EngineTemperatureC
                });

                var bytes = Encoding.UTF8.GetBytes(payload);

                var request = new PutRecordRequest
                {
                    StreamName = _streamName,
                    PartitionKey = record.VehicleId.ToString(),
                    Data = new MemoryStream(bytes)
                };

                var response = await _kinesisClient.PutRecordAsync(request, cancellationToken);

                _logger.LogInformation(
                    "Telemetry sent to Kinesis. VehicleId={VehicleId}, ShardId={ShardId}, SequenceNumber={Sequence}",
                    record.VehicleId,
                    response.ShardId,
                    response.SequenceNumber);
            }
            catch (Exception ex)
            {
                // Ödev/demo için: mesajlaşma başarısız olsa bile API çökmüyor
                _logger.LogError(ex,
                    "Failed to send telemetry to Kinesis for VehicleId={VehicleId}", record.VehicleId);
            }
        }

    }
}
