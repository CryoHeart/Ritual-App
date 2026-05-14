using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Logic.Exceptions;
using server.Logic.Interfaces;
using server.Models.Requests;
using server.Models.Responses;

namespace server.Controllers;

[ApiController]
[Route("api/bands")]
public class BandsController : ControllerBase
{
    private readonly IBandsLogic _bandsLogic;

    public BandsController(IBandsLogic bandsLogic)
    {
        _bandsLogic = bandsLogic;
    }

    private string? OptionalUserId => User.Identity?.IsAuthenticated == true
        ? User.FindFirstValue(ClaimTypes.NameIdentifier)
        : null;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<BandResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<BandResponse>>> GetBands([FromQuery] string? userId = null)
    {
        // Prefer JWT identity; fall back to legacy query-param for backwards compatibility.
        var effectiveUserId = OptionalUserId ?? userId;
        return Ok(await _bandsLogic.GetBandsAsync(effectiveUserId));
    }

    [HttpGet("{bandId}")]
    [ProducesResponseType(typeof(BandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BandResponse>> GetBand(string bandId)
    {
        try
        {
            return Ok(await _bandsLogic.GetBandAsync(bandId));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(BandResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BandResponse>> CreateBand([FromBody] CreateBandRequest request)
    {
        try
        {
            var created = await _bandsLogic.CreateBandAsync(request);
            return CreatedAtAction(nameof(GetBand), new { bandId = created.Id }, created);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{bandId}")]
    [ProducesResponseType(typeof(BandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BandResponse>> UpdateBandName(string bandId, [FromBody] UpdateBandNameRequest request)
    {
        try
        {
            return Ok(await _bandsLogic.UpdateBandNameAsync(bandId, request));
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

    [Authorize]
    [HttpGet("{bandId}/members")]
    [ProducesResponseType(typeof(IReadOnlyCollection<BandMemberResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<BandMemberResponse>>> GetMembers(string bandId)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            return Ok(await _bandsLogic.GetMembersAsync(userId, bandId));
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

    [Authorize]
    [HttpPost("{bandId}/members")]
    [ProducesResponseType(typeof(BandMemberResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BandMemberResponse>> AddMember(string bandId, [FromBody] AddBandMemberRequest request)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var added = await _bandsLogic.AddMemberAsync(userId, bandId, request);
            return Created(string.Empty, added);
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message });
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
