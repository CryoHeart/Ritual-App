using server.Models.Requests;
using server.Models.Responses;

namespace server.Logic.Interfaces;

public interface IMusicBrainzLogic
{
    Task<IReadOnlyCollection<MusicBrainzArtistResponse>> SearchArtistsAsync(string query);

    Task<IReadOnlyCollection<MusicBrainzReleaseGroupResponse>> SearchReleaseGroupsAsync(string query);

    Task<IReadOnlyCollection<MusicBrainzRecordingResponse>> SearchRecordingsAsync(string query);

    Task<IReadOnlyCollection<MusicBrainzReleaseGroupResponse>> GetArtistAlbumsAsync(string artistMbid);

    Task<IReadOnlyCollection<MusicBrainzReleaseResponse>> GetReleaseGroupReleasesAsync(string releaseGroupMbid);

    Task<IReadOnlyCollection<MusicBrainzTrackResponse>> GetReleaseTracksAsync(string releaseMbid);

    Task<MusicBrainzImportSummaryResponse> ImportSelectionAsync(ImportMusicBrainzSelectionRequest request);
}
