using Microsoft.AspNetCore.SignalR;

namespace OpsRescue.Hubs
{
    public class IncidentHub : Hub
    {
        public async Task JoinIncidentGroup(string incidentId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, incidentId.Trim().ToUpperInvariant());
        }

        public async Task LeaveIncidentGroup(string incidentId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, incidentId.Trim().ToUpperInvariant());
        }
    }
}
