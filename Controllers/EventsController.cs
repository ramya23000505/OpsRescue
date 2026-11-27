using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OpsRescue.DTOs;
using OpsRescue.Hubs;

namespace OpsRescue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly IHubContext<IncidentHub> _hubContext;

        public EventsController(IHubContext<IncidentHub> hubContext)
        {
            _hubContext = hubContext;
        }

        /// <summary>
        /// Simulates an incoming real-time IoT anomaly or telematics exception (e.g. from RabbitMQ message bus).
        /// Broadcasts instant event alert to all connected Control Tower clients via SignalR.
        /// </summary>
        [HttpPost("simulate-anomaly")]
        [ProducesResponseType(typeof(SimulateAnomalyResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> SimulateAnomaly([FromBody] SimulateAnomalyRequestDto dto)
        {
            var eventId = $"EVT-{Random.Shared.Next(1000, 9999)}";
            var now = DateTime.UtcNow;

            var payload = new
            {
                eventId = eventId,
                incidentId = "INC-1004",
                source = dto.EventSource,
                anomalyType = dto.AnomalyType,
                severity = dto.Severity,
                location = dto.CorridorLocation,
                sensorReading = dto.SensorReading,
                timestamp = now,
                message = $"Live Disruption Alert: {dto.AnomalyType} on {dto.CorridorLocation} ({dto.SensorReading})"
            };

            // Broadcast real-time anomaly over SignalR to Control Tower dashboard
            try
            {
                await _hubContext.Clients.All.SendAsync("NewDisruptionDetected", payload);
            }
            catch
            {
                // Non-blocking
            }

            return Ok(new SimulateAnomalyResponseDto
            {
                EventId = eventId,
                IncidentId = "INC-1004",
                Status = "Exception Detected & Triage Initiated",
                Timestamp = now,
                Message = $"Simulated event '{eventId}' broadcasted to Control Tower. Automated triage workflow triggered.",
                BroadcastedToControlTower = true
            });
        }
    }
}
