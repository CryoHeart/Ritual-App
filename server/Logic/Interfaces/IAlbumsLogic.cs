using server.Models.Requests;
using server.Models.Responses;

namespace server.Logic.Interfaces;

public interface IAlbumsLogic
{
    Task<IReadOnlyCollection<AlbumResponse>> GetAlbumsAsync(string bandId);

    Task<IReadOnlyCollection<AlbumWithSongsResponse>> GetAlbumsWithSongsAsync(string bandId);

    Task<AlbumResponse> CreateAlbumAsync(string bandId, CreateAlbumRequest request);

    Task<AlbumResponse> UpdateAlbumAsync(string bandId, string albumId, UpdateAlbumRequest request);

    Task DeleteAlbumAsync(string bandId, string albumId);
}
