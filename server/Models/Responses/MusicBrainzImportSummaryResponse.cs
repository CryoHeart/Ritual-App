namespace server.Models.Responses;

public class MusicBrainzImportSummaryResponse
{
    public int ImportedBands { get; set; }

    public int ImportedAlbums { get; set; }

    public int ImportedSongs { get; set; }

    public int SkippedDuplicates { get; set; }
}
