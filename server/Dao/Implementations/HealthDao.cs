using Microsoft.EntityFrameworkCore;
using server.Dao.Interfaces;
using server.Data;

namespace server.Dao.Implementations;

public class HealthDao : IHealthDao
{
    private readonly RitualDbContext _db;

    public HealthDao(RitualDbContext db)
    {
        _db = db;
    }

    public string GetHealthStatus()
    {
        return "RITUAL server is alive";
    }

    public async Task<bool> CanConnectToDatabaseAsync()
    {
        return await _db.Database.CanConnectAsync();
    }
}