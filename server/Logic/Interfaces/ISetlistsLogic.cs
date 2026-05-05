using server.Models.Requests;
using server.Models.Responses;

namespace server.Logic.Interfaces;

public interface ISetlistsLogic
{
    Task<List<SetlistResponse>> GetSetlistsAsync(string bandId);

    Task<SetlistDetailsResponse?> GetSetlistDetailsAsync(string bandId, string setlistId);

    Task<SetlistResponse> CreateSetlistAsync(string bandId, CreateSetlistRequest request);

    Task<SetlistResponse?> UpdateSetlistAsync(string bandId, string setlistId, UpdateSetlistRequest request);

    Task<bool> DeleteSetlistAsync(string bandId, string setlistId);

    Task<SetlistDetailsResponse> AddSongToSetlistAsync(string bandId, string setlistId, AddSongToSetlistRequest request);

    Task<SetlistDetailsResponse> RemoveSongFromSetlistAsync(string bandId, string setlistId, string setlistSongId);

    Task<SetlistDetailsResponse> ReorderSetlistSongsAsync(string bandId, string setlistId, ReorderSetlistSongsRequest request);
}
