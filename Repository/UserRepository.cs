using OpsRescue.Data;
using OpsRescue.Interface;
using OpsRescue.Models;
using Microsoft.EntityFrameworkCore;

namespace OpsRescue.Repository
{
    public class UserRepository : IUserinterface
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> usernameexistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> emailexitsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User?> GetbyEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.OperationsManagerProfile)
                .Include(u => u.OperationsUserProfile)
                .Include(u => u.DeliveryPartnerProfile)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetByEmailOrUsernameWithProfileAsync(string emailOrUsername)
        {
            return await _context.Users
                .Include(u => u.OperationsManagerProfile)
                .Include(u => u.OperationsUserProfile)
                .Include(u => u.DeliveryPartnerProfile)
                .FirstOrDefaultAsync(u =>
                    u.Email == emailOrUsername || u.Username == emailOrUsername);
        }

        public async Task<User?> GetByIdWithProfileAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.OperationsManagerProfile)
                .Include(u => u.OperationsUserProfile)
                .Include(u => u.DeliveryPartnerProfile)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<bool> EmployeeIdExistsAsync(string employeeId, string role)
        {
            return role == "Operations Manager"
                ? await _context.OperationsManagerProfiles.AnyAsync(p => p.EmployeeId == employeeId)
                : await _context.OperationsUserProfiles.AnyAsync(p => p.EmployeeId == employeeId);
        }

        public async Task<bool> PartnerIdExistsAsync(string partnerId)
        {
            return await _context.DeliveryPartnerProfiles.AnyAsync(p => p.PartnerId == partnerId);
        }
    }
}