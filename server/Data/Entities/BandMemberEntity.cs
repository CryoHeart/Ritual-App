namespace server.Data.Entities;

public class BandMemberEntity
{
    public string BandMemberId { get; set; } = string.Empty;

    public string BandId { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public string? Instrument { get; set; }

    public DateTime JoinedAt { get; set; }
}
