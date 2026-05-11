namespace server.Models.Export;

public class SetlistExportSong
{
    public int PositionIndex { get; set; }

    public string SongId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? AlbumTitle { get; set; }

    public int? AlbumTrackNumber { get; set; }

    public int? DurationSeconds { get; set; }

    public int? Bpm { get; set; }

    public string? Tuning { get; set; }

    public string? SongKey { get; set; }

    public string? Notes { get; set; }

    public string? TransitionNotes { get; set; }

    public string? PerformanceNotes { get; set; }
}
