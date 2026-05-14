using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace server.Hubs;

[Authorize]
public class LiveRitualHub : Hub
{
    public async Task JoinBand(string bandId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"band:{bandId}");
    }

    public async Task LeaveBand(string bandId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"band:{bandId}");
    }

    public async Task JoinLiveSession(string liveSessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"live-session:{liveSessionId}");
    }

    public async Task LeaveLiveSession(string liveSessionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"live-session:{liveSessionId}");
    }
}
