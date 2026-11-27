using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpsRescue.DTOs;
using OpsRescue.Interface;

namespace OpsRescue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ControlTowerController : ControllerBase
    {
        private readonly IControlTowerService _controlTowerService;

        public ControlTowerController(IControlTowerService controlTowerService)
        {
            _controlTowerService = controlTowerService;
        }

        /// <summary>
        /// Retrieves the real-time operational dashboard for the Control Tower.
        /// Powers Lavanya's Module 8 Control Tower screen with live KPIs, active disruptions, and fleet readiness.
        /// </summary>
        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(ControlTowerDashboardDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _controlTowerService.GetDashboardOverviewAsync();
            return Ok(result);
        }
    }
}
