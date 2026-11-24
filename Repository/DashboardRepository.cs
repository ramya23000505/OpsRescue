using Microsoft.EntityFrameworkCore;
using OpsRescue.Data;
using OpsRescue.Interface;
using OpsRescue.Models;

namespace OpsRescue.Repository
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly AppDbContext _context;

        public DashboardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalUsersCountAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<int> GetUserCountByRoleAsync(string role)
        {
            return await _context.Users.CountAsync(u => u.Role == role);
        }

        public async Task<List<User>> GetRecentUsersAsync(int count)
        {
            return await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<User?> GetUserWithProfileByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.OperationsManagerProfile)
                .Include(u => u.OperationsUserProfile)
                .Include(u => u.DeliveryPartnerProfile)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<int> GetOperationsUsersCountByBranchAsync(string branchLocation)
        {
            return await _context.OperationsUserProfiles
                .CountAsync(p => p.BranchLocation == branchLocation);
        }

        public async Task<int> GetDeliveryPartnersCountByLocationAsync(string location)
        {
            return await _context.DeliveryPartnerProfiles
                .CountAsync(p => p.OperatingLocation == location);
        }

        public async Task<List<OperationsUserProfile>> GetOperationsUsersByBranchAsync(string branchLocation)
        {
            return await _context.OperationsUserProfiles
                .Include(p => p.User)
                .Where(p => p.BranchLocation == branchLocation)
                .ToListAsync();
        }

        public async Task<List<OperationsManagerProfile>> GetOperationsManagersByBranchAsync(string branchLocation)
        {
            return await _context.OperationsManagerProfiles
                .Include(p => p.User)
                .Where(p => p.BranchLocation == branchLocation)
                .ToListAsync();
        }
    }
}
