using server.Dao.Interfaces;
using server.Logic.Interfaces;
using server.Models.Responses;

namespace server.Logic.Implementations;

public class HealthLogic : IHealthLogic
{
    private readonly IHealthDao _healthDao;

    public HealthLogic(IHealthDao healthDao)
    {
        _healthDao = healthDao;
    }

    public async Task<HealthStatusResponse> GetHealthStatusAsync()
    {
        var canConnect = await _healthDao.CanConnectToDatabaseAsync();

        return new HealthStatusResponse
        {
            Status = "RITUAL server is alive",
            Database = canConnect ? "connected" : "disconnected"
        };
    }
}