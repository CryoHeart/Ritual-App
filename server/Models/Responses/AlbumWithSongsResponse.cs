namespace server.Models.Responses;

public class AlbumWithSongsResponse
{
    public string? AlbumId { get; set; }

    public string BandId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public int? ReleaseYear { get; set; }

    public int SortOrder { get; set; }

    public List<AlbumSongResponse> Songs { get; set; } = new();
}
