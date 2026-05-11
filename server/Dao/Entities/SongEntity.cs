namespace server.Dao.Entities;

public class SongEntity
{
    public string Id { get; set; } = string.Empty;

    public string BandId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? MusicBrainzRecordingId { get; set; }

    public int? Bpm { get; set; }

    public int? DurationSeconds { get; set; }

    public string? Tuning { get; set; }

    public string? SongKey { get; set; }

    public string? Notes { get; set; }

    public string? AlbumId { get; set; }

    public string? AlbumTitle { get; set; }

    public int? AlbumTrackNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}