using server.Models.Requests;
using server.Models.Responses;

namespace server.Logic.Interfaces;

public interface ISongsLogic
{
    Task<IReadOnlyCollection<SongResponse>> GetSongsAsync(string bandId);

    Task<SongResponse> GetSongAsync(string bandId, string songId);

    Task<SongResponse> CreateSongAsync(string bandId, CreateSongRequest request);
}
