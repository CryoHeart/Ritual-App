namespace server.Dao.Entities;

public class SetlistSongEntity
{
    public string SetlistSongId { get; set; } = string.Empty;

    public string SetlistId { get; set; } = string.Empty;

    public string SongId { get; set; } = string.Empty;

    public int PositionIndex { get; set; }

    public string? TransitionNotes { get; set; }

    public string? PerformanceNotes { get; set; }

    // Song details (populated via join)
    public string Title { get; set; } = string.Empty;

    public int? Bpm { get; set; }

    public int? DurationSeconds { get; set; }

    public string? Tuning { get; set; }

    public string? SongKey { get; set; }

    public string? Notes { get; set; }

    public string? AlbumId { get; set; }

    public string? AlbumTitle { get; set; }

    public int? AlbumTrackNumber { get; set; }
}
