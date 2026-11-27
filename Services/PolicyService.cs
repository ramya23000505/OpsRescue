using Microsoft.EntityFrameworkCore;
using OpsRescue.Data;
using OpsRescue.DTOs;
using OpsRescue.Interface;

namespace OpsRescue.Services
{
    public class PolicyService : IPolicyService
    {
        private readonly AppDbContext _context;

        public PolicyService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PolicyEvaluationResultDto> EvaluatePolicyAsync(string incidentId, string strategyId)
        {
            var normalizedIncidentId = incidentId.Trim().ToUpperInvariant();
            var normalizedStrategyId = strategyId.Trim().ToUpperInvariant();

            var incident = await _context.Incidents
                .AsNoTracking()
                .Include(i => i.ExposureDossiers)
                .FirstOrDefaultAsync(i => i.IncidentId == normalizedIncidentId);

            if (incident == null)
            {
                throw new KeyNotFoundException($"Incident '{incidentId}' not found.");
            }

            var strategy = await _context.SimulationStrategies
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.IncidentId == normalizedIncidentId && s.StrategyId == normalizedStrategyId);

            if (strategy == null)
            {
                throw new KeyNotFoundException($"Strategy '{strategyId}' not found for incident '{incidentId}'.");
            }

            // Rule 1: Financial Intervention Threshold (< ₹ 50,000)
            bool passesCostThreshold = strategy.EstimatedOutlay <= 50000m;
            var costRule = new PolicyRuleEvaluationItemDto
            {
                RuleCode = "POL-FIN-01",
                RuleName = "Intervention Cost Delegation Limit",
                Condition = "Estimated Intervention Cost <= ₹ 50,000 for autonomous dispatch",
                Passed = passesCostThreshold,
                ImpactOnApproval = passesCostThreshold 
                    ? "Passed: Within autonomous delegation threshold" 
                    : "Flagged: Cost exceeds autonomous limit; requires Operations Manager signoff"
            };

            // Rule 2: High Consequence Consignee / Cargo Criticality
            bool hasCriticalHospitalConsignee = incident.ExposureDossiers.Any(d => d.EntityName.Contains("Hospital", StringComparison.OrdinalIgnoreCase));
            bool passesConsigneeRule = !hasCriticalHospitalConsignee || strategy.ExpectedSlaOutcome.Contains("Preserved", StringComparison.OrdinalIgnoreCase) || !strategy.DeliveryMargin.Contains("-");
            var consigneeRule = new PolicyRuleEvaluationItemDto
            {
                RuleCode = "POL-SLA-02",
                RuleName = "Life-Science & High-Value Consignee Protection",
                Condition = "Pharma/Hospital shipments must maintain positive delivery margin",
                Passed = passesConsigneeRule,
                ImpactOnApproval = passesConsigneeRule 
                    ? "Passed: Delivery margin protects critical consignee" 
                    : "Warning: Low buffer for clinical consignee; requires Manager review"
            };

            // Rule 3: Strategy Classification
            bool isStrategyRecommended = strategy.IsRecommended;
            var recommendationRule = new PolicyRuleEvaluationItemDto
            {
                RuleCode = "POL-ENG-03",
                RuleName = "Engine Optimality Verification",
                Condition = "Selected strategy must be ranked optimal by Recovery Simulator Engine",
                Passed = isStrategyRecommended,
                ImpactOnApproval = isStrategyRecommended
                    ? "Passed: Engine verified as optimal recovery option"
                    : "Caution: Non-optimal strategy selected; mandatory justification required"
            };

            bool isAutoApproved = passesCostThreshold && isStrategyRecommended && passesConsigneeRule;
            string requiredTier = isAutoApproved ? "Tier 1 Auto-Approved" : strategy.RequiredApprovalTier;
            string verdict = isAutoApproved ? "Eligible for Autonomous Dispatch" : $"Escalated to {requiredTier}";

            return new PolicyEvaluationResultDto
            {
                IncidentId = incident.IncidentId,
                StrategyId = strategy.StrategyId,
                StrategyTitle = strategy.Title,
                IsEligibleForAutoApproval = isAutoApproved,
                RequiredApprovalTier = requiredTier,
                GovernanceVerdict = verdict,
                EstimatedInterventionCost = strategy.EstimatedOutlay,
                EstimatedInterventionCostFormatted = FormatCurrency(strategy.EstimatedOutlay),
                ProtectedRevenueValue = incident.LiquidatedPenalty,
                ProtectedRevenueValueFormatted = FormatCurrency(incident.LiquidatedPenalty),
                AppliedRules = new List<PolicyRuleEvaluationItemDto> { costRule, consigneeRule, recommendationRule },
                RecommendedNextStep = isAutoApproved 
                    ? "Execute automated fleet dispatch immediately via SignalR."
                    : $"Route to Operations Manager for {requiredTier} authorization."
            };
        }

        public List<PolicyApprovalMatrixDto> GetApprovalMatrix()
        {
            return new List<PolicyApprovalMatrixDto>
            {
                new PolicyApprovalMatrixDto
                {
                    Tier = "Tier 1 (Autonomous)",
                    CostThreshold = "<= ₹ 25,000",
                    RequiredRole = "OpsRescue Autonomous Agent / Operations User",
                    Description = "Low-cost rerouting, carrier notifications, automated dry-ice recharge dispatch.",
                    ExampleScenario = "Minor route detour under 45 mins with zero temp excursion."
                },
                new PolicyApprovalMatrixDto
                {
                    Tier = "Tier 2 (Manager Authorized)",
                    CostThreshold = "₹ 25,001 - ₹ 1,00,000",
                    RequiredRole = "Operations Manager",
                    Description = "Prime mover vehicle swap, dedicated reefer cross-dock transfer, cargo offload.",
                    ExampleScenario = "INC-1004 Reefer Excursion on NH-544 (Dry-Ice Emergency Intervention)."
                },
                new PolicyApprovalMatrixDto
                {
                    Tier = "Tier 3 (Executive Escalation)",
                    CostThreshold = "> ₹ 1,00,000",
                    RequiredRole = "Director of Logistics / Admin",
                    Description = "Charter flight contingency, total shipment write-off, high-level SLA indemnity waiver.",
                    ExampleScenario = "Multi-vehicle pile-up blocking primary arterial corridor for > 12 hours."
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
