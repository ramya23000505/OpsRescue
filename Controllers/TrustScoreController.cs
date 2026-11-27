using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpsRescue.DTOs;
using OpsRescue.Interface;

namespace OpsRescue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TrustScoreController : ControllerBase
    {
        private readonly ITrustScoreService _trustScoreService;

        public TrustScoreController(ITrustScoreService trustScoreService)
        {
            _trustScoreService = trustScoreService;
        }

        /// <summary>
        /// Retrieves the accountability and trust score for a specific incident (Module 9).
        /// Returns predicted vs actual recovery times and precision confidence.
        /// </summary>
        [HttpGet("{incidentId}")]
        [ProducesResponseType(typeof(TrustScoreDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTrustScore(string incidentId)
        {
            var result = await _trustScoreService.GetTrustScoreByIncidentAsync(incidentId);
            if (result == null)
            {
                return NotFound(new { message = $"No trust score evaluation recorded for incident '{incidentId}'." });
            }
            return Ok(result);
        }

        /// <summary>
        /// Retrieves the aggregate platform trust score summary and accuracy grade across all completed recoveries.
        /// </summary>
        [HttpGet("summary")]
        [ProducesResponseType(typeof(TrustScoreSummaryDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTrustScoreSummary()
        {
            var summary = await _trustScoreService.GetTrustScoreSummaryAsync();
            return Ok(summary);
        }
    }
}
