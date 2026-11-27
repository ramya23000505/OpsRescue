using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpsRescue.DTOs;
using OpsRescue.Interface;
using System.Security.Claims;

namespace OpsRescue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExecutionController : ControllerBase
    {
        private readonly IExecutionService _executionService;

        public ExecutionController(IExecutionService executionService)
        {
            _executionService = executionService;
        }

        /// <summary>
        /// Dispatches and executes an approved recovery strategy for an incident.
        /// Broadcasts real-time updates via SignalR IncidentHub.
        /// </summary>
        [HttpPost("dispatch")]
        [ProducesResponseType(typeof(DispatchExecutionResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DispatchExecution([FromBody] DispatchExecutionRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var approvedBy = User.FindFirstValue(ClaimTypes.Name) 
                                 ?? User.FindFirstValue("unique_name") 
                                 ?? User.Identity?.Name 
                                 ?? "Operations Manager";

                var response = await _executionService.DispatchExecutionAsync(dto, approvedBy);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Gets the latest recovery execution status for an incident.
        /// </summary>
        [HttpGet("{incidentId}")]
        [ProducesResponseType(typeof(RecoveryExecutionSummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetExecutionStatus(string incidentId)
        {
            var status = await _executionService.GetExecutionStatusAsync(incidentId);
            if (status == null)
            {
                return NotFound(new { message = $"No active execution found for incident '{incidentId}'." });
            }
            return Ok(status);
        }
    }
}
