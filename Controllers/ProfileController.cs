using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpsRescue.Data;
using OpsRescue.DTOs;

namespace OpsRescue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized(new { message = "Invalid token claims." });
            }

            var user = await _context.Users
                .Include(u => u.OperationsManagerProfile)
                .Include(u => u.OperationsUserProfile)
                .Include(u => u.DeliveryPartnerProfile)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound(new { message = "User profile not found." });
            }

            object? roleProfile = user.Role switch
            {
                "Operations Manager" => user.OperationsManagerProfile is null ? null : new
                {
                    user.OperationsManagerProfile.EmployeeId,
                    user.OperationsManagerProfile.Department,
                    user.OperationsManagerProfile.BranchLocation,
                    user.OperationsManagerProfile.Designation
                },
                "Operations User" => user.OperationsUserProfile is null ? null : new
                {
                    user.OperationsUserProfile.EmployeeId,
                    user.OperationsUserProfile.Department,
                    user.OperationsUserProfile.BranchLocation,
                    user.OperationsUserProfile.Designation
                },
                "Delivery Partner" => user.DeliveryPartnerProfile is null ? null : new
                {
                    user.DeliveryPartnerProfile.PartnerId,
                    user.DeliveryPartnerProfile.CompanyName,
                    user.DeliveryPartnerProfile.VehicleNumber,
                    user.DeliveryPartnerProfile.OperatingLocation
                },
                "Admin" => null,
                _ => null
            };

            var response = new UserProfileResponseDto
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                RoleProfile = roleProfile
            };

            return Ok(response);
        }
    }
}
