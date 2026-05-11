using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;

namespace server.Controllers;

[ApiController]
[Route("api")]
public class DbTestController : ControllerBase
{
    private readonly RitualDbContext _db;
    private readonly ILogger<DbTestController> _logger;

    public DbTestController(RitualDbContext db, ILogger<DbTestController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet("db-test")]
    public async Task<IActionResult> DbTest()
    {
        try
        {
            var connection = _db.Database.GetDbConnection();
            var shouldClose = connection.State != ConnectionState.Open;
            if (shouldClose)
            {
                await connection.OpenAsync();
            }

            try
            {
                await using var command = connection.CreateCommand();
                command.CommandText = "SELECT DATABASE() AS db, NOW() AS now";

                await using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new
                    {
                        error = "Database test query returned no rows."
                    });
                }

                return Ok(new
                {
                    db = reader["db"]?.ToString(),
                    now = reader["now"]
                });
            }
            finally
            {
                if (shouldClose)
                {
                    await connection.CloseAsync();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database test endpoint failed.");
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                error = "Database test failed. Check server logs for details."
            });
        }
    }
}
