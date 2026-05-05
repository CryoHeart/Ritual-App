namespace server.Data.Entities;

public class AlbumEntity
{
    public string AlbumId { get; set; } = string.Empty;

    public string BandId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public int? ReleaseYear { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
