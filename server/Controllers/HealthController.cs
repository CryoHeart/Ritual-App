using Microsoft.AspNetCore.Mvc;
using server.Logic.Interfaces;
using server.Models.Responses;

namespace server.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    private readonly IHealthLogic _healthLogic;

    public HealthController(IHealthLogic healthLogic)
    {
        _healthLogic = healthLogic;
    }

    [HttpGet]
    [ProducesResponseType(typeof(HealthStatusResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<HealthStatusResponse>> GetHealth()
    {
        var response = await _healthLogic.GetHealthStatusAsync();
        return Ok(response);
    }
}