using Microsoft.EntityFrameworkCore;
using OpsRescue.Data;
using OpsRescue.DTOs;
using OpsRescue.Interface;

namespace OpsRescue.Services
{
    public class ControlTowerService : IControlTowerService
    {
        private readonly AppDbContext _context;

        public ControlTowerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ControlTowerDashboardDto> GetDashboardOverviewAsync()
        {
            var incidents = await _context.Incidents
                .AsNoTracking()
                .Include(i => i.BlastRadius)
                .Include(i => i.ExposureDossiers)
                .OrderBy(i => i.RemainingSlaBufferMinutes)
                .ToListAsync();

            var executions = await _context.RecoveryExecutions
                .AsNoTracking()
                .ToListAsync();

            var trustScores = await _context.AccountabilityTrustScores
                .AsNoTracking()
                .ToListAsync();

            var criticalCount = incidents.Count(i => i.Severity.Contains("CRITICAL", StringComparison.OrdinalIgnoreCase));
            var highCount = incidents.Count(i => i.Severity.Contains("HIGH", StringComparison.OrdinalIgnoreCase));
            var imminentSla = incidents.Count(i => i.RemainingSlaBufferMinutes <= 30);
            var totalRevenueAtRisk = incidents.Sum(i => i.LiquidatedPenalty);

            var pendingApprovals = incidents.Count(i => i.Status.Equals("Pending Approval", StringComparison.OrdinalIgnoreCase) || i.Status.Equals("Action Required", StringComparison.OrdinalIgnoreCase));
            var activeExecutions = executions.Count(e => e.Status.Equals("Dispatched", StringComparison.OrdinalIgnoreCase) || e.Status.Equals("Rendezvous", StringComparison.OrdinalIgnoreCase));

            decimal avgTrust = trustScores.Any() ? trustScores.Average(t => t.TrustScorePct) : 96.4m;

            var liveDisruptions = incidents.Select(i =>
            {
                var blast = i.BlastRadius;
                int blastCount = blast != null
                    ? (blast.RootIncidents + blast.Shipments + blast.PurchaseOrders + blast.Consignees)
                    : (1 + i.ExposureDossiers.Count);

                var primaryCarrier = i.ExposureDossiers.FirstOrDefault()?.AssignedCarrier ?? "TransExpress Logistics";

                return new ControlTowerIncidentItemDto
                {
                    IncidentId = i.IncidentId,
                    IncidentType = i.IncidentType,
                    Severity = i.Severity,
                    Status = i.Status,
                    RemainingSlaBufferMinutes = i.RemainingSlaBufferMinutes,
                    RemainingSlaBufferFormatted = $"{i.RemainingSlaBufferMinutes} mins",
                    LiquidatedPenaltyFormatted = FormatCurrency(i.LiquidatedPenalty),
                    TotalBlastRadius = blastCount,
                    AffectedConsignees = i.ExposureDossiers.Count,
                    AssignedCarrier = primaryCarrier,
                    CreatedAt = i.CreatedAt
                };
            }).ToList();

            var fleet = new List<FleetReadinessDto>
            {
                new FleetReadinessDto
                {
                    PartnerName = "TransExpress Logistics (Ravi Kumar)",
                    VehicleId = "TRK-882 (Reefer Unit)",
                    Location = "NH-544 / Salem Toll Plaza",
                    Status = "Dispatched (Active Mission)",
                    Specialization = "Cold-Chain Reefer (-20°C to +8°C)",
                    EtaToCorridor = "12 mins"
                },
                new FleetReadinessDto
                {
                    PartnerName = "Apex Recovery Fleet (Karthik S)",
                    VehicleId = "TOW-401 (Heavy Prime Mover)",
                    Location = "NH-48 KM 140 Depot",
                    Status = "Rendezvous On-Site",
                    Specialization = "Prime Mover Tow & Swap",
                    EtaToCorridor = "0 mins"
                },
                new FleetReadinessDto
                {
                    PartnerName = "RapidCargo Express (Manoj V)",
                    VehicleId = "VAN-104 (Dry-Ice Express)",
                    Location = "Talegaon Cross-Dock Hub",
                    Status = "Standby Ready",
                    Specialization = "Express Parcel & Cross-Dock",
                    EtaToCorridor = "18 mins"
                }
            };

            var slaDistribution = new SlaRiskDistributionDto
            {
                CriticalBreachRisk = incidents.Count(i => i.RemainingSlaBufferMinutes <= 35),
                ElevatedRisk = incidents.Count(i => i.RemainingSlaBufferMinutes > 35 && i.RemainingSlaBufferMinutes <= 90),
                ManagedRisk = incidents.Count(i => i.RemainingSlaBufferMinutes > 90)
            };

            return new ControlTowerDashboardDto
            {
                Kpis = new ControlTowerKpisDto
                {
                    TotalActiveDisruptions = incidents.Count,
                    CriticalP1Count = criticalCount,
                    HighP2Count = highCount,
                    ImminentSlaBreachesCount = imminentSla,
                    TotalRevenueAtRisk = totalRevenueAtRisk,
                    TotalRevenueAtRiskFormatted = FormatCurrency(totalRevenueAtRisk),
                    PendingApprovalsCount = pendingApprovals,
                    ActiveRecoveriesCount = activeExecutions,
                    AverageTrustScorePct = Math.Round(avgTrust, 1)
                },
                LiveDisruptions = liveDisruptions,
                FleetReadiness = fleet,
                SlaDistribution = slaDistribution,
                SystemStatus = new SystemStatusDto
                {
                    OperationalMode = "Active Governed Intelligence",
                    SignalRHubStatus = "Connected (Broadcasting)",
                    ActiveCausalEngine = "Online (v2.4)",
                    LastSyncedAt = DateTime.UtcNow
                }
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
