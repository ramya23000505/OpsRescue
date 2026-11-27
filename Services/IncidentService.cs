using Microsoft.EntityFrameworkCore;
using OpsRescue.Data;
using OpsRescue.DTOs;
using OpsRescue.Interface;
using System.Globalization;

namespace OpsRescue.Services
{
    public class IncidentService : IIncidentService
    {
        private readonly AppDbContext _context;

        public IncidentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<IncidentListDto>> GetIncidentsListAsync()
        {
            var incidents = await _context.Incidents
                .AsNoTracking()
                .Include(i => i.BlastRadius)
                .Include(i => i.ExposureDossiers)
                .OrderBy(i => i.RemainingSlaBufferMinutes)
                .ToListAsync();

            return incidents.Select(i =>
            {
                var blast = i.BlastRadius;
                int totalBlast = blast != null
                    ? (blast.RootIncidents + blast.Shipments + blast.PurchaseOrders + blast.Consignees)
                    : (1 + i.ExposureDossiers.Count);

                return new IncidentListDto
                {
                    IncidentId = i.IncidentId,
                    IncidentType = i.IncidentType,
                    Severity = i.Severity,
                    Status = i.Status,
                    RootCause = i.RootCause,
                    CargoValue = i.CargoValue,
                    CargoValueFormatted = FormatCurrency(i.CargoValue),
                    LiquidatedPenalty = i.LiquidatedPenalty,
                    LiquidatedPenaltyFormatted = FormatCurrency(i.LiquidatedPenalty),
                    RemainingSlaBufferMinutes = i.RemainingSlaBufferMinutes,
                    RemainingSlaBufferFormatted = $"{i.RemainingSlaBufferMinutes} mins",
                    TotalBlastRadius = totalBlast,
                    AffectedAccounts = i.ExposureDossiers.Count,
                    CreatedAt = i.CreatedAt
                };
            }).ToList();
        }

        public async Task<IncidentDetailDto?> GetIncidentDetailsAsync(string incidentId)
        {
            var normalizedId = incidentId.Trim().ToUpperInvariant();

            var incident = await _context.Incidents
                .AsNoTracking()
                .Include(i => i.BlastRadius)
                .Include(i => i.ExposureDossiers)
                .Include(i => i.TimelineEvents)
                .Include(i => i.RecoveryExecutions)
                .Include(i => i.ApprovalLogs)
                .Include(i => i.TrustScores)
                .FirstOrDefaultAsync(i => i.IncidentId == normalizedId);

            if (incident == null)
            {
                return null;
            }

            var blast = incident.BlastRadius ?? new Models.ImpactBlastRadius
            {
                IncidentId = incident.IncidentId,
                RootIncidents = 1,
                Shipments = 1,
                PurchaseOrders = 1,
                Consignees = incident.ExposureDossiers.Count
            };

            int totalBlast = blast.RootIncidents + blast.Shipments + blast.PurchaseOrders + blast.Consignees;

            var latestExec = incident.RecoveryExecutions.OrderByDescending(e => e.DispatchedAt).FirstOrDefault();
            var latestApproval = incident.ApprovalLogs.OrderByDescending(a => a.ApprovedAt).FirstOrDefault();
            var trustScore = incident.TrustScores.OrderByDescending(t => t.EvaluatedAt).FirstOrDefault();

            return new IncidentDetailDto
            {
                IncidentId = incident.IncidentId,
                IncidentType = incident.IncidentType,
                Severity = incident.Severity,
                Status = incident.Status,
                RootCause = incident.RootCause,
                CargoValue = incident.CargoValue,
                CargoValueFormatted = FormatCurrency(incident.CargoValue),
                LiquidatedPenalty = incident.LiquidatedPenalty,
                LiquidatedPenaltyFormatted = FormatCurrency(incident.LiquidatedPenalty),
                RemainingSlaBufferMinutes = incident.RemainingSlaBufferMinutes,
                RemainingSlaBufferFormatted = $"{incident.RemainingSlaBufferMinutes} mins",
                TotalBlastRadius = totalBlast,
                AffectedAccounts = incident.ExposureDossiers.Count,
                CreatedAt = incident.CreatedAt,
                PropagationModel = incident.PropagationModel,
                BlastRadiusBreakdown = new BlastRadiusBreakdownDto
                {
                    RootIncidents = blast.RootIncidents,
                    Shipments = blast.Shipments,
                    PurchaseOrders = blast.PurchaseOrders,
                    Consignees = blast.Consignees
                },
                Timeline = incident.TimelineEvents
                    .OrderBy(t => t.EventTime)
                    .Select(t => new IncidentTimelineEventDto
                    {
                        Id = t.Id,
                        EventTime = t.EventTime,
                        EventTimeFormatted = t.EventTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                        Stage = t.Stage,
                        Title = t.Title,
                        Description = t.Description,
                        SeverityBadge = t.SeverityBadge,
                        Source = t.Source
                    }).ToList(),
                LatestExecution = latestExec != null ? new RecoveryExecutionSummaryDto
                {
                    ExecutionId = latestExec.ExecutionId,
                    StrategyId = latestExec.StrategyId,
                    StrategyName = latestExec.StrategyName,
                    Status = latestExec.Status,
                    DispatchedAtFormatted = latestExec.DispatchedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    EstimatedRendezvousFormatted = latestExec.EstimatedRendezvousAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "TBD",
                    AssignedPartner = latestExec.AssignedPartner,
                    VehicleId = latestExec.VehicleId,
                    CurrentLocation = latestExec.CurrentLocation
                } : null,
                LatestApproval = latestApproval != null ? new ManagerApprovalSummaryDto
                {
                    StrategyId = latestApproval.StrategyId,
                    StrategyName = latestApproval.StrategyName,
                    ApprovedBy = latestApproval.ApprovedBy,
                    ApprovedAtFormatted = latestApproval.ApprovedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    ApprovalTier = latestApproval.ApprovalTier,
                    Notes = latestApproval.Notes
                } : null,
                TrustScore = trustScore != null ? new AccountabilityTrustScoreSummaryDto
                {
                    RecoveryStrategyId = trustScore.RecoveryStrategyId,
                    PredictedRecoveryTimeMins = trustScore.PredictedRecoveryTimeMins,
                    ActualRecoveryTimeMins = trustScore.ActualRecoveryTimeMins,
                    TrustScorePct = trustScore.TrustScorePct,
                    PredictionAccuracyGrade = trustScore.PredictionAccuracyGrade,
                    VarianceReason = trustScore.VarianceReason
                } : null
            };
        }

        private static string FormatCurrency(decimal amount)
        {
            if (amount >= 10000000m)
            {
                var cr = amount / 10000000m;
                return $"₹ {cr:0.##} Cr";
            }
            if (amount >= 100000m)
            {
                var lakhs = amount / 100000m;
                return $"₹ {lakhs:0.##} Lakhs";
            }
            return $"₹ {amount:N0}";
        }
    }
}
