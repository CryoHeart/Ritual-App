using Microsoft.EntityFrameworkCore;
using server.Data.Entities;

namespace server.Data;

public class RitualDbContext : DbContext
{
    public RitualDbContext(DbContextOptions<RitualDbContext> options) : base(options)
    {
    }

    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<BandEntity> Bands => Set<BandEntity>();
    public DbSet<BandMemberEntity> BandMembers => Set<BandMemberEntity>();
    public DbSet<AlbumEntity> Albums => Set<AlbumEntity>();
    public DbSet<SongEntity> Songs => Set<SongEntity>();
    public DbSet<SetlistEntity> Setlists => Set<SetlistEntity>();
    public DbSet<SetlistSongEntity> SetlistSongs => Set<SetlistSongEntity>();
    public DbSet<LiveSessionEntity> LiveSessions => Set<LiveSessionEntity>();
    public DbSet<SongCueEntity> SongCues => Set<SongCueEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.DisplayName).HasColumnName("display_name");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<BandEntity>(entity =>
        {
            entity.ToTable("bands");
            entity.HasKey(e => e.BandId);
            entity.Property(e => e.BandId).HasColumnName("band_id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Country).HasColumnName("country");
            entity.Property(e => e.MusicBrainzArtistId).HasColumnName("musicbrainz_artist_id");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<BandMemberEntity>(entity =>
        {
            entity.ToTable("band_members");
            entity.HasKey(e => e.BandMemberId);
            entity.Property(e => e.BandMemberId).HasColumnName("band_member_id");
            entity.Property(e => e.BandId).HasColumnName("band_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.Instrument).HasColumnName("instrument");
            entity.Property(e => e.JoinedAt).HasColumnName("joined_at");
        });

        modelBuilder.Entity<SongEntity>(entity =>
        {
            entity.ToTable("songs");
            entity.HasKey(e => e.SongId);
            entity.Property(e => e.SongId).HasColumnName("song_id");
            entity.Property(e => e.BandId).HasColumnName("band_id");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.MusicBrainzRecordingId).HasColumnName("musicbrainz_recording_id");
            entity.Property(e => e.Bpm).HasColumnName("bpm");
            entity.Property(e => e.DurationSeconds).HasColumnName("duration_seconds");
            entity.Property(e => e.Tuning).HasColumnName("tuning");
            entity.Property(e => e.SongKey).HasColumnName("song_key");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.AlbumId).HasColumnName("album_id");
            entity.Property(e => e.AlbumTrackNumber).HasColumnName("album_track_number");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<AlbumEntity>(entity =>
        {
            entity.ToTable("albums");
            entity.HasKey(e => e.AlbumId);
            entity.Property(e => e.AlbumId).HasColumnName("album_id");
            entity.Property(e => e.BandId).HasColumnName("band_id");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.MusicBrainzReleaseGroupId).HasColumnName("musicbrainz_release_group_id");
            entity.Property(e => e.MusicBrainzReleaseId).HasColumnName("musicbrainz_release_id");
            entity.Property(e => e.ReleaseYear).HasColumnName("release_year");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<SetlistEntity>(entity =>
        {
            entity.ToTable("setlists");
            entity.HasKey(e => e.SetlistId);
            entity.Property(e => e.SetlistId).HasColumnName("setlist_id");
            entity.Property(e => e.BandId).HasColumnName("band_id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<SetlistSongEntity>(entity =>
        {
            entity.ToTable("setlist_songs");
            entity.HasKey(e => e.SetlistSongId);
            entity.Property(e => e.SetlistSongId).HasColumnName("setlist_song_id");
            entity.Property(e => e.SetlistId).HasColumnName("setlist_id");
            entity.Property(e => e.SongId).HasColumnName("song_id");
            entity.Property(e => e.PositionIndex).HasColumnName("position_index");
            entity.Property(e => e.TransitionNotes).HasColumnName("transition_notes");
            entity.Property(e => e.PerformanceNotes).HasColumnName("performance_notes");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });

        modelBuilder.Entity<LiveSessionEntity>(entity =>
        {
            entity.ToTable("live_sessions");
            entity.HasKey(e => e.LiveSessionId);
            entity.Property(e => e.LiveSessionId).HasColumnName("live_session_id");
            entity.Property(e => e.BandId).HasColumnName("band_id");
            entity.Property(e => e.SetlistId).HasColumnName("setlist_id");
            entity.Property(e => e.StartedByUserId).HasColumnName("started_by_user_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.CurrentPositionIndex).HasColumnName("current_position_index");
            entity.Property(e => e.StartedAt).HasColumnName("started_at");
            entity.Property(e => e.EndedAt).HasColumnName("ended_at");
        });

        modelBuilder.Entity<SongCueEntity>(entity =>
        {
            entity.ToTable("song_cues");
            entity.HasKey(e => e.SongCueId);
            entity.Property(e => e.SongCueId).HasColumnName("song_cue_id");
            entity.Property(e => e.SongId).HasColumnName("song_id");
            entity.Property(e => e.CueType).HasColumnName("cue_type");
            entity.Property(e => e.CueText).HasColumnName("cue_text");
            entity.Property(e => e.CueTimeSeconds).HasColumnName("cue_time_seconds");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });
    }
}
