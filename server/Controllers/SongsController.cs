using Microsoft.AspNetCore.Mvc;
using server.Logic.Exceptions;
using server.Logic.Interfaces;
using server.Models.Requests;
using server.Models.Responses;

namespace server.Controllers;

[ApiController]
[Route("api/bands/{bandId}/songs")]
public class SongsController : ControllerBase
{
    private readonly ISongsLogic _songsLogic;

    public SongsController(ISongsLogic songsLogic)
    {
        _songsLogic = songsLogic;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<SongResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<SongResponse>>> GetSongs(string bandId)
    {
        try
        {
            return Ok(await _songsLogic.GetSongsAsync(bandId));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet("{songId}")]
    [ProducesResponseType(typeof(SongResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SongResponse>> GetSong(string bandId, string songId)
    {
        try
        {
            return Ok(await _songsLogic.GetSongAsync(bandId, songId));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(SongResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SongResponse>> CreateSong(string bandId, [FromBody] CreateSongRequest request)
    {
        try
        {
            var created = await _songsLogic.CreateSongAsync(bandId, request);
            return CreatedAtAction(nameof(GetSong), new { bandId, songId = created.Id }, created);
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
