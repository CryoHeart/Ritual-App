using Microsoft.EntityFrameworkCore;
using server.Dao.Entities;
using server.Dao.Interfaces;
using server.Data;

namespace server.Dao.Implementations;

public class BandsDao : IBandsDao
{
    private readonly RitualDbContext _db;

    public BandsDao(RitualDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyCollection<BandEntity>> GetAllAsync()
    {
        var rows = await _db.Bands.ToListAsync();
        return rows.Select(r => new BandEntity
        {
            Id = r.BandId,
            Name = r.Name,
            Description = r.Description,
            CreatedByUserId = r.CreatedByUserId,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        }).ToList();
    }

    public async Task<BandEntity?> GetByIdAsync(string bandId)
    {
        var r = await _db.Bands.FirstOrDefaultAsync(b => b.BandId == bandId);
        if (r == null) return null;
        return new BandEntity
        {
            Id = r.BandId,
            Name = r.Name,
            Description = r.Description,
            CreatedByUserId = r.CreatedByUserId,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        };
    }

    public async Task<BandEntity> CreateAsync(string name, string createdByUserId)
    {
        var r = new server.Data.Entities.BandEntity
        {
            BandId = Guid.NewGuid().ToString(),
            Name = name,
            CreatedByUserId = createdByUserId,
            CreatedAt = DateTime.UtcNow
        };
        _db.Bands.Add(r);
        await _db.SaveChangesAsync();
        return new BandEntity
        {
            Id = r.BandId,
            Name = r.Name,
            Description = r.Description,
            CreatedByUserId = r.CreatedByUserId,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        };
    }
}