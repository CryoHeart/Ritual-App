using server.Models.Requests;
using server.Models.Responses;

namespace server.Logic.Interfaces;

public interface ILiveSessionsLogic
{
    Task<LiveSessionResponse> StartSessionAsync(string bandId, string setlistId, StartLiveSessionRequest request);

    Task<LiveSessionResponse> GetSessionAsync(string bandId, string sessionId);

    Task<LiveSessionResponse> NextSongAsync(string bandId, string sessionId);

    Task<LiveSessionResponse> PreviousSongAsync(string bandId, string sessionId);

    Task<LiveSessionResponse> EndSessionAsync(string bandId, string sessionId);
}
