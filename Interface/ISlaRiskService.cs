using OpsRescue.DTOs;

namespace OpsRescue.Interface
{
    public interface ISlaRiskService
    {
        Task<SlaRiskAnalysisResponseDto?> GetSlaRiskAnalysisByIncidentIdAsync(string incidentId);
    }
}
