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
    [ProducesResponseType(typeof(IReadOnlyCollection<SetlistResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<SetlistResponse>>> GetSetlists(string bandId)
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
    [ProducesResponseType(typeof(SetlistResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SetlistResponse>> GetSetlist(string bandId, string setlistId)
    {
        try
        {
            return Ok(await _setlistsLogic.GetSetlistAsync(bandId, setlistId));
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

    [HttpPost]
    [ProducesResponseType(typeof(SetlistResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SetlistResponse>> CreateSetlist(string bandId, [FromBody] CreateSetlistRequest request)
    {
        try
        {
            var created = await _setlistsLogic.CreateSetlistAsync(bandId, request);
            return CreatedAtAction(nameof(GetSetlist), new { bandId, setlistId = created.Id }, created);
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

    [HttpPost("{setlistId}/songs")]
    [ProducesResponseType(typeof(SetlistResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SetlistResponse>> AddSongToSetlist(string bandId, string setlistId, [FromBody] AddSongToSetlistRequest request)
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
}
