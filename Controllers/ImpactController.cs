using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpsRescue.Interface;

namespace OpsRescue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImpactController : ControllerBase
    {
        private readonly IImpactAnalysisService _impactService;

        public ImpactController(IImpactAnalysisService impactService)
        {
            _impactService = impactService;
        }

        /// <summary>
        /// Retrieves the comprehensive Impact Analysis & Causal Blast Radius for a given incident from MySQL.
        /// Returns 404 Not Found if the incident ID does not exist.
        /// </summary>
        /// <param name="incidentId">The incident ID (e.g. INC-1001, INC-1004)</param>
        [HttpGet("{incidentId}")]
        [Authorize]
        public async Task<IActionResult> GetImpactAnalysis(string incidentId)
        {
            if (string.IsNullOrWhiteSpace(incidentId))
                return BadRequest(new { message = "Incident ID must be provided." });

            var result = await _impactService.GetImpactAnalysisByIncidentIdAsync(incidentId);
            if (result == null)
            {
                return NotFound(new { message = $"Incident '{incidentId}' not found." });
            }

            return Ok(result);
        }

        /// <summary>
        /// Retrieves high-level impact summaries across all active corridor disruptions for the Control Tower.
        /// Driven by the same underlying MySQL records as the detail endpoint.
        /// </summary>
        [HttpGet("summary")]
        [Authorize]
        public async Task<IActionResult> GetActiveDisruptionsSummary()
        {
            var summaries = await _impactService.GetActiveDisruptionsImpactSummaryAsync();
            return Ok(summaries);
        }
    }
}
