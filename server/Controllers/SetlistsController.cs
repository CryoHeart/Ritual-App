using Microsoft.AspNetCore.Mvc;
using server.Logic.Exceptions;
using server.Logic.Interfaces;
using server.Models.Requests;
using server.Models.Responses;

namespace server.Controllers;

[ApiController]
[Route("api/bands/{bandId}/setlists")]
public class SetlistsController : ControllerBase
{
    private readonly ISetlistsLogic _setlistsLogic;

    public SetlistsController(ISetlistsLogic setlistsLogic)
    {
        _setlistsLogic = setlistsLogic;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<SetlistResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<SetlistResponse>>> GetSetlists(string bandId)
    {
        try
        {
            return Ok(await _setlistsLogic.GetSetlistsAsync(bandId));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet("{setlistId}")]
    [ProducesResponseType(typeof(SetlistDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SetlistDetailsResponse>> GetSetlist(string bandId, string setlistId)
    {
        try
        {
            var details = await _setlistsLogic.GetSetlistDetailsAsync(bandId, setlistId);
            if (details is null)
            {
                return NotFound(new { error = "Setlist was not found." });
            }

            return Ok(details);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(SetlistResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SetlistResponse>> CreateSetlist(string bandId, [FromBody] CreateSetlistRequest request)
    {
        try
        {
            var created = await _setlistsLogic.CreateSetlistAsync(bandId, request);
            return CreatedAtAction(nameof(GetSetlist), new { bandId, setlistId = created.SetlistId }, created);
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

    [HttpPut("{setlistId}")]
    [ProducesResponseType(typeof(SetlistResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SetlistResponse>> UpdateSetlist(string bandId, string setlistId, [FromBody] UpdateSetlistRequest request)
    {
        try
        {
            var updated = await _setlistsLogic.UpdateSetlistAsync(bandId, setlistId, request);
            if (updated is null)
            {
                return NotFound(new { error = "Setlist was not found." });
            }

            return Ok(updated);
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

    [HttpDelete("{setlistId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSetlist(string bandId, string setlistId)
    {
        try
        {
            var deleted = await _setlistsLogic.DeleteSetlistAsync(bandId, setlistId);
            if (!deleted)
            {
                return NotFound(new { error = "Setlist was not found." });
            }

            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost("{setlistId}/songs")]
    [ProducesResponseType(typeof(SetlistDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SetlistDetailsResponse>> AddSongToSetlist(string bandId, string setlistId, [FromBody] AddSongToSetlistRequest request)
    {
        try
        {
            return Ok(await _setlistsLogic.AddSongToSetlistAsync(bandId, setlistId, request));
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ConflictException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpDelete("{setlistId}/songs/{setlistSongId}")]
    [ProducesResponseType(typeof(SetlistDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SetlistDetailsResponse>> RemoveSongFromSetlist(string bandId, string setlistId, string setlistSongId)
    {
        try
        {
            return Ok(await _setlistsLogic.RemoveSongFromSetlistAsync(bandId, setlistId, setlistSongId));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ConflictException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpPut("{setlistId}/songs/reorder")]
    [ProducesResponseType(typeof(SetlistDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SetlistDetailsResponse>> ReorderSetlistSongs(string bandId, string setlistId, [FromBody] ReorderSetlistSongsRequest request)
    {
        try
        {
            return Ok(await _setlistsLogic.ReorderSetlistSongsAsync(bandId, setlistId, request));
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ConflictException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }
}

