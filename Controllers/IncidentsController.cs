using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpsRescue.DTOs;
using OpsRescue.Interface;

namespace OpsRescue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class IncidentsController : ControllerBase
    {
        private readonly IIncidentService _incidentService;

        public IncidentsController(IIncidentService incidentService)
        {
            _incidentService = incidentService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<IncidentListDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetIncidents()
        {
            var incidents = await _incidentService.GetIncidentsListAsync();
            return Ok(incidents);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IncidentDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetIncidentById(string id)
        {
            var incident = await _incidentService.GetIncidentDetailsAsync(id);
            if (incident == null)
            {
                return NotFound(new { message = $"Incident '{id}' not found." });
            }
            return Ok(incident);
        }
    }
}
