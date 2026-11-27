using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpsRescue.Interface;

namespace OpsRescue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimulationController : ControllerBase
    {
        private readonly IRecoverySimulationService _simulationService;

        public SimulationController(IRecoverySimulationService simulationService)
        {
            _simulationService = simulationService;
        }

        /// <summary>
        /// Simulates and compares recovery options side-by-side (including continuation/do-nothing) on cost, time, and SLA outcome from MySQL.
        /// Returns 404 Not Found if the incident ID does not exist.
        /// </summary>
        /// <param name="incidentId">The incident ID (e.g. INC-1001, INC-1004)</param>
        [HttpGet("{incidentId}")]
        [Authorize]
        public async Task<IActionResult> GetRecoverySimulation(string incidentId)
        {
            if (string.IsNullOrWhiteSpace(incidentId))
                return BadRequest(new { message = "Incident ID must be provided." });

            var result = await _simulationService.GetSimulationStrategiesByIncidentIdAsync(incidentId);
            if (result == null)
            {
                return NotFound(new { message = $"Incident '{incidentId}' not found." });
            }

            return Ok(result);
        }
    }
}
