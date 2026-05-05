namespace server.Models.Requests;

public class UpdateAlbumRequest
{
    public string Title { get; set; } = string.Empty;

    public int? ReleaseYear { get; set; }

    public int SortOrder { get; set; }
}
