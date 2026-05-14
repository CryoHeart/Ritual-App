namespace server.Models.Responses;

public class BandResponse
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Country { get; set; }

    public string? MusicBrainzArtistId { get; set; }

    /// <summary>The authenticated user's role in this band. Null when queried without user context.</summary>
    public string? Role { get; set; }
}