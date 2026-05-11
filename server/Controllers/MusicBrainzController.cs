using Microsoft.AspNetCore.Mvc;
using server.Logic.Exceptions;
using server.Logic.Interfaces;
using server.Models.Requests;
using server.Models.Responses;

namespace server.Controllers;

[ApiController]
[Route("api/musicbrainz")]
public class MusicBrainzController : ControllerBase
{
    private readonly IMusicBrainzLogic _musicBrainzLogic;

    public MusicBrainzController(IMusicBrainzLogic musicBrainzLogic)
    {
        _musicBrainzLogic = musicBrainzLogic;
    }

    [HttpGet("search-artists")]
    [ProducesResponseType(typeof(IReadOnlyCollection<MusicBrainzArtistResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<MusicBrainzArtistResponse>>> SearchArtists([FromQuery] string query)
    {
        try
        {
            return Ok(await _musicBrainzLogic.SearchArtistsAsync(query));
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new { error = $"MusicBrainz request failed: {ex.Message}" });
        }
    }

    [HttpGet("search-release-groups")]
    [ProducesResponseType(typeof(IReadOnlyCollection<MusicBrainzReleaseGroupResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<MusicBrainzReleaseGroupResponse>>> SearchReleaseGroups([FromQuery] string query)
    {
        try
        {
            return Ok(await _musicBrainzLogic.SearchReleaseGroupsAsync(query));
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new { error = $"MusicBrainz request failed: {ex.Message}" });
        }
    }

    [HttpGet("search-recordings")]
    [ProducesResponseType(typeof(IReadOnlyCollection<MusicBrainzRecordingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<MusicBrainzRecordingResponse>>> SearchRecordings([FromQuery] string query)
    {
        try
        {
            return Ok(await _musicBrainzLogic.SearchRecordingsAsync(query));
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new { error = $"MusicBrainz request failed: {ex.Message}" });
        }
    }

    [HttpGet("artist/{mbid}/albums")]
    [ProducesResponseType(typeof(IReadOnlyCollection<MusicBrainzReleaseGroupResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<MusicBrainzReleaseGroupResponse>>> GetArtistAlbums(string mbid)
    {
        try
        {
            return Ok(await _musicBrainzLogic.GetArtistAlbumsAsync(mbid));
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new { error = $"MusicBrainz request failed: {ex.Message}" });
        }
    }

    [HttpGet("release-group/{mbid}/releases")]
    [ProducesResponseType(typeof(IReadOnlyCollection<MusicBrainzReleaseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<MusicBrainzReleaseResponse>>> GetReleaseGroupReleases(string mbid)
    {
        try
        {
            return Ok(await _musicBrainzLogic.GetReleaseGroupReleasesAsync(mbid));
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new { error = $"MusicBrainz request failed: {ex.Message}" });
        }
    }

    [HttpGet("release/{mbid}/tracks")]
    [ProducesResponseType(typeof(IReadOnlyCollection<MusicBrainzTrackResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<MusicBrainzTrackResponse>>> GetReleaseTracks(string mbid)
    {
        try
        {
            return Ok(await _musicBrainzLogic.GetReleaseTracksAsync(mbid));
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new { error = $"MusicBrainz request failed: {ex.Message}" });
        }
    }

    [HttpPost("import-selection")]
    [ProducesResponseType(typeof(MusicBrainzImportSummaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MusicBrainzImportSummaryResponse>> ImportSelection([FromBody] ImportMusicBrainzSelectionRequest request)
    {
        try
        {
            return Ok(await _musicBrainzLogic.ImportSelectionAsync(request));
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}
