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
        var rows = await (
            from b in _db.Bands
            join m in _db.BandMembers on b.BandId equals m.BandId
            where m.UserId == userId
            select new { Band = b, m.Role }
        ).ToListAsync();

        return rows.Select(r => new BandEntity
        {
            Id = r.Band.BandId,
            Name = r.Band.Name,
            Description = r.Band.Description,
            Country = r.Band.Country,
            MusicBrainzArtistId = r.Band.MusicBrainzArtistId,
            CreatedByUserId = r.Band.CreatedByUserId,
            CreatedAt = r.Band.CreatedAt,
            UpdatedAt = r.Band.UpdatedAt,
            UserRole = r.Role
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
            Role = "RitualLeader",
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

    public async Task<string?> GetUserRoleInBandAsync(string userId, string bandId)
        => await _db.BandMembers
            .Where(m => m.UserId == userId && m.BandId == bandId)
            .Select(m => m.Role)
            .FirstOrDefaultAsync();

    public async Task<bool> IsMemberAsync(string userId, string bandId)
        => await _db.BandMembers.AnyAsync(m => m.UserId == userId && m.BandId == bandId);

    public async Task<IReadOnlyCollection<(string BandMemberId, string UserId, string DisplayName, string Email, string Role, string? Instrument, DateTime JoinedAt)>> GetMembersAsync(string bandId)
    {
        var rows = await (
            from m in _db.BandMembers
            join u in _db.Users on m.UserId equals u.UserId
            where m.BandId == bandId
            orderby m.JoinedAt
            select new { m.BandMemberId, m.UserId, u.DisplayName, u.Email, m.Role, m.Instrument, m.JoinedAt }
        ).ToListAsync();

        return rows.Select(r => (r.BandMemberId, r.UserId, r.DisplayName, r.Email, r.Role, r.Instrument, r.JoinedAt)).ToList();
    }

    public async Task AddMemberAsync(string bandId, string userId, string role, string? instrument)
    {
        var member = new server.Data.Entities.BandMemberEntity
        {
            BandMemberId = Guid.NewGuid().ToString(),
            BandId = bandId,
            UserId = userId,
            Role = role,
            Instrument = instrument,
            JoinedAt = DateTime.UtcNow
        };
        _db.BandMembers.Add(member);
        await _db.SaveChangesAsync();
    }
}