namespace server.Models.Responses;

public class MusicBrainzRecordingResponse
{
    public string Id { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public int? LengthMs { get; set; }

    public string? ArtistCredit { get; set; }

    public string? ArtistId { get; set; }

    public string? ReleaseTitle { get; set; }

    public int? Score { get; set; }
}
