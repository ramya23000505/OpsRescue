using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OpsRescue.Data;
using OpsRescue.DTOs;
using OpsRescue.Interface;

namespace OpsRescue.Services
{
    public class SlaRiskService : ISlaRiskService
    {
        private readonly AppDbContext _context;

        public SlaRiskService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SlaRiskAnalysisResponseDto?> GetSlaRiskAnalysisByIncidentIdAsync(string incidentId)
        {
            if (string.IsNullOrWhiteSpace(incidentId))
                return null;

            var normalizedId = incidentId.Trim().ToUpperInvariant();

            var assessment = await _context.SlaRiskAssessments
                .Include(a => a.TrendPoints)
                .Include(a => a.ContributingFactors)
                .Include(a => a.ShipmentContext)
                .FirstOrDefaultAsync(a => a.IncidentId == normalizedId);

            if (assessment == null)
                return null;

            var trendList = assessment.TrendPoints
                .OrderBy(t => t.SortOrder)
                .Select(t => new SlaTrendPointDto
                {
                    TimeLabel = t.TimeLabel,
                    NormalTransitProbability = (double)t.NormalTransitProbability,
                    BreachCurveProbability = (double)t.BreachCurveProbability,
                    IsCurrentPoint = t.IsCurrentPoint,
                    IsHardBreachThreshold = t.IsHardBreachThreshold
                }).ToList();

            var factorList = assessment.ContributingFactors
                .OrderBy(f => f.SortOrder)
                .Select(f => new RiskContributingFactorDto
                {
                    FactorName = f.FactorName,
                    SeverityBadge = f.SeverityBadge,
                    ContributionPercentage = f.ContributionPercentage,
                    Description = f.Description,
                    KeyMetrics = string.IsNullOrWhiteSpace(f.KeyMetricsJson)
                        ? new Dictionary<string, string>()
                        : JsonSerializer.Deserialize<Dictionary<string, string>>(f.KeyMetricsJson) ?? new Dictionary<string, string>()
                }).ToList();

            var contextDto = assessment.ShipmentContext != null ? new ShipmentContextDto
            {
                Consignee = assessment.ShipmentContext.Consignee,
                MasterSla = assessment.ShipmentContext.MasterSla,
                Criticality = assessment.ShipmentContext.Criticality,
                TemperatureStatus = assessment.ShipmentContext.TemperatureStatus,
                AssignedVehicle = assessment.ShipmentContext.AssignedVehicle
            } : new ShipmentContextDto();

            return new SlaRiskAnalysisResponseDto
            {
                IncidentId = assessment.IncidentId,
                ShipmentId = assessment.ShipmentId,
                Route = assessment.Route,
                Severity = assessment.Severity,

                BreachProbability = (double)assessment.BreachProbability,
                BreachProbabilityFormatted = $"{assessment.BreachProbability:0.#}%",
                RiskTier = assessment.RiskTier,
                RiskBaselineDelta = assessment.RiskBaselineDelta,

                TimeToSlaBreachMinutes = assessment.TimeToSlaBreachMinutes,
                TimeToSlaBreachFormatted = FormatBuffer(assessment.TimeToSlaBreachMinutes),
                HardBreachThresholdTime = assessment.HardBreachThresholdTime,

                ProjectedDelay = assessment.ProjectedDelay,
                CommittedSlaWindow = assessment.CommittedSlaWindow,

                FinancialExposure = assessment.FinancialExposure,
                FinancialExposureFormatted = FormatExposure(assessment.FinancialExposure),
                FinancialExposureType = assessment.FinancialExposureType,

                RemainingBufferBadge = assessment.RemainingBufferBadge,
                BreachProbabilityTrend = trendList,
                ContributingFactors = factorList,
                ShipmentContext = contextDto
            };
        }

        private static string FormatExposure(decimal amount)
        {
            if (amount >= 100000m)
                return $"₹ {amount / 100000m:0.##}L";
            return $"₹ {amount:N0}";
        }

        private static string FormatBuffer(int minutes)
        {
            if (minutes >= 60)
            {
                int hours = minutes / 60;
                int remainingMins = minutes % 60;
                return remainingMins > 0 ? $"{hours}h {remainingMins}m" : $"{hours}h";
            }
            return $"{minutes} min";
        }
    }
}
