namespace server.Models.Responses;

public class SetlistDetailsResponse
{
    public string SetlistId { get; set; } = string.Empty;

    public string BandId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int TotalSongs { get; set; }

    public int TotalDurationSeconds { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public List<SetlistSongResponse> Songs { get; set; } = new();
}
