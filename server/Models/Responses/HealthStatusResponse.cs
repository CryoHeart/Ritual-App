namespace server.Models.Responses;

public class HealthStatusResponse
{
    public string Status { get; set; } = string.Empty;

    public string Database { get; set; } = string.Empty;
}