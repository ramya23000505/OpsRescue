using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OpsRescue.Data;
using OpsRescue.DTOs;
using OpsRescue.Hubs;
using OpsRescue.Interface;
using OpsRescue.Models;

namespace OpsRescue.Services
{
    public class ExecutionService : IExecutionService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<IncidentHub> _hubContext;

        public ExecutionService(AppDbContext context, IHubContext<IncidentHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<DispatchExecutionResponseDto> DispatchExecutionAsync(DispatchExecutionRequestDto dto, string approvedBy)
        {
            var normalizedIncidentId = dto.IncidentId.Trim().ToUpperInvariant();
            var normalizedStrategyId = dto.StrategyId.Trim().ToUpperInvariant();

            var incident = await _context.Incidents
                .FirstOrDefaultAsync(i => i.IncidentId == normalizedIncidentId);

            if (incident == null)
            {
                throw new KeyNotFoundException($"Incident '{dto.IncidentId}' not found.");
            }

            var strategy = await _context.SimulationStrategies
                .FirstOrDefaultAsync(s => s.IncidentId == normalizedIncidentId && s.StrategyId == normalizedStrategyId);

            var strategyName = strategy != null ? strategy.Title : $"Recovery Plan ({normalizedStrategyId})";
            var approvalTier = strategy != null ? strategy.RequiredApprovalTier : "Tier 1 Auto-Approved";

            var executionId = $"EXEC-{Random.Shared.Next(100, 999)}";
            var now = DateTime.UtcNow;
            var estimatedRendezvous = now.AddMinutes(35);

            var partner = !string.IsNullOrWhiteSpace(dto.AssignedPartner)
                ? dto.AssignedPartner
                : "TransExpress Fast-Response Unit";

            var vehicle = !string.IsNullOrWhiteSpace(dto.VehicleId)
                ? dto.VehicleId
                : "TRK-RELIEF-401";

            var location = "Highway Fast-Track Corridor";

            var execution = new RecoveryExecution
            {
                ExecutionId = executionId,
                IncidentId = normalizedIncidentId,
                StrategyId = normalizedStrategyId,
                StrategyName = strategyName,
                DispatchedAt = now,
                EstimatedRendezvousAt = estimatedRendezvous,
                Status = "Dispatched",
                AssignedPartner = partner,
                VehicleId = vehicle,
                CurrentLocation = location,
                ActionNotes = dto.ActionNotes ?? $"Dispatched under {approvalTier} policy guidelines."
            };

            var approvalLog = new ManagerApprovalLog
            {
                IncidentId = normalizedIncidentId,
                StrategyId = normalizedStrategyId,
                StrategyName = strategyName,
                ApprovedBy = string.IsNullOrWhiteSpace(approvedBy) ? "Operations Manager" : approvedBy,
                ApprovedAt = now,
                ApprovalTier = approvalTier,
                Notes = dto.ActionNotes
            };

            // Update incident status
            incident.Status = "Dispatched";

            _context.RecoveryExecutions.Add(execution);
            _context.ManagerApprovalLogs.Add(approvalLog);
            await _context.SaveChangesAsync();

            var response = new DispatchExecutionResponseDto
            {
                ExecutionId = executionId,
                IncidentId = normalizedIncidentId,
                StrategyId = normalizedStrategyId,
                StrategyName = strategyName,
                Status = "Dispatched",
                DispatchedAt = now,
                DispatchedAtFormatted = now.ToString("yyyy-MM-dd HH:mm:ss"),
                EstimatedRendezvousAt = estimatedRendezvous,
                EstimatedRendezvousFormatted = estimatedRendezvous.ToString("yyyy-MM-dd HH:mm:ss"),
                AssignedPartner = partner,
                VehicleId = vehicle,
                CurrentLocation = location,
                ApprovalTier = approvalTier,
                Message = $"Strategy '{strategyName}' successfully approved and dispatched. Live tracking initiated."
            };

            // Broadcast real-time SignalR notifications
            try
            {
                await _hubContext.Clients.All.SendAsync("ExecutionDispatched", response);
                await _hubContext.Clients.Group(normalizedIncidentId).SendAsync("IncidentStatusChanged", new
                {
                    incidentId = normalizedIncidentId,
                    status = "Dispatched",
                    timestamp = now
                });
            }
            catch
            {
                // Non-blocking if clients not connected yet
            }

            return response;
        }

        public async Task<RecoveryExecutionSummaryDto?> GetExecutionStatusAsync(string incidentId)
        {
            var normalizedId = incidentId.Trim().ToUpperInvariant();

            var exec = await _context.RecoveryExecutions
                .AsNoTracking()
                .Where(e => e.IncidentId == normalizedId)
                .OrderByDescending(e => e.DispatchedAt)
                .FirstOrDefaultAsync();

            if (exec == null)
            {
                return null;
            }

            return new RecoveryExecutionSummaryDto
            {
                ExecutionId = exec.ExecutionId,
                StrategyId = exec.StrategyId,
                StrategyName = exec.StrategyName,
                Status = exec.Status,
                DispatchedAtFormatted = exec.DispatchedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                EstimatedRendezvousFormatted = exec.EstimatedRendezvousAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "TBD",
                AssignedPartner = exec.AssignedPartner,
                VehicleId = exec.VehicleId,
                CurrentLocation = exec.CurrentLocation
            };
        }
    }
}
