using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using OpsRescue.Data;
using OpsRescue.DTOs;
using OpsRescue.Interface;
using OpsRescue.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace OpsRescue.Services
{
    public class AuthService : IAuthInterface
    {
        private readonly IUserinterface _userRepository;
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJwtService _jwtService;

        private static readonly HashSet<string> AllowedRoles = new(StringComparer.Ordinal)
        {
            "Admin",
            "Operations Manager",
            "Operations User",
            "Delivery Partner"
        };

        public AuthService(
            IUserinterface userRepository,
            AppDbContext context,
            IPasswordHasher<User> passwordHasher,
            IJwtService jwtService)
        {
            _userRepository = userRepository;
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            if (!AllowedRoles.Contains(dto.Role))
                throw new ArgumentException("Invalid role specified.");

            if (await _userRepository.usernameexistsAsync(dto.Username))
                throw new InvalidOperationException("Username already exists.");

            if (await _userRepository.emailexitsAsync(dto.Email))
                throw new InvalidOperationException("Email already exists.");

            await ValidateRoleSpecificFieldsAsync(dto);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = new User
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Username = dto.Username,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                    Role = dto.Role,
                    CreatedAt = DateTime.UtcNow
                };

                user.Password = _passwordHasher.HashPassword(user, dto.Password);

                switch (dto.Role)
                {
                    case "Operations Manager":
                        user.OperationsManagerProfile = new OperationsManagerProfile
                        {
                            EmployeeId = dto.EmployeeId!,
                            Department = dto.Department!,
                            BranchLocation = dto.BranchLocation!,
                            Designation = dto.Designation!
                        };
                        break;

                    case "Operations User":
                        user.OperationsUserProfile = new OperationsUserProfile
                        {
                            EmployeeId = dto.EmployeeId!,
                            Department = dto.Department!,
                            BranchLocation = dto.BranchLocation!,
                            Designation = dto.Designation!
                        };
                        break;

                    case "Delivery Partner":
                        user.DeliveryPartnerProfile = new DeliveryPartnerProfile
                        {
                            PartnerId = dto.PartnerId!,
                            CompanyName = dto.CompanyName!,
                            VehicleNumber = dto.VehicleNumber!,
                            OperatingLocation = dto.OperatingLocation!
                        };
                        break;

                    case "Admin":
                        break;
                }

                await _userRepository.AddAsync(user);
                await transaction.CommitAsync();

                return BuildAuthResponse(user);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailOrUsernameWithProfileAsync(dto.EmailOrUsername);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials.");

            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.Password, dto.Password);
            if (verifyResult == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("Invalid credentials.");

            if (!AllowedRoles.Contains(user.Role))
                throw new UnauthorizedAccessException("User role is not authorized for system access.");

            return BuildAuthResponse(user);
        }

        public async Task<AuthResponseDto?> GetUserWithProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdWithProfileAsync(userId);
            return user == null ? null : BuildAuthResponse(user, includeToken: false);
        }

        private async Task ValidateRoleSpecificFieldsAsync(RegisterDto dto)
        {
            if (dto.Role is "Operations Manager" or "Operations User")
            {
                if (string.IsNullOrWhiteSpace(dto.EmployeeId) ||
                    string.IsNullOrWhiteSpace(dto.Department) ||
                    string.IsNullOrWhiteSpace(dto.BranchLocation) ||
                    string.IsNullOrWhiteSpace(dto.Designation))
                {
                    throw new ArgumentException($"EmployeeId, Department, BranchLocation, and Designation are required for {dto.Role}.");
                }

                if (await _userRepository.EmployeeIdExistsAsync(dto.EmployeeId, dto.Role))
                    throw new InvalidOperationException($"Employee ID '{dto.EmployeeId}' is already registered.");
            }
            else if (dto.Role == "Delivery Partner")
            {
                if (string.IsNullOrWhiteSpace(dto.PartnerId) ||
                    string.IsNullOrWhiteSpace(dto.CompanyName) ||
                    string.IsNullOrWhiteSpace(dto.VehicleNumber) ||
                    string.IsNullOrWhiteSpace(dto.OperatingLocation))
                {
                    throw new ArgumentException("PartnerId, CompanyName, VehicleNumber, and OperatingLocation are required for Delivery Partner.");
                }

                if (await _userRepository.PartnerIdExistsAsync(dto.PartnerId))
                    throw new InvalidOperationException($"Partner ID '{dto.PartnerId}' is already registered.");
            }
        }

        private AuthResponseDto BuildAuthResponse(User user, bool includeToken = true)
        {
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

            return new AuthResponseDto
            {
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Token = includeToken ? _jwtService.GenerateToken(user.Id, user.Username, user.Email, user.Role) : string.Empty,
                RoleProfile = roleProfile
            };
        }
    }
}   