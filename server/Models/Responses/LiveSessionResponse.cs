namespace server.Models.Responses;

public class LiveSessionResponse
{
    public string Id { get; set; } = string.Empty;

    public string BandId { get; set; } = string.Empty;

    public string SetlistId { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public bool IsEnded { get; set; }

    public int CurrentSongIndex { get; set; }

    public int TotalSongs { get; set; }

    public string? CurrentSongId { get; set; }

    public string? CurrentSongTitle { get; set; }
}