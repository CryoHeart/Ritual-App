using server.Dao.Entities;
using server.Dao.Interfaces;
using server.Logic.Exceptions;
using server.Logic.Interfaces;
using server.Models.Requests;
using server.Models.Responses;

namespace server.Logic.Implementations;

public class LiveSessionsLogic : ILiveSessionsLogic
{
    private readonly IBandsDao _bandsDao;
    private readonly ISongsDao _songsDao;
    private readonly ISetlistsDao _setlistsDao;
    private readonly ILiveSessionsDao _liveSessionsDao;

    public LiveSessionsLogic(
        IBandsDao bandsDao,
        ISongsDao songsDao,
        ISetlistsDao setlistsDao,
        ILiveSessionsDao liveSessionsDao)
    {
        _bandsDao = bandsDao;
        _songsDao = songsDao;
        _setlistsDao = setlistsDao;
        _liveSessionsDao = liveSessionsDao;
    }

    public async Task<LiveSessionResponse> StartSessionAsync(string bandId, string setlistId, StartLiveSessionRequest request)
    {
        await EnsureBandExistsAsync(bandId);

        var setlist = await GetSetlistAndEnsureBandAsync(setlistId, bandId);
        if (setlist.TotalSongs == 0)
        {
            throw new ConflictException("Cannot start live session for an empty setlist.");
        }

        var session = await _liveSessionsDao.CreateAsync(bandId, setlistId);
        return await MapSessionAsync(session, setlist);
    }

    public async Task<LiveSessionResponse> GetSessionAsync(string bandId, string sessionId)
    {
        await EnsureBandExistsAsync(bandId);
        var session = await GetSessionAndEnsureBandAsync(sessionId, bandId);
        var setlist = await GetSetlistAndEnsureBandAsync(session.SetlistId, bandId);
        return await MapSessionAsync(session, setlist);
    }

    public async Task<LiveSessionResponse> NextSongAsync(string bandId, string sessionId)
    {
        await EnsureBandExistsAsync(bandId);
        var session = await GetSessionAndEnsureBandAsync(sessionId, bandId);
        if (session.IsEnded)
        {
            throw new ConflictException("Cannot advance an ended session.");
        }

        var setlist = await GetSetlistAndEnsureBandAsync(session.SetlistId, bandId);
        var totalSongs = setlist.TotalSongs;
        if (totalSongs == 0)
        {
            throw new ConflictException("Cannot advance session with an empty setlist.");
        }

        if (session.CurrentSongIndex >= totalSongs - 1)
        {
            throw new ConflictException("Cannot go past the final song.");
        }

        session.CurrentSongIndex++;
        await _liveSessionsDao.UpdateAsync(session);
        return await MapSessionAsync(session, setlist);
    }

    public async Task<LiveSessionResponse> PreviousSongAsync(string bandId, string sessionId)
    {
        await EnsureBandExistsAsync(bandId);
        var session = await GetSessionAndEnsureBandAsync(sessionId, bandId);
        if (session.IsEnded)
        {
            throw new ConflictException("Cannot go back on an ended session.");
        }

        var setlist = await GetSetlistAndEnsureBandAsync(session.SetlistId, bandId);
        if (session.CurrentSongIndex <= 0)
        {
            throw new ConflictException("Cannot go before the first song.");
        }

        session.CurrentSongIndex--;
        await _liveSessionsDao.UpdateAsync(session);
        return await MapSessionAsync(session, setlist);
    }

    public async Task<LiveSessionResponse> EndSessionAsync(string bandId, string sessionId)
    {
        await EnsureBandExistsAsync(bandId);
        var session = await GetSessionAndEnsureBandAsync(sessionId, bandId);
        if (session.IsEnded)
        {
            throw new ConflictException("Session is already ended.");
        }

        var setlist = await GetSetlistAndEnsureBandAsync(session.SetlistId, bandId);

        session.Status = "ended";
        session.EndedAt = DateTime.UtcNow;
        await _liveSessionsDao.UpdateAsync(session);
        return await MapSessionAsync(session, setlist);
    }

    private async Task EnsureBandExistsAsync(string bandId)
    {
        if (await _bandsDao.GetByIdAsync(bandId) is null)
        {
            throw new NotFoundException("Band was not found.");
        }
    }

    private async Task<LiveSessionEntity> GetSessionAndEnsureBandAsync(string sessionId, string bandId)
    {
        var session = await _liveSessionsDao.GetByIdAsync(sessionId);
        if (session is null)
        {
            throw new NotFoundException("Live session was not found.");
        }

        if (session.BandId != bandId)
        {
            throw new NotFoundException("Live session was not found for this band.");
        }

        return session;
    }

    private async Task<SetlistEntity> GetSetlistAndEnsureBandAsync(string setlistId, string bandId)
    {
        var setlist = await _setlistsDao.GetByIdWithSongsAsync(setlistId);
        if (setlist is null)
        {
            throw new NotFoundException("Setlist was not found.");
        }

        if (setlist.BandId != bandId)
        {
            throw new ConflictException("Setlist does not belong to the provided band.");
        }

        return setlist;
    }

    private async Task<LiveSessionResponse> MapSessionAsync(LiveSessionEntity session, SetlistEntity setlist)
    {
        string? currentSongId = null;
        string? currentSongTitle = null;

        if (setlist.TotalSongs > 0 && session.CurrentSongIndex >= 0 && session.CurrentSongIndex < setlist.TotalSongs)
        {
            var songId = setlist.Songs[session.CurrentSongIndex].SongId;
            currentSongId = songId;
            var song = await _songsDao.GetByIdAsync(songId);
            currentSongTitle = song?.Title;
        }

        return new LiveSessionResponse
        {
            Id = session.Id,
            BandId = session.BandId,
            SetlistId = session.SetlistId,
            Status = session.Status,
            IsEnded = session.IsEnded,
            CurrentSongIndex = session.CurrentSongIndex,
            TotalSongs = setlist.TotalSongs,
            CurrentSongId = currentSongId,
            CurrentSongTitle = currentSongTitle
        };
    }
}
