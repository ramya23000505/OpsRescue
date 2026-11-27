using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OpsRescue.Data;
using OpsRescue.DTOs;
using OpsRescue.Interface;
using OpsRescue.Models;

namespace OpsRescue.Services
{
    public class ImpactAnalysisService : IImpactAnalysisService
    {
        private readonly AppDbContext _context;

        public ImpactAnalysisService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ImpactAnalysisResponseDto?> GetImpactAnalysisByIncidentIdAsync(string incidentId)
        {
            if (string.IsNullOrWhiteSpace(incidentId))
                return null;

            var normalizedId = incidentId.Trim().ToUpperInvariant();

            var incident = await _context.Incidents
                .Include(i => i.BlastRadius)
                .Include(i => i.ExposureDossiers)
                .Include(i => i.GraphNodes)
                .Include(i => i.GraphEdges)
                .FirstOrDefaultAsync(i => i.IncidentId == normalizedId);

            if (incident == null)
                return null;

            int root = incident.BlastRadius?.RootIncidents ?? 1;
            int shipments = incident.BlastRadius?.Shipments ?? 0;
            int pos = incident.BlastRadius?.PurchaseOrders ?? 0;
            int consignees = incident.BlastRadius?.Consignees ?? incident.ExposureDossiers.Count;
            int totalBlastRadius = root + shipments + pos + consignees;

            decimal totalRevenueRisk = incident.CargoValue + incident.LiquidatedPenalty;
            int affectedAccountsCount = incident.ExposureDossiers.Count;

            var exposureList = incident.ExposureDossiers.Select(d => new EntityExposureDto
            {
                EntityId = d.EntityId,
                EntityName = d.EntityName,
                Tier = d.Tier,
                RiskLevel = d.RiskLevel,
                Location = d.Location,
                PoReference = d.PoReference,
                ValueAtRisk = d.ValueAtRisk,
                ValueAtRiskFormatted = FormatCurrency(d.ValueAtRisk),
                BufferRemainingMinutes = d.BufferRemainingMinutes,
                BufferRemainingFormatted = FormatBuffer(d.BufferRemainingMinutes),
                SlaPenaltyRate = d.SlaPenaltyRate,
                ClinicalOrBusinessImpact = d.ClinicalOrBusinessImpact,
                AssignedCarrier = d.AssignedCarrier,
                CargoDetails = d.CargoDetails
            }).ToList();

            var nodesList = incident.GraphNodes.Select(n => new ImpactGraphNodeDto
            {
                Id = n.NodeId,
                Label = n.Label,
                TierLevel = n.TierLevel,
                NodeType = n.NodeType,
                Status = n.Status,
                Subtitle = n.Subtitle,
                Attributes = string.IsNullOrWhiteSpace(n.AttributesJson)
                    ? new Dictionary<string, string>()
                    : JsonSerializer.Deserialize<Dictionary<string, string>>(n.AttributesJson) ?? new Dictionary<string, string>()
            }).ToList();

            var edgesList = incident.GraphEdges.Select(e => new ImpactGraphEdgeDto
            {
                From = e.FromNode,
                To = e.ToNode,
                Relationship = e.Relationship,
                Label = e.Label,
                Severity = e.Severity
            }).ToList();

            return new ImpactAnalysisResponseDto
            {
                IncidentId = incident.IncidentId,
                IncidentType = incident.IncidentType,
                RootCause = incident.RootCause,
                Severity = incident.Severity,
                PropagationModel = incident.PropagationModel,
                Status = incident.Status,

                TotalBlastRadius = totalBlastRadius,
                BlastRadiusBreakdown = new BlastRadiusBreakdownDto
                {
                    RootIncidents = root,
                    Shipments = shipments,
                    PurchaseOrders = pos,
                    Consignees = consignees
                },

                ContractualRevenueRisk = totalRevenueRisk,
                RevenueRiskFormatted = FormatCurrency(totalRevenueRisk),
                CargoValue = incident.CargoValue,
                LiquidatedPenalty = incident.LiquidatedPenalty,

                AffectedCustomerAccounts = affectedAccountsCount,
                CustomerImpactSummary = $"{affectedAccountsCount:00} Accounts Impacted",

                RemainingSlaBufferMinutes = incident.RemainingSlaBufferMinutes,
                RemainingSlaBufferFormatted = FormatBuffer(incident.RemainingSlaBufferMinutes),

                ExposureDossier = exposureList,
                GraphNodes = nodesList,
                GraphEdges = edgesList
            };
        }

        public async Task<List<ImpactSummaryDto>> GetActiveDisruptionsImpactSummaryAsync()
        {
            var incidents = await _context.Incidents
                .Include(i => i.BlastRadius)
                .Include(i => i.ExposureDossiers)
                .OrderBy(i => i.IncidentId)
                .ToListAsync();

            return incidents.Select(inc =>
            {
                int root = inc.BlastRadius?.RootIncidents ?? 1;
                int shipments = inc.BlastRadius?.Shipments ?? 0;
                int pos = inc.BlastRadius?.PurchaseOrders ?? 0;
                int consignees = inc.BlastRadius?.Consignees ?? inc.ExposureDossiers.Count;
                int totalBlast = root + shipments + pos + consignees;
                decimal totalRisk = inc.CargoValue + inc.LiquidatedPenalty;

                return new ImpactSummaryDto
                {
                    IncidentId = inc.IncidentId,
                    TotalBlastRadius = totalBlast,
                    RevenueRiskFormatted = FormatCurrency(totalRisk),
                    AffectedAccounts = inc.ExposureDossiers.Count,
                    RemainingBufferFormatted = FormatBuffer(inc.RemainingSlaBufferMinutes)
                };
            }).ToList();
        }

        public static string FormatCurrency(decimal amount)
        {
            if (amount >= 10000000m)
                return $"₹ {amount / 10000000m:0.##} Cr";
            if (amount >= 100000m)
                return $"₹ {amount / 100000m:0.##} Lakhs";
            return $"₹ {amount:N0}";
        }

        public static string FormatBuffer(int minutes)
        {
            if (minutes >= 60)
            {
                int hours = minutes / 60;
                int remainingMins = minutes % 60;
                return remainingMins > 0 ? $"{hours}h {remainingMins}m" : $"{hours}h";
            }
            return $"{minutes} mins";
        }
    }
}
