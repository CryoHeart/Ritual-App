namespace server.Dao.Entities;

public class BandEntity
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Country { get; set; }

    public string? MusicBrainzArtistId { get; set; }

    public string CreatedByUserId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    /// <summary>The requesting user's role in this band. Populated only when querying by userId.</summary>
    public string? UserRole { get; set; }
}