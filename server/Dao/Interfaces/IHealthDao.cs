namespace server.Dao.Interfaces;

public interface IHealthDao
{
    string GetHealthStatus();

    Task<bool> CanConnectToDatabaseAsync();
}