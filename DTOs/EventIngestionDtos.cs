using System.ComponentModel.DataAnnotations;

namespace OpsRescue.DTOs
{
    public class SimulateAnomalyRequestDto
    {
        [Required]
        public string EventSource { get; set; } = "Carrier IoT Telematics (TRK-882)";

        [Required]
        public string AnomalyType { get; set; } = "Temperature Excursion";

        [Required]
        public string Severity { get; set; } = "CRITICAL P1";

        public string CorridorLocation { get; set; } = "NH-544 KM 88.2";

        public string SensorReading { get; set; } = "Current: +5.2°C (Threshold: +2.0°C to +8.0°C)";

        public string Description { get; set; } = "Thermal spike detected due to compressor intermittent cutoff.";
    }

    public class SimulateAnomalyResponseDto
    {
        public string EventId { get; set; } = string.Empty;
        public string IncidentId { get; set; } = string.Empty;
        public string Status { get; set; } = "Exception Detected & Triage Initiated";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Message { get; set; } = string.Empty;
        public bool BroadcastedToControlTower { get; set; } = true;
    }
}
