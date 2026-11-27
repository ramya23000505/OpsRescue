using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpsRescue.Interface;

namespace OpsRescue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SlaRiskController : ControllerBase
    {
        private readonly ISlaRiskService _slaRiskService;

        public SlaRiskController(ISlaRiskService slaRiskService)
        {
            _slaRiskService = slaRiskService;
        }

        /// <summary>
        /// Calculates and returns quantitative SLA breach probability, delay projection, and contributing risk factors from MySQL.
        /// Returns 404 Not Found if the incident ID does not exist.
        /// </summary>
        /// <param name="incidentId">The incident ID (e.g. INC-1001, INC-1004)</param>
        [HttpGet("{incidentId}")]
        [Authorize]
        public async Task<IActionResult> GetSlaRiskAnalysis(string incidentId)
        {
            if (string.IsNullOrWhiteSpace(incidentId))
                return BadRequest(new { message = "Incident ID must be provided." });

            var result = await _slaRiskService.GetSlaRiskAnalysisByIncidentIdAsync(incidentId);
            if (result == null)
            {
                return NotFound(new { message = $"Incident '{incidentId}' not found." });
            }

            return Ok(result);
        }
    }
}
