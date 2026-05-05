namespace server.Models.Requests;

public class CreateSetlistRequest
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}