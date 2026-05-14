using server.Dao.Interfaces;
using server.Logic.Exceptions;
using server.Logic.Interfaces;
using server.Models.Requests;
using server.Models.Responses;

namespace server.Logic.Implementations;

public class BandsLogic : IBandsLogic
{
    private readonly IBandsDao _bandsDao;
    private readonly IUsersDao _usersDao;

    public BandsLogic(IBandsDao bandsDao, IUsersDao usersDao)
    {
        _bandsDao = bandsDao;
        _usersDao = usersDao;
    }

    public async Task<IReadOnlyCollection<BandResponse>> GetBandsAsync(string? userId = null)
    {
        var bands = userId is not null
            ? await _bandsDao.GetByUserIdAsync(userId)
            : await _bandsDao.GetAllAsync();
        return bands.Select(MapBand).ToList();
    }

    public async Task<BandResponse> GetBandAsync(string bandId)
    {
        var band = await _bandsDao.GetByIdAsync(bandId);
        if (band is null)
        {
            throw new NotFoundException("Band was not found.");
        }

        return MapBand(band);
    }

    public async Task<BandResponse> CreateBandAsync(CreateBandRequest request)
    {
        var name = request.Name?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException("Band name is required.");
        }

        if (name.Length > 100)
        {
            throw new ValidationException("Band name cannot exceed 100 characters.");
        }

        // TODO(authz): Add permission checks when authentication is introduced.
        var created = await _bandsDao.CreateAsync(name, "system");
        return MapBand(created);
    }

    public async Task<BandResponse> UpdateBandNameAsync(string bandId, UpdateBandNameRequest request)
    {
        var name = request.Name?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException("Band name is required.");
        }

        if (name.Length > 100)
        {
            throw new ValidationException("Band name cannot exceed 100 characters.");
        }

        var updated = await _bandsDao.UpdateNameAsync(bandId, name);
        if (updated is null)
        {
            throw new NotFoundException("Band was not found.");
        }

        return MapBand(updated);
    }

    public async Task<IReadOnlyCollection<BandMemberResponse>> GetMembersAsync(string requestingUserId, string bandId)
    {
        var band = await _bandsDao.GetByIdAsync(bandId);
        if (band is null) throw new NotFoundException("Band was not found.");

        var role = await _bandsDao.GetUserRoleInBandAsync(requestingUserId, bandId);
        if (role is null) throw new ForbiddenException("You are not a member of this band.");
        if (role != "RitualLeader") throw new ForbiddenException("Only a Ritual Leader can view band members.");

        var members = await _bandsDao.GetMembersAsync(bandId);
        return members.Select(m => new BandMemberResponse
        {
            BandMemberId = m.BandMemberId,
            UserId = m.UserId,
            DisplayName = m.DisplayName,
            Email = m.Email,
            Role = m.Role,
            Instrument = m.Instrument,
            JoinedAt = m.JoinedAt
        }).ToList();
    }

    public async Task<BandMemberResponse> AddMemberAsync(string requestingUserId, string bandId, AddBandMemberRequest request)
    {
        var band = await _bandsDao.GetByIdAsync(bandId);
        if (band is null) throw new NotFoundException("Band was not found.");

        var requesterRole = await _bandsDao.GetUserRoleInBandAsync(requestingUserId, bandId);
        if (requesterRole is null) throw new ForbiddenException("You are not a member of this band.");
        if (requesterRole != "RitualLeader") throw new ForbiddenException("Only a Ritual Leader can add members.");

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ValidationException("Email is required.");

        var allowedRoles = new[] { "RitualLeader", "BandMember" };
        if (!allowedRoles.Contains(request.Role))
            throw new ValidationException("Role must be RitualLeader or BandMember.");

        var targetUser = await _usersDao.GetByEmailAsync(request.Email.Trim().ToLowerInvariant());
        if (targetUser is null)
            throw new NotFoundException($"No user found with email '{request.Email}'.");

        var alreadyMember = await _bandsDao.IsMemberAsync(targetUser.UserId, bandId);
        if (alreadyMember)
            throw new ConflictException("This user is already a member of the band.");

        await _bandsDao.AddMemberAsync(bandId, targetUser.UserId, request.Role, request.Instrument);

        var members = await _bandsDao.GetMembersAsync(bandId);
        var added = members.First(m => m.UserId == targetUser.UserId);
        return new BandMemberResponse
        {
            BandMemberId = added.BandMemberId,
            UserId = added.UserId,
            DisplayName = added.DisplayName,
            Email = added.Email,
            Role = added.Role,
            Instrument = added.Instrument,
            JoinedAt = added.JoinedAt
        };
    }

    private static BandResponse MapBand(server.Dao.Entities.BandEntity band)
    {
        return new BandResponse
        {
            Id = band.Id,
            Name = band.Name,
            Description = band.Description,
            Country = band.Country,
            MusicBrainzArtistId = band.MusicBrainzArtistId,
            Role = band.UserRole
        };
    }
}
