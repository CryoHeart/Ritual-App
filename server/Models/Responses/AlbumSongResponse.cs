namespace server.Models.Responses;

public class AlbumSongResponse
{
    public string SongId { get; set; } = string.Empty;

    public string BandId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public int? Bpm { get; set; }

    public int? DurationSeconds { get; set; }

    public string? AlbumId { get; set; }

    public string? AlbumTitle { get; set; }

    public int? AlbumTrackNumber { get; set; }

    public string? Tuning { get; set; }

    public string? SongKey { get; set; }

    public string? Notes { get; set; }
}
