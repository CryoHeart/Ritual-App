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

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<BandResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<BandResponse>>> GetBands()
    {
        return Ok(await _bandsLogic.GetBandsAsync());
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
}
