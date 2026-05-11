using server.Models.Responses;

namespace server.Logic.Interfaces;

public interface IMusicBrainzClient
{
    Task<IReadOnlyCollection<MusicBrainzArtistResponse>> SearchArtistsAsync(string query);

    Task<IReadOnlyCollection<MusicBrainzReleaseGroupResponse>> SearchReleaseGroupsAsync(string query);

    Task<IReadOnlyCollection<MusicBrainzRecordingResponse>> SearchRecordingsAsync(string query);

    Task<IReadOnlyCollection<MusicBrainzReleaseGroupResponse>> GetArtistAlbumsAsync(string artistMbid);

    Task<IReadOnlyCollection<MusicBrainzReleaseResponse>> GetReleaseGroupReleasesAsync(string releaseGroupMbid);

    Task<IReadOnlyCollection<MusicBrainzTrackResponse>> GetReleaseTracksAsync(string releaseMbid);
}
