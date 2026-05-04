namespace server.Models.Responses;

public class SongResponse
{
    public string Id { get; set; } = string.Empty;

    public string BandId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public int? Bpm { get; set; }

    public int? DurationSeconds { get; set; }

    public string? Tuning { get; set; }

    public string? SongKey { get; set; }

    public string? Notes { get; set; }
}