namespace server.Models.Requests;

public class UpdateSetlistRequest
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}
