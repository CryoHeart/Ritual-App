namespace server.Models.Requests;

public class CreateSongRequest
{
    public string Title { get; set; } = string.Empty;

    public int? Bpm { get; set; }

    public int? DurationSeconds { get; set; }
}