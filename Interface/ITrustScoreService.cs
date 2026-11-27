using OpsRescue.DTOs;

namespace OpsRescue.Interface
{
    public interface ITrustScoreService
    {
        Task<TrustScoreDetailDto?> GetTrustScoreByIncidentAsync(string incidentId);
        Task<TrustScoreSummaryDto> GetTrustScoreSummaryAsync();
    }
}
