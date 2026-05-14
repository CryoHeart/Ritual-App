using server.Models.Requests;
using server.Models.Responses;

namespace server.Logic.Interfaces;

public interface IBandsLogic
{
    Task<IReadOnlyCollection<BandResponse>> GetBandsAsync(string? userId = null);

    Task<BandResponse> GetBandAsync(string bandId);

    Task<BandResponse> CreateBandAsync(CreateBandRequest request);

    Task<BandResponse> UpdateBandNameAsync(string bandId, UpdateBandNameRequest request);

    Task<IReadOnlyCollection<BandMemberResponse>> GetMembersAsync(string requestingUserId, string bandId);

    Task<BandMemberResponse> AddMemberAsync(string requestingUserId, string bandId, AddBandMemberRequest request);
}
