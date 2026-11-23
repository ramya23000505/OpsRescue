using System.Security.Claims;
using OpsRescue.DTOs;
using OpsRescue.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OpsRescue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthInterface _authService;

        public AuthController(IAuthInterface authService)
        {
            _authService = authService;
        }

        [HttpGet("roles/fields")]
        public IActionResult GetRoleRegistrationFields()
        {
            var roleFields = new
            {
                commonFields = new[] { "firstName", "lastName", "username", "email", "phoneNumber", "password" },
                roles = new Dictionary<string, string[]>
                {
                    ["Admin"] = Array.Empty<string>(),
                    ["Operations Manager"] = new[] { "employeeId", "department", "branchLocation", "designation" },
                    ["Operations User"] = new[] { "employeeId", "department", "branchLocation", "designation" },
                    ["Delivery Partner"] = new[] { "partnerId", "companyName", "vehicleNumber", "operatingLocation" }
                }
            };

            return Ok(roleFields);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                var result = await _authService.RegisterAsync(dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var result = await _authService.LoginAsync(dto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUserProfile()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var profile = await _authService.GetUserWithProfileAsync(userId);
            return profile is null ? NotFound() : Ok(profile);
        }
    }
}