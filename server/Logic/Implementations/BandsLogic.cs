using server.Dao.Interfaces;
using server.Logic.Exceptions;
using server.Logic.Interfaces;
using server.Models.Requests;
using server.Models.Responses;

namespace server.Logic.Implementations;

public class BandsLogic : IBandsLogic
{
    private readonly IBandsDao _bandsDao;

    public BandsLogic(IBandsDao bandsDao)
    {
        _bandsDao = bandsDao;
    }

    public async Task<IReadOnlyCollection<BandResponse>> GetBandsAsync()
    {
        var bands = await _bandsDao.GetAllAsync();
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

    private static BandResponse MapBand(server.Dao.Entities.BandEntity band)
    {
        return new BandResponse
        {
            Id = band.Id,
            Name = band.Name,
            Description = band.Description
        };
    }
}
 