using Microsoft.EntityFrameworkCore;
using server.Dao.Interfaces;
using server.Data;
using server.Models.Export;

namespace server.Dao.Implementations;

public class SetlistExportDao : ISetlistExportDao
{
    private readonly RitualDbContext _db;

    public SetlistExportDao(RitualDbContext db)
    {
        _db = db;
    }

    public async Task<SetlistExportData?> GetSetlistExportDataAsync(string bandId, string setlistId)
    {
        var band = await _db.Bands.FirstOrDefaultAsync(b => b.BandId == bandId);
        if (band is null) return null;

        var setlist = await _db.Setlists
            .FirstOrDefaultAsync(s => s.SetlistId == setlistId && s.BandId == bandId);
        if (setlist is null) return null;

        var songs = await (
            from ss in _db.SetlistSongs
            join s in _db.Songs on ss.SongId equals s.SongId
            join a in _db.Albums on s.AlbumId equals a.AlbumId into albumJoin
            from a in albumJoin.DefaultIfEmpty()
            where ss.SetlistId == setlistId
            orderby ss.PositionIndex
            select new SetlistExportSong
            {
                PositionIndex = ss.PositionIndex,
                SongId = s.SongId,
                Title = s.Title,
                AlbumTitle = a != null ? a.Title : null,
                AlbumTrackNumber = s.AlbumTrackNumber,
                DurationSeconds = s.DurationSeconds,
                Bpm = s.Bpm,
                Tuning = s.Tuning,
                SongKey = s.SongKey,
                Notes = s.Notes,
                TransitionNotes = ss.TransitionNotes,
                PerformanceNotes = ss.PerformanceNotes
            }
        ).ToListAsync();

        return new SetlistExportData
        {
            BandId = band.BandId,
            BandName = band.Name,
            SetlistId = setlist.SetlistId,
            SetlistName = setlist.Name,
            SetlistDescription = setlist.Description,
            TotalSongs = songs.Count,
            TotalDurationSeconds = songs.Sum(s => s.DurationSeconds ?? 0),
            ExportedAt = DateTime.UtcNow,
            Songs = songs
        };
    }
}
