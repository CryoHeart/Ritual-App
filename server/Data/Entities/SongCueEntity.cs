namespace server.Data.Entities;

public class SongCueEntity
{
    public string SongCueId { get; set; } = string.Empty;

    public string SongId { get; set; } = string.Empty;

    public string CueType { get; set; } = string.Empty;

    public string CueText { get; set; } = string.Empty;

    public int? CueTimeSeconds { get; set; }

    public DateTime CreatedAt { get; set; }
}
