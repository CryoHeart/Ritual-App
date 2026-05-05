using Microsoft.AspNetCore.Mvc;
using server.Logic.Exceptions;
using server.Logic.Interfaces;
using server.Models.Requests;
using server.Models.Responses;

namespace server.Controllers;

[ApiController]
[Route("api/bands/{bandId}/albums")]
public class AlbumsController : ControllerBase
{
    private readonly IAlbumsLogic _albumsLogic;

    public AlbumsController(IAlbumsLogic albumsLogic)
    {
        _albumsLogic = albumsLogic;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<AlbumResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<AlbumResponse>>> GetAlbums(string bandId)
    {
        try
        {
            return Ok(await _albumsLogic.GetAlbumsAsync(bandId));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet("with-songs")]
    [ProducesResponseType(typeof(IReadOnlyCollection<AlbumWithSongsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<AlbumWithSongsResponse>>> GetAlbumsWithSongs(string bandId)
    {
        try
        {
            return Ok(await _albumsLogic.GetAlbumsWithSongsAsync(bandId));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet("/api/bands/{bandId}/albums-with-songs")]
    [ProducesResponseType(typeof(IReadOnlyCollection<AlbumWithSongsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<ActionResult<IReadOnlyCollection<AlbumWithSongsResponse>>> GetAlbumsWithSongsAlias(string bandId)
    {
        return GetAlbumsWithSongs(bandId);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AlbumResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlbumResponse>> CreateAlbum(string bandId, [FromBody] CreateAlbumRequest request)
    {
        try
        {
            var created = await _albumsLogic.CreateAlbumAsync(bandId, request);
            return CreatedAtAction(nameof(GetAlbums), new { bandId }, created);
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

    [HttpPut("{albumId}")]
    [ProducesResponseType(typeof(AlbumResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlbumResponse>> UpdateAlbum(string bandId, string albumId, [FromBody] UpdateAlbumRequest request)
    {
        try
        {
            return Ok(await _albumsLogic.UpdateAlbumAsync(bandId, albumId, request));
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

    [HttpDelete("{albumId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAlbum(string bandId, string albumId)
    {
        try
        {
            await _albumsLogic.DeleteAlbumAsync(bandId, albumId);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}
