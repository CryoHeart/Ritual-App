namespace server.Data.Entities;

public class SetlistSongEntity
{
    public string SetlistSongId { get; set; } = string.Empty;

    public string SetlistId { get; set; } = string.Empty;

    public string SongId { get; set; } = string.Empty;

    public int PositionIndex { get; set; }

    public string? TransitionNotes { get; set; }

    public string? PerformanceNotes { get; set; }

    public DateTime CreatedAt { get; set; }
}
