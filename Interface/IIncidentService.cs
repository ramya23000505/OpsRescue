using OpsRescue.DTOs;

namespace OpsRescue.Interface
{
    public interface IIncidentService
    {
        Task<List<IncidentListDto>> GetIncidentsListAsync();
        Task<IncidentDetailDto?> GetIncidentDetailsAsync(string incidentId);
    }
}
