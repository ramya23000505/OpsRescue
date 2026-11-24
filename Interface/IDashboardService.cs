using OpsRescue.DTOs;

namespace OpsRescue.Interface
{
    public interface IDashboardService
    {
        Task<AdminDashboardDto> GetAdminDashboardAsync();
        Task<OperationsManagerDashboardDto> GetOperationsManagerDashboardAsync(int userId);
        Task<OperationsUserDashboardDto> GetOperationsUserDashboardAsync(int userId);
        Task<DeliveryPartnerDashboardDto> GetDeliveryPartnerDashboardAsync(int userId);
    }
}
