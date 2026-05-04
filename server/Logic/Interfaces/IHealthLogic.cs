using server.Models.Responses;

namespace server.Logic.Interfaces;

public interface IHealthLogic
{
    Task<HealthStatusResponse> GetHealthStatusAsync();
}