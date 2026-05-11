namespace server.Models.Responses;

public class MusicBrainzTrackResponse
{
    public string Id { get; set; } = string.Empty;

    public string? RecordingId { get; set; }

    public string Title { get; set; } = string.Empty;

    public int? LengthMs { get; set; }

    public int? Position { get; set; }

    public string? Number { get; set; }

    public string? ArtistCredit { get; set; }
}
