using server.Models.Responses;

namespace server.Logic.Interfaces;

public interface ILiveSessionsLogic
{
    Task<LiveSessionResponse> StartSessionAsync(string userId, string bandId, string setlistId);

    Task<LiveSessionResponse?> GetActiveSessionAsync(string userId, string bandId);

    Task<LiveSessionResponse> GetSessionAsync(string userId, string bandId, string sessionId);

    Task<LiveSessionResponse> NextSongAsync(string userId, string bandId, string sessionId);

    Task<LiveSessionResponse> PreviousSongAsync(string userId, string bandId, string sessionId);

    Task<LiveSessionResponse> EndSessionAsync(string userId, string bandId, string sessionId);
}
