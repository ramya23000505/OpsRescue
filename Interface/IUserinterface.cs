using OpsRescue.Models;


namespace OpsRescue.Interface
{
    public interface IUserinterface
    {
        Task<bool> usernameexistsAsync(string username);

        Task<bool> emailexitsAsync(string email);

        Task<User?> GetbyEmailAsync(string email);

        Task AddAsync(User user);

        // New methods for 4-role profiles & login
        Task<User?> GetByEmailOrUsernameWithProfileAsync(string emailOrUsername);

        Task<User?> GetByIdWithProfileAsync(int userId);

        Task<bool> EmployeeIdExistsAsync(string employeeId, string role);

        Task<bool> PartnerIdExistsAsync(string partnerId);
    }
}