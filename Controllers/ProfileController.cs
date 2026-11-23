using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpsRescue.Interface;

namespace OpsRescue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IAuthInterface _authService;

        public ProfileController(IAuthInterface authService)
        {
            _authService = authService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User.FindFirstValue(ClaimTypes.Name);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (!int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized(new { message = "Invalid token claims." });
            }

            var userProfile = await _authService.GetUserWithProfileAsync(userId);
            if (userProfile == null)
            {
                return NotFound(new { message = "User profile not found." });
            }

            return Ok(new
            {
                message = "JWT Authentication successful",
                userId = userId,
                username = username,
                email = email,
                role = role,
                profile = userProfile
            });
        }
    }
}
