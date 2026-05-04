using Microsoft.AspNetCore.Mvc;
using server.Logic.Exceptions;
using server.Logic.Interfaces;
using server.Models.Requests;
using server.Models.Responses;

namespace server.Controllers;

[ApiController]
[Route("api/bands/{bandId}")]
public class LiveSessionsController : ControllerBase
{
    private readonly ILiveSessionsLogic _liveSessionsLogic;

    public LiveSessionsController(ILiveSessionsLogic liveSessionsLogic)
    {
        _liveSessionsLogic = liveSessionsLogic;
    }

    [HttpPost("setlists/{setlistId}/live-sessions/start")]
    [ProducesResponseType(typeof(LiveSessionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LiveSessionResponse>> StartSession(string bandId, string setlistId, [FromBody] StartLiveSessionRequest request)
    {
        try
        {
            var created = await _liveSessionsLogic.StartSessionAsync(bandId, setlistId, request);
            return CreatedAtAction(nameof(GetSession), new { bandId, sessionId = created.Id }, created);
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

    [HttpGet("live-sessions/{sessionId}")]
    [ProducesResponseType(typeof(LiveSessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LiveSessionResponse>> GetSession(string bandId, string sessionId)
    {
        try
        {
            return Ok(await _liveSessionsLogic.GetSessionAsync(bandId, sessionId));
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

    [HttpPost("live-sessions/{sessionId}/next")]
    [ProducesResponseType(typeof(LiveSessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LiveSessionResponse>> Next(string bandId, string sessionId)
    {
        try
        {
            return Ok(await _liveSessionsLogic.NextSongAsync(bandId, sessionId));
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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LiveSessionResponse>> Previous(string bandId, string sessionId)
    {
        try
        {
            return Ok(await _liveSessionsLogic.PreviousSongAsync(bandId, sessionId));
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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LiveSessionResponse>> End(string bandId, string sessionId)
    {
        try
        {
            return Ok(await _liveSessionsLogic.EndSessionAsync(bandId, sessionId));
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
