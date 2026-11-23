using OpsRescue.DTOs;
using OpsRescue.Interface;

namespace OpsRescue.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _repository;

        public DashboardService(IDashboardRepository repository)
        {
            _repository = repository;
        }

        public async Task<AdminDashboardDto> GetAdminDashboardAsync()
        {
            var recentUsers = await _repository.GetRecentUsersAsync(5);

            return new AdminDashboardDto
            {
                TotalUsersCount = await _repository.GetTotalUsersCountAsync(),
                AdminCount = await _repository.GetUserCountByRoleAsync("Admin"),
                OperationsManagerCount = await _repository.GetUserCountByRoleAsync("Operations Manager"),
                OperationsUserCount = await _repository.GetUserCountByRoleAsync("Operations User"),
                DeliveryPartnerCount = await _repository.GetUserCountByRoleAsync("Delivery Partner"),
                RecentUsers = recentUsers.Select(u => new RecentUserDto
                {
                    Id = u.Id,
                    FullName = $"{u.FirstName} {u.LastName}".Trim(),
                    Username = u.Username,
                    Email = u.Email,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt
                }).ToList()
            };
        }

        public async Task<OperationsManagerDashboardDto> GetOperationsManagerDashboardAsync(int userId)
        {
            var user = await _repository.GetUserWithProfileByIdAsync(userId);
            if (user == null || user.OperationsManagerProfile == null)
            {
                throw new KeyNotFoundException("Operations Manager profile not found.");
            }

            var profile = user.OperationsManagerProfile;
            var branchUsers = await _repository.GetOperationsUsersByBranchAsync(profile.BranchLocation);
            var deliveryPartnersCount = await _repository.GetDeliveryPartnersCountByLocationAsync(profile.BranchLocation);

            return new OperationsManagerDashboardDto
            {
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Email = user.Email,
                EmployeeId = profile.EmployeeId,
                Department = profile.Department,
                BranchLocation = profile.BranchLocation,
                Designation = profile.Designation,
                TotalOperationsUsersInBranch = branchUsers.Count,
                TotalDeliveryPartnersInLocation = deliveryPartnersCount,
                BranchTeamMembers = branchUsers.Select(bu => new TeamMemberSummaryDto
                {
                    UserId = bu.UserId,
                    FullName = bu.User != null ? $"{bu.User.FirstName} {bu.User.LastName}".Trim() : string.Empty,
                    EmployeeId = bu.EmployeeId,
                    Designation = bu.Designation
                }).ToList()
            };
        }

        public async Task<OperationsUserDashboardDto> GetOperationsUserDashboardAsync(int userId)
        {
            var user = await _repository.GetUserWithProfileByIdAsync(userId);
            if (user == null || user.OperationsUserProfile == null)
            {
                throw new KeyNotFoundException("Operations User profile not found.");
            }

            var profile = user.OperationsUserProfile;
            var branchUsersCount = await _repository.GetOperationsUsersCountByBranchAsync(profile.BranchLocation);
            var branchManagers = await _repository.GetOperationsManagersByBranchAsync(profile.BranchLocation);

            return new OperationsUserDashboardDto
            {
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Email = user.Email,
                EmployeeId = profile.EmployeeId,
                Department = profile.Department,
                BranchLocation = profile.BranchLocation,
                Designation = profile.Designation,
                // Count colleagues excluding current user
                BranchColleaguesCount = Math.Max(0, branchUsersCount - 1),
                BranchManagers = branchManagers
                    .Where(m => m.User != null)
                    .Select(m => $"{m.User!.FirstName} {m.User.LastName}".Trim())
                    .ToList()
            };
        }

        public async Task<DeliveryPartnerDashboardDto> GetDeliveryPartnerDashboardAsync(int userId)
        {
            var user = await _repository.GetUserWithProfileByIdAsync(userId);
            if (user == null || user.DeliveryPartnerProfile == null)
            {
                throw new KeyNotFoundException("Delivery Partner profile not found.");
            }

            var profile = user.DeliveryPartnerProfile;
            var peersCount = await _repository.GetDeliveryPartnersCountByLocationAsync(profile.OperatingLocation);

            return new DeliveryPartnerDashboardDto
            {
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                PartnerId = profile.PartnerId,
                CompanyName = profile.CompanyName,
                VehicleNumber = profile.VehicleNumber,
                OperatingLocation = profile.OperatingLocation,
                Status = "Active",
                PeersInOperatingLocation = Math.Max(0, peersCount - 1)
            };
        }
    }
}
