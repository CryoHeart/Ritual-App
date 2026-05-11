namespace server.Models.Responses;

public class MusicBrainzReleaseGroupResponse
{
    public string Id { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? FirstReleaseDate { get; set; }

    public string? PrimaryType { get; set; }

    public IReadOnlyCollection<string> SecondaryTypes { get; set; } = Array.Empty<string>();

    public string? ArtistCredit { get; set; }

    public string? ArtistId { get; set; }

    public int? Score { get; set; }
}
