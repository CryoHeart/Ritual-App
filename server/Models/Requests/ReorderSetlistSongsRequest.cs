namespace server.Models.Requests;

public class ReorderSetlistSongsRequest
{
    public List<ReorderItem> Items { get; set; } = new();

    public class ReorderItem
    {
        public string SetlistSongId { get; set; } = string.Empty;

        public int PositionIndex { get; set; }
    }
}
