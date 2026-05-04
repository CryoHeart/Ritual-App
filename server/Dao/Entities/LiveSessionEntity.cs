namespace server.Dao.Entities;

public class LiveSessionEntity
{
    public string Id { get; set; } = string.Empty;

    public string BandId { get; set; } = string.Empty;

    public string SetlistId { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public int CurrentSongIndex { get; set; }

    public bool IsEnded => Status == "ended";

    public DateTime StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }
}