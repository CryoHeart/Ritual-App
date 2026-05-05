namespace server.Models.Requests;

public class CreateSongRequest
{
    public string Title { get; set; } = string.Empty;

    public string? AlbumId { get; set; }

    public int? AlbumTrackNumber { get; set; }

    public int? DurationSeconds { get; set; }

    public int? Bpm { get; set; }

    public string? Tuning { get; set; }

    public string? SongKey { get; set; }

    public string? Notes { get; set; }
}