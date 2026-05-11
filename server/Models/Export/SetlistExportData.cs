namespace server.Models.Export;

public class SetlistExportData
{
    public string BandId { get; set; } = string.Empty;

    public string BandName { get; set; } = string.Empty;

    public string SetlistId { get; set; } = string.Empty;

    public string SetlistName { get; set; } = string.Empty;

    public string? SetlistDescription { get; set; }

    public int TotalSongs { get; set; }

    public int TotalDurationSeconds { get; set; }

    public DateTime ExportedAt { get; set; }

    public List<SetlistExportSong> Songs { get; set; } = new();
}
