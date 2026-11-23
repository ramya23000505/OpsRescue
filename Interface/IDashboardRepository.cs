using OpsRescue.Models;

namespace OpsRescue.Interface
{
    public interface IDashboardRepository
    {
        Task<int> GetTotalUsersCountAsync();
        Task<int> GetUserCountByRoleAsync(string role);
        Task<List<User>> GetRecentUsersAsync(int count);
        Task<User?> GetUserWithProfileByIdAsync(int userId);
        Task<int> GetOperationsUsersCountByBranchAsync(string branchLocation);
        Task<int> GetDeliveryPartnersCountByLocationAsync(string location);
        Task<List<OperationsUserProfile>> GetOperationsUsersByBranchAsync(string branchLocation);
        Task<List<OperationsManagerProfile>> GetOperationsManagersByBranchAsync(string branchLocation);
    }
}
