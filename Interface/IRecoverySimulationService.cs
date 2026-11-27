using OpsRescue.DTOs;

namespace OpsRescue.Interface
{
    public interface IRecoverySimulationService
    {
        Task<RecoverySimulationResponseDto?> GetSimulationStrategiesByIncidentIdAsync(string incidentId);
    }
}
