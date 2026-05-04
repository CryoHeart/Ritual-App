namespace server.Data.Entities;

public class SetlistEntity
{
    public string SetlistId { get; set; } = string.Empty;

    public string BandId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
