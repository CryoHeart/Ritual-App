using Microsoft.AspNetCore.SignalR;
using server.Dao.Entities;
using server.Dao.Interfaces;
using server.Hubs;
using server.Logic.Exceptions;
using server.Logic.Interfaces;
using server.Models.Responses;

namespace server.Logic.Implementations;

public class LiveSessionsLogic : ILiveSessionsLogic
{
    private readonly IBandsDao _bandsDao;
    private readonly ISetlistsDao _setlistsDao;
    private readonly ILiveSessionsDao _liveSessionsDao;
    private readonly IPermissionsLogic _permissions;
    private readonly IHubContext<LiveRitualHub> _hub;

    public LiveSessionsLogic(
        IBandsDao bandsDao,
        ISetlistsDao setlistsDao,
        ILiveSessionsDao liveSessionsDao,
        IPermissionsLogic permissions,
        IHubContext<LiveRitualHub> hub)
    {
        _bandsDao = bandsDao;
        _setlistsDao = setlistsDao;
        _liveSessionsDao = liveSessionsDao;
        _permissions = permissions;
        _hub = hub;
    }

    public async Task<LiveSessionResponse> StartSessionAsync(string userId, string bandId, string setlistId)
    {
        await EnsureBandExistsAsync(bandId);
        await _permissions.EnsureRitualLeaderAsync(userId, bandId);

        var existing = await _liveSessionsDao.GetActiveByBandIdAsync(bandId);
        if (existing != null)
            throw new ConflictException("A live session is already active for this band.");

        var setlist = await GetSetlistAndEnsureBandAsync(setlistId, bandId);
        if (setlist.TotalSongs == 0)
            throw new ConflictException("Cannot start a live session for an empty setlist.");

        var session = await _liveSessionsDao.CreateAsync(bandId, setlistId, userId);
        var response = await BuildResponseAsync(session, setlist, userId);

        await _hub.Clients.Group($"band:{bandId}").SendAsync("RitualStarted", response);
        return response;
    }

    public async Task<LiveSessionResponse?> GetActiveSessionAsync(string userId, string bandId)
    {
        await EnsureBandExistsAsync(bandId);
        await _permissions.EnsureBandMemberAsync(userId, bandId);

        var session = await _liveSessionsDao.GetActiveByBandIdAsync(bandId);
        if (session == null) return null;
        var setlist = await GetSetlistAndEnsureBandAsync(session.SetlistId, bandId);
        return await BuildResponseAsync(session, setlist, userId);
    }

    public async Task<LiveSessionResponse> GetSessionAsync(string userId, string bandId, string sessionId)
    {
        await EnsureBandExistsAsync(bandId);
        await _permissions.EnsureBandMemberAsync(userId, bandId);

        var session = await GetSessionAndEnsureBandAsync(sessionId, bandId);
        var setlist = await GetSetlistAndEnsureBandAsync(session.SetlistId, bandId);
        return await BuildResponseAsync(session, setlist, userId);
    }

    public async Task<LiveSessionResponse> NextSongAsync(string userId, string bandId, string sessionId)
    {
        await EnsureBandExistsAsync(bandId);
        await _permissions.EnsureRitualLeaderAsync(userId, bandId);

        var session = await GetSessionAndEnsureBandAsync(sessionId, bandId);
        if (session.IsEnded)
            throw new ConflictException("Cannot advance an ended session.");

        var setlist = await GetSetlistAndEnsureBandAsync(session.SetlistId, bandId);
        if (setlist.TotalSongs == 0)
            throw new ConflictException("Cannot advance session with an empty setlist.");
        if (session.CurrentSongIndex >= setlist.TotalSongs - 1)
            throw new ConflictException("Cannot go past the final song.");

        session.CurrentSongIndex++;
        await _liveSessionsDao.UpdateAsync(session);
        var response = await BuildResponseAsync(session, setlist, userId);

        await _hub.Clients.Group($"live-session:{sessionId}").SendAsync("RitualUpdated", response);
        await _hub.Clients.Group($"band:{bandId}").SendAsync("RitualUpdated", response);
        return response;
    }

    public async Task<LiveSessionResponse> PreviousSongAsync(string userId, string bandId, string sessionId)
    {
        await EnsureBandExistsAsync(bandId);
        await _permissions.EnsureRitualLeaderAsync(userId, bandId);

        var session = await GetSessionAndEnsureBandAsync(sessionId, bandId);
        if (session.IsEnded)
            throw new ConflictException("Cannot go back on an ended session.");
        if (session.CurrentSongIndex <= 0)
            throw new ConflictException("Cannot go before the first song.");

        var setlist = await GetSetlistAndEnsureBandAsync(session.SetlistId, bandId);
        session.CurrentSongIndex--;
        await _liveSessionsDao.UpdateAsync(session);
        var response = await BuildResponseAsync(session, setlist, userId);

        await _hub.Clients.Group($"live-session:{sessionId}").SendAsync("RitualUpdated", response);
        await _hub.Clients.Group($"band:{bandId}").SendAsync("RitualUpdated", response);
        return response;
    }

