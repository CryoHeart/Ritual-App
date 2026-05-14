namespace server.Models.Responses;

public class LiveSessionResponse
{
    public string LiveSessionId { get; set; } = string.Empty;

    public string BandId { get; set; } = string.Empty;

    public string SetlistId { get; set; } = string.Empty;

    public string SetlistName { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public int CurrentPositionIndex { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public string? StartedByUserId { get; set; }

    public string? StartedByDisplayName { get; set; }

    public LiveSessionSongResponse? CurrentSong { get; set; }

    public LiveSessionSongResponse? NextSong { get; set; }

    public int TotalSongs { get; set; }

    public int TotalDurationSeconds { get; set; }

    public bool CanControl { get; set; }
}

public class LiveSessionSongResponse
{
    public string SetlistSongId { get; set; } = string.Empty;

    public string SongId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? AlbumTitle { get; set; }

    public int? AlbumTrackNumber { get; set; }

    public int? DurationSeconds { get; set; }

    public int? Bpm { get; set; }

    public string? Tuning { get; set; }

    public string? SongKey { get; set; }

    public string? Notes { get; set; }

    public string? TransitionNotes { get; set; }

    public string? PerformanceNotes { get; set; }

    public int PositionIndex { get; set; }
}