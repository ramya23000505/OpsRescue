using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpsRescue.Interface;

namespace OpsRescue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            var summary = await _dashboardService.GetAdminDashboardAsync();
            return Ok(summary);
        }

        [HttpGet("operations-manager")]
        [Authorize(Roles = "Operations Manager")]
        public async Task<IActionResult> GetOperationsManagerDashboard()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { message = "Invalid user identification in token." });

            try
            {
                var summary = await _dashboardService.GetOperationsManagerDashboardAsync(userId);
                return Ok(summary);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("operations-user")]
        [Authorize(Roles = "Operations User")]
        public async Task<IActionResult> GetOperationsUserDashboard()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { message = "Invalid user identification in token." });

            try
            {
                var summary = await _dashboardService.GetOperationsUserDashboardAsync(userId);
                return Ok(summary);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("delivery-partner")]
        [Authorize(Roles = "Delivery Partner")]
        public async Task<IActionResult> GetDeliveryPartnerDashboard()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { message = "Invalid user identification in token." });

            try
            {
                var summary = await _dashboardService.GetDeliveryPartnerDashboardAsync(userId);
                return Ok(summary);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
