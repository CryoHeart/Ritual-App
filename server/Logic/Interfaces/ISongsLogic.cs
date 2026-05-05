using server.Models.Requests;
using server.Models.Responses;

namespace server.Logic.Interfaces;

public interface ISongsLogic
{
    Task<IReadOnlyCollection<SongResponse>> GetSongsAsync(string bandId);

    Task<SongResponse> GetSongAsync(string bandId, string songId);

    Task<SongResponse> CreateSongAsync(string bandId, CreateSongRequest request);

    Task<SongResponse> UpdateSongAsync(string bandId, string songId, UpdateSongRequest request);

    Task DeleteSongAsync(string bandId, string songId);
}
