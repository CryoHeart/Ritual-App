namespace server.Models.Responses;

public class BandMemberResponse
{
    public string BandMemberId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? Instrument { get; set; }
    public DateTime JoinedAt { get; set; }
}
