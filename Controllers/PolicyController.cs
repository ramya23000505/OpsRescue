using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpsRescue.DTOs;
using OpsRescue.Interface;

namespace OpsRescue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PolicyController : ControllerBase
    {
        private readonly IPolicyService _policyService;

        public PolicyController(IPolicyService policyService)
        {
            _policyService = policyService;
        }

        /// <summary>
        /// Evaluates business rules and approval tier requirements for a specific recovery strategy.
        /// Module 5 Policy & Approval Engine.
        /// </summary>
        [HttpGet("evaluate/{incidentId}/{strategyId}")]
        [ProducesResponseType(typeof(PolicyEvaluationResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EvaluatePolicy(string incidentId, string strategyId)
        {
            try
            {
                var result = await _policyService.EvaluatePolicyAsync(incidentId, strategyId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Returns the operational policy approval matrix (Tiers, Cost Thresholds, Roles).
        /// </summary>
        [HttpGet("matrix")]
        [ProducesResponseType(typeof(List<PolicyApprovalMatrixDto>), StatusCodes.Status200OK)]
        public IActionResult GetApprovalMatrix()
        {
            var matrix = _policyService.GetApprovalMatrix();
            return Ok(matrix);
        }
    }
}
