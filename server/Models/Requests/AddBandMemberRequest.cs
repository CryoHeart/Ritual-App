namespace server.Models.Requests;

public class AddBandMemberRequest
{
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "BandMember";
    public string? Instrument { get; set; }
}