    public async Task<LiveSessionResponse> EndSessionAsync(string userId, string bandId, string sessionId)
    {
        await EnsureBandExistsAsync(bandId);
        await _permissions.EnsureRitualLeaderAsync(userId, bandId);

        var session = await GetSessionAndEnsureBandAsync(sessionId, bandId);
        if (session.IsEnded)
            throw new ConflictException("Session is already ended.");

        var setlist = await GetSetlistAndEnsureBandAsync(session.SetlistId, bandId);
        session.Status = "ended";
        session.EndedAt = DateTime.UtcNow;
        await _liveSessionsDao.UpdateAsync(session);
        var response = await BuildResponseAsync(session, setlist, userId);

        await _hub.Clients.Group($"live-session:{sessionId}").SendAsync("RitualEnded", response);
        await _hub.Clients.Group($"band:{bandId}").SendAsync("RitualEnded", response);
        return response;
    }

    // ─────────────────────────────────────────────
    //  Private helpers
    // ─────────────────────────────────────────────

    private async Task EnsureBandExistsAsync(string bandId)
    {
        if (await _bandsDao.GetByIdAsync(bandId) is null)
            throw new NotFoundException("Band was not found.");
    }

    private async Task<LiveSessionEntity> GetSessionAndEnsureBandAsync(string sessionId, string bandId)
    {
        var session = await _liveSessionsDao.GetByIdAsync(sessionId);
        if (session is null)
            throw new NotFoundException("Live session was not found.");
        if (session.BandId != bandId)
            throw new NotFoundException("Live session was not found for this band.");
        return session;
    }

    private async Task<SetlistEntity> GetSetlistAndEnsureBandAsync(string setlistId, string bandId)
    {
        var setlist = await _setlistsDao.GetByIdWithSongsAsync(setlistId);
        if (setlist is null)
            throw new NotFoundException("Setlist was not found.");
        if (setlist.BandId != bandId)
            throw new ConflictException("Setlist does not belong to the provided band.");
        return setlist;
    }

    private async Task<LiveSessionResponse> BuildResponseAsync(LiveSessionEntity session, SetlistEntity setlist, string requestingUserId)
    {
        var canControl = await _permissions.CanControlLiveRitualAsync(requestingUserId, session.BandId);

        LiveSessionSongResponse? currentSong = null;
        LiveSessionSongResponse? nextSong = null;

        if (setlist.Songs != null && setlist.TotalSongs > 0)
        {
            var idx = session.CurrentSongIndex;
            if (idx >= 0 && idx < setlist.Songs.Count)
                currentSong = MapSetlistSong(setlist.Songs[idx], idx);
            var nextIdx = idx + 1;
            if (nextIdx < setlist.Songs.Count)
                nextSong = MapSetlistSong(setlist.Songs[nextIdx], nextIdx);
        }

        return new LiveSessionResponse
        {
            LiveSessionId = session.Id,
            BandId = session.BandId,
            SetlistId = session.SetlistId,
            SetlistName = setlist.Name,
            Status = session.Status,
            CurrentPositionIndex = session.CurrentSongIndex,
            StartedAt = session.StartedAt,
            EndedAt = session.EndedAt,
            StartedByUserId = session.StartedByUserId,
            StartedByDisplayName = session.StartedByDisplayName,
            CurrentSong = currentSong,
            NextSong = nextSong,
            TotalSongs = setlist.TotalSongs,
            TotalDurationSeconds = setlist.Songs?.Sum(s => s.DurationSeconds ?? 0) ?? 0,
            CanControl = canControl
        };
    }

    private static LiveSessionSongResponse MapSetlistSong(SetlistSongEntity s, int index) => new LiveSessionSongResponse
    {
        SetlistSongId = s.SetlistSongId,
        SongId = s.SongId,
        Title = s.Title,
        AlbumTitle = s.AlbumTitle,
        AlbumTrackNumber = s.AlbumTrackNumber,
        DurationSeconds = s.DurationSeconds,
        Bpm = s.Bpm,
        Tuning = s.Tuning,
        SongKey = s.SongKey,
        Notes = s.Notes,
        TransitionNotes = s.TransitionNotes,
        PerformanceNotes = s.PerformanceNotes,
        PositionIndex = index
    };
}
