namespace server.Data.Entities;

public class LiveSessionEntity
{
    public string LiveSessionId { get; set; } = string.Empty;

    public string BandId { get; set; } = string.Empty;

    public string SetlistId { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public int CurrentPositionIndex { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }
}
