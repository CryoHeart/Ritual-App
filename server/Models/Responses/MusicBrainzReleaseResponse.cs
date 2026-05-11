namespace server.Models.Responses;

public class MusicBrainzReleaseResponse
{
    public string Id { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? Date { get; set; }

    public string? Country { get; set; }

    public string? Status { get; set; }

    public int? TrackCount { get; set; }
}
