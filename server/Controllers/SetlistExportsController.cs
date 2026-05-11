using Microsoft.AspNetCore.Mvc;
using server.Logic.Exceptions;
using server.Logic.Interfaces;

namespace server.Controllers;

[ApiController]
[Route("api/bands/{bandId}/setlists/{setlistId}/exports")]
public class SetlistExportsController : ControllerBase
{
    private readonly ISetlistExportLogic _exportLogic;

    public SetlistExportsController(ISetlistExportLogic exportLogic)
    {
        _exportLogic = exportLogic;
    }

    [HttpGet("pdf")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExportPdf(string bandId, string setlistId)
    {
        try
        {
            var result = await _exportLogic.ExportSetlistPdfAsync(bandId, setlistId);
            Response.Headers["Content-Disposition"] =
                $"attachment; filename=\"{result.FileName}\"; filename*=UTF-8''{Uri.EscapeDataString(result.FileName)}";
            return File(result.Content, result.ContentType);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("docx")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExportDocx(string bandId, string setlistId)
    {
        try
        {
            var result = await _exportLogic.ExportSetlistDocxAsync(bandId, setlistId);
            Response.Headers["Content-Disposition"] =
                $"attachment; filename=\"{result.FileName}\"; filename*=UTF-8''{Uri.EscapeDataString(result.FileName)}";
            return File(result.Content, result.ContentType);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
