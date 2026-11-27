using Microsoft.EntityFrameworkCore;
using OpsRescue.Data;
using OpsRescue.DTOs;
using OpsRescue.Interface;

namespace OpsRescue.Services
{
    public class RecoverySimulationService : IRecoverySimulationService
    {
        private readonly AppDbContext _context;

        public RecoverySimulationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RecoverySimulationResponseDto?> GetSimulationStrategiesByIncidentIdAsync(string incidentId)
        {
            if (string.IsNullOrWhiteSpace(incidentId))
                return null;

            var normalizedId = incidentId.Trim().ToUpperInvariant();

            var simulation = await _context.IncidentSimulations
                .Include(s => s.Strategies)
                .FirstOrDefaultAsync(s => s.IncidentId == normalizedId);

            if (simulation == null)
                return null;

            var strategyList = simulation.Strategies
                .OrderBy(s => s.StrategyNumber)
                .Select(s => new RecoveryStrategyDto
                {
                    StrategyId = s.StrategyId,
                    StrategyNumber = s.StrategyNumber,
                    Title = s.Title,
                    CategoryTag = s.CategoryTag,
                    Description = s.Description,
                    IsRecommended = s.IsRecommended,
                    RecommendationBadge = s.RecommendationBadge,
                    PredictedDeliveryTime = s.PredictedDeliveryTime,
                    DeliveryMargin = s.DeliveryMargin,
                    EstimatedOutlay = s.EstimatedOutlay,
                    EstimatedOutlayFormatted = $"₹ {s.EstimatedOutlay:N0}",
                    OutlayBreakdown = s.OutlayBreakdown,
                    ExpectedSlaOutcome = s.ExpectedSlaOutcome,
                    SlaSuccessProbability = (double)s.SlaSuccessProbability,
                    SlaSuccessProbabilityFormatted = $"{s.SlaSuccessProbability:0.#}%",
                    LiquidatedDamagesImpact = s.LiquidatedDamagesImpact,
                    LiquidatedDamagesImpactFormatted = s.IsDamagesAvoided 
                        ? $"₹ {s.LiquidatedDamagesImpact:N0} Avoided" 
                        : $"₹ {s.LiquidatedDamagesImpact:N0} Penalty",
                    IsDamagesAvoided = s.IsDamagesAvoided,
                    OperationalProtocol = s.OperationalProtocol,
                    IdentifiedConstraints = s.IdentifiedConstraints,
                    OperationalGovernance = s.OperationalGovernance,
                    RequiredApprovalTier = s.RequiredApprovalTier,
                    GovernanceAlert = s.GovernanceAlert
                }).ToList();

            var recommended = strategyList.FirstOrDefault(s => s.IsRecommended) ?? strategyList.FirstOrDefault();
            string headline = recommended != null
                ? $"System Optimization Engine · Evaluated {strategyList.Count} recovery paths for {incidentId} (Recommended: Strategy {recommended.StrategyNumber} - {recommended.Title})"
                : $"System Optimization Engine · Evaluated {strategyList.Count} recovery paths for {incidentId}";

            decimal netBenefit = 0;
            if (recommended != null)
            {
                netBenefit = recommended.IsDamagesAvoided
                    ? recommended.LiquidatedDamagesImpact - recommended.EstimatedOutlay
                    : -recommended.EstimatedOutlay;
            }

            return new RecoverySimulationResponseDto
            {
                IncidentId = simulation.IncidentId,
                ShipmentId = simulation.ShipmentId,
                OptimizationEngineHeadline = headline,
                RecommendedStrategyId = simulation.RecommendedStrategyId,
                RecommendationSummary = simulation.RecommendationSummary,
                Strategies = strategyList,
                TradeoffSummary = new StrategyTradeoffSummaryDto
                {
                    KeyTradeoffFinding = simulation.KeyTradeoffFinding,
                    NetBenefitFormatted = $"₹ {netBenefit:N0} Net Benefit",
                    WhyRecommended = simulation.WhyRecommended,
                    WhyRejectedAlternatives = simulation.WhyRejectedAlternatives
                }
            };
        }
    }
}
