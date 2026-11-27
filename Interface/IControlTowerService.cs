using OpsRescue.DTOs;

namespace OpsRescue.Interface
{
    public interface IControlTowerService
    {
        Task<ControlTowerDashboardDto> GetDashboardOverviewAsync();
    }
}
