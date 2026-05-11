namespace server.Models.Requests;

public class ImportMusicBrainzSelectionRequest
{
    public string UserId { get; set; } = string.Empty;

    public string BandId { get; set; } = string.Empty;

    public MusicBrainzArtistSelectionDto Artist { get; set; } = new();

    public IReadOnlyCollection<ImportedAlbumDto> Albums { get; set; } = Array.Empty<ImportedAlbumDto>();
}

public class MusicBrainzArtistSelectionDto
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Country { get; set; }
}

public class ImportedAlbumDto
{
    public string MusicBrainzReleaseGroupId { get; set; } = string.Empty;

    public string? MusicBrainzReleaseId { get; set; }

    public string Title { get; set; } = string.Empty;

    public int? ReleaseYear { get; set; }

    public IReadOnlyCollection<ImportedSongDto> Songs { get; set; } = Array.Empty<ImportedSongDto>();
}

public class ImportedSongDto
{
    public string? MusicBrainzRecordingId { get; set; }

    public string Title { get; set; } = string.Empty;

    public int? DurationSeconds { get; set; }

    public int? TrackNumber { get; set; }
}
