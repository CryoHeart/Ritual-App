namespace server.Dao.Entities;

public class SongEntity
{
    public string Id { get; set; } = string.Empty;

    public string BandId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public int? Bpm { get; set; }

    public int? DurationSeconds { get; set; }

    public string? Tuning { get; set; }

    public string? SongKey { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}