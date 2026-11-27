using OpsRescue.DTOs;

namespace OpsRescue.Interface
{
    public interface IImpactAnalysisService
    {
        Task<ImpactAnalysisResponseDto?> GetImpactAnalysisByIncidentIdAsync(string incidentId);
        Task<List<ImpactSummaryDto>> GetActiveDisruptionsImpactSummaryAsync();
    }
}
