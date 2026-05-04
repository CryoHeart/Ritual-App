using server.Models.Requests;
using server.Models.Responses;

namespace server.Logic.Interfaces;

public interface ISetlistsLogic
{
    Task<IReadOnlyCollection<SetlistResponse>> GetSetlistsAsync(string bandId);

    Task<SetlistResponse> GetSetlistAsync(string bandId, string setlistId);

    Task<SetlistResponse> CreateSetlistAsync(string bandId, CreateSetlistRequest request);

    Task<SetlistResponse> AddSongToSetlistAsync(string bandId, string setlistId, AddSongToSetlistRequest request);
}
