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
            Country = r.Country,
            MusicBrainzArtistId = r.MusicBrainzArtistId,
            CreatedByUserId = r.CreatedByUserId,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        }).ToList();
    }

    public async Task<IReadOnlyCollection<BandEntity>> GetByUserIdAsync(string userId)
    {
        var rows = await _db.Bands
            .Where(b => _db.BandMembers.Any(m => m.BandId == b.BandId && m.UserId == userId))
            .ToListAsync();
        return rows.Select(r => new BandEntity
        {
            Id = r.BandId,
            Name = r.Name,
            Description = r.Description,
            Country = r.Country,
            MusicBrainzArtistId = r.MusicBrainzArtistId,
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
            Country = r.Country,
            MusicBrainzArtistId = r.MusicBrainzArtistId,
            CreatedByUserId = r.CreatedByUserId,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        };
    }

    public async Task<BandEntity> CreateAsync(string name, string createdByUserId)
    {
        var band = new server.Data.Entities.BandEntity
        {
            BandId = Guid.NewGuid().ToString(),
            Name = name,
            CreatedByUserId = createdByUserId,
            CreatedAt = DateTime.UtcNow
        };
        _db.Bands.Add(band);
        await _db.SaveChangesAsync(); // persist band first so FK is satisfied

        var member = new server.Data.Entities.BandMemberEntity
        {
            BandMemberId = Guid.NewGuid().ToString(),
            BandId = band.BandId,
            UserId = createdByUserId,
            Role = "Admin",
            Instrument = "Band Account",
            JoinedAt = band.CreatedAt
        };
        _db.BandMembers.Add(member);
        await _db.SaveChangesAsync();

        return new BandEntity
        {
            Id = band.BandId,
            Name = band.Name,
            Description = band.Description,
            Country = band.Country,
            MusicBrainzArtistId = band.MusicBrainzArtistId,
            CreatedByUserId = band.CreatedByUserId,
            CreatedAt = band.CreatedAt,
            UpdatedAt = band.UpdatedAt
        };
    }

    public async Task<BandEntity?> UpdateNameAsync(string bandId, string name)
    {
        var band = await _db.Bands.FirstOrDefaultAsync(b => b.BandId == bandId);
        if (band is null) return null;

        band.Name = name;
        band.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return new BandEntity
        {
            Id = band.BandId,
            Name = band.Name,
            Description = band.Description,
            Country = band.Country,
            MusicBrainzArtistId = band.MusicBrainzArtistId,
            CreatedByUserId = band.CreatedByUserId,
            CreatedAt = band.CreatedAt,
            UpdatedAt = band.UpdatedAt
        };
    }
}