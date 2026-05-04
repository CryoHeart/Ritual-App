namespace server.Models.Responses;

public class SetlistSongResponse
{
    public string SongId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public int? Bpm { get; set; }

    public int? DurationSeconds { get; set; }

    public int Order { get; set; }

    public string? TransitionNotes { get; set; }

    public string? PerformanceNotes { get; set; }
}