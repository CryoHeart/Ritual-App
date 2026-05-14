using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Logic.Exceptions;
using server.Logic.Interfaces;
using server.Models.Responses;

namespace server.Controllers;

[ApiController]
[Route("api/bands/{bandId}")]
[Authorize]
public class LiveSessionsController : ControllerBase
{
    private readonly ILiveSessionsLogic _liveSessionsLogic;

    public LiveSessionsController(ILiveSessionsLogic liveSessionsLogic)
    {
        _liveSessionsLogic = liveSessionsLogic;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPost("setlists/{setlistId}/live-sessions/start")]
    [ProducesResponseType(typeof(LiveSessionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> StartSession(string bandId, string setlistId)
    {
        try
        {
            var created = await _liveSessionsLogic.StartSessionAsync(UserId, bandId, setlistId);
            return CreatedAtAction(nameof(GetSession), new { bandId, sessionId = created.LiveSessionId }, created);
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message });
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

    [HttpGet("live-sessions/active")]
    [ProducesResponseType(typeof(LiveSessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetActiveSession(string bandId)
    {
        try
        {
            var session = await _liveSessionsLogic.GetActiveSessionAsync(UserId, bandId);
            if (session is null) return NoContent();
            return Ok(session);
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet("live-sessions/{sessionId}")]
    [ProducesResponseType(typeof(LiveSessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetSession(string bandId, string sessionId)
    {
        try
        {
            return Ok(await _liveSessionsLogic.GetSessionAsync(UserId, bandId, sessionId));
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost("live-sessions/{sessionId}/next")]
    [ProducesResponseType(typeof(LiveSessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Next(string bandId, string sessionId)
    {
        try
        {
            return Ok(await _liveSessionsLogic.NextSongAsync(UserId, bandId, sessionId));
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message });
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

    [HttpPost("live-sessions/{sessionId}/previous")]
    [ProducesResponseType(typeof(LiveSessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Previous(string bandId, string sessionId)
    {
        try
        {
            return Ok(await _liveSessionsLogic.PreviousSongAsync(UserId, bandId, sessionId));
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message });
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

    [HttpPost("live-sessions/{sessionId}/end")]
    [ProducesResponseType(typeof(LiveSessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> End(string bandId, string sessionId)
    {
        try
        {
            return Ok(await _liveSessionsLogic.EndSessionAsync(UserId, bandId, sessionId));
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message });
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
