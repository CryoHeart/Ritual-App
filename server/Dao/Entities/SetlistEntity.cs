namespace server.Dao.Entities;

public class SetlistEntity
{
    public string Id { get; set; } = string.Empty;

    public string BandId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int TotalSongs { get; set; }

    public int TotalDurationSeconds { get; set; }

    public List<SetlistSongEntity> Songs { get; set; } = new();
}