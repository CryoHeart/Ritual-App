namespace server.Models.Responses;

public class SetlistResponse
{
    public string Id { get; set; } = string.Empty;

    public string BandId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public List<SetlistSongResponse> Songs { get; set; } = new();
}