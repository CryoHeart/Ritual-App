namespace server.Models.Responses;

public class MusicBrainzArtistResponse
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Country { get; set; }

    public string? Disambiguation { get; set; }

    public string? Type { get; set; }

    public int? Score { get; set; }
}
