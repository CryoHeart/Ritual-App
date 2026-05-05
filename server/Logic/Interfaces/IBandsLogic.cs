using server.Models.Requests;
using server.Models.Responses;

namespace server.Logic.Interfaces;

public interface IBandsLogic
{
    Task<IReadOnlyCollection<BandResponse>> GetBandsAsync(string? userId = null);

    Task<BandResponse> GetBandAsync(string bandId);

    Task<BandResponse> CreateBandAsync(CreateBandRequest request);
}
