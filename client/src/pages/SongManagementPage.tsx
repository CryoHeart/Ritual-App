import { useEffect, useState } from 'react';
import {
  createAlbum,
  deleteAlbum,
  getAlbumsWithSongs,
  updateAlbum
} from '../api/albumsApi';
import { createSong, deleteSong, updateSong } from '../api/songsApi';
import { AlbumSection } from '../components/SongManagement/AlbumSection';
import { AlbumFormModal } from '../components/SongManagement/AlbumFormModal';
import { SongFormModal } from '../components/SongManagement/SongFormModal';
import { ConfirmDialog } from '../components/ui/ConfirmDialog';
import { RitualButton } from '../components/ui/RitualButton';
import { RitualCard } from '../components/ui/RitualCard';
import type { Album, AlbumWithSongs, CreateAlbumRequest, UpdateAlbumRequest } from '../types/album';
import type { Song, CreateSongRequest, UpdateSongRequest } from '../types/song';

interface Props {
  bandId: string;
  bandName?: string;
  onBack: () => void;
}

export function SongManagementPage({ bandId, bandName, onBack }: Props) {
  const [albums, setAlbums] = useState<AlbumWithSongs[]>([]);
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState<string | null>(null);
  const [searchQuery, setSearchQuery] = useState('');
  const [isBusy, setIsBusy] = useState(false);

  // Album modal state
  const [albumModalOpen, setAlbumModalOpen] = useState(false);
  const [editingAlbum, setEditingAlbum] = useState<Album | null>(null);
  const [albumSubmitError, setAlbumSubmitError] = useState<string | null>(null);

  // Song modal state
  const [songModalOpen, setSongModalOpen] = useState(false);
  const [editingSong, setEditingSong] = useState<Song | null>(null);
  const [songSubmitError, setSongSubmitError] = useState<string | null>(null);

  // Confirm delete state
  const [confirmAlbum, setConfirmAlbum] = useState<AlbumWithSongs | null>(null);
  const [confirmSong, setConfirmSong] = useState<{ songId: string; title: string } | null>(null);
  const [isDeleting, setIsDeleting] = useState(false);

  useEffect(() => {
    load();
  }, [bandId]);

  const load = () => {
    setLoading(true);
    setLoadError(null);
    getAlbumsWithSongs(bandId)
      .then(setAlbums)
      .catch(e => setLoadError(e instanceof Error ? e.message : 'Failed to load songs.'))
      .finally(() => setLoading(false));
  };

  const refresh = async () => {
    const updated = await getAlbumsWithSongs(bandId);
    setAlbums(updated);
  };

  // ----- Album handlers -----

  const handleOpenNewAlbum = () => {
    setEditingAlbum(null);
    setAlbumSubmitError(null);
    setAlbumModalOpen(true);
  };

  const handleOpenEditAlbum = (album: AlbumWithSongs) => {
    setEditingAlbum(album as Album);
    setAlbumSubmitError(null);
    setAlbumModalOpen(true);
  };

  const handleSubmitAlbum = async (payload: CreateAlbumRequest | UpdateAlbumRequest) => {
    setIsBusy(true);
    setAlbumSubmitError(null);
    try {
      if (editingAlbum?.albumId) {
        await updateAlbum(bandId, editingAlbum.albumId, payload as UpdateAlbumRequest);
      } else {
        await createAlbum(bandId, payload as CreateAlbumRequest);
      }
      await refresh();
      setAlbumModalOpen(false);
    } catch (e) {
      setAlbumSubmitError(e instanceof Error ? e.message : 'Failed to save album.');
    } finally {
      setIsBusy(false);
    }
  };

  const handleConfirmDeleteAlbum = async () => {
    if (!confirmAlbum?.albumId) return;
    setIsDeleting(true);
    try {
      await deleteAlbum(bandId, confirmAlbum.albumId);
      await refresh();
      setConfirmAlbum(null);
    } catch {
      // Leave dialog open on error — user can retry
    } finally {
      setIsDeleting(false);
    }
  };

  // ----- Song handlers -----

  const handleOpenNewSong = () => {
    setEditingSong(null);
    setSongSubmitError(null);
    setSongModalOpen(true);
  };

  const handleOpenEditSong = (songId: string) => {
    for (const album of albums) {
      const match = album.songs.find(s => s.songId === songId);
      if (match) {
        setEditingSong({
          songId: match.songId,
          bandId: match.bandId,
          title: match.title,
          bpm: match.bpm,
          durationSeconds: match.durationSeconds,
          albumId: match.albumId ?? undefined,
          albumTitle: match.albumTitle ?? undefined,
          albumTrackNumber: match.albumTrackNumber,
          tuning: match.tuning,
          songKey: match.songKey,
          notes: match.notes
        });
        setSongSubmitError(null);
        setSongModalOpen(true);
        return;
      }
    }
  };

  const handleSubmitSong = async (payload: CreateSongRequest | UpdateSongRequest) => {
    setIsBusy(true);
    setSongSubmitError(null);
    try {
      if (editingSong) {
        await updateSong(bandId, editingSong.songId, payload as UpdateSongRequest);
      } else {
        await createSong(bandId, payload as CreateSongRequest);
      }
      await refresh();
      setSongModalOpen(false);
    } catch (e) {
      setSongSubmitError(e instanceof Error ? e.message : 'Failed to save song.');
    } finally {
      setIsBusy(false);
    }
  };

  const handleConfirmDeleteSong = async () => {
    if (!confirmSong) return;
    setIsDeleting(true);
    try {
      await deleteSong(bandId, confirmSong.songId);
      await refresh();
      setConfirmSong(null);
    } catch {
      // Leave dialog open on error — user can retry
    } finally {
      setIsDeleting(false);
    }
  };

  const totalSongs = albums.reduce((sum, a) => sum + a.songs.length, 0);
  const realAlbums = albums.filter(a => a.albumId !== null);

  // ---- Render ----

  return (
    <div className="flex min-h-screen flex-col bg-zinc-950">
      {/* Header */}
      <header className="flex items-center justify-between border-b border-zinc-800/60 bg-zinc-900/60 px-6 py-4 backdrop-blur-sm">
        <div className="flex items-center gap-4">
          <button
            onClick={onBack}
            className="flex items-center gap-2 text-xs font-semibold uppercase tracking-[0.2em] text-zinc-400 transition-colors hover:text-zinc-100"
          >
            ← Back
          </button>
          <div className="h-4 w-px bg-zinc-700" />
          <div>
            <p className="text-[10px] font-semibold uppercase tracking-[0.36em] text-red-500">
              {bandName ?? 'Band'}
            </p>
            <h1 className="text-lg font-semibold tracking-tight text-zinc-100">Song Management</h1>
          </div>
        </div>

        <div className="flex items-center gap-2">
          <RitualButton variant="neutral" size="sm" onClick={handleOpenNewAlbum}>
            + New Album
          </RitualButton>
          <RitualButton variant="primary" size="sm" onClick={handleOpenNewSong}>
            + New Song
          </RitualButton>
        </div>
      </header>

      {/* Stats strip */}
      <div className="flex items-center gap-6 border-b border-zinc-800/40 bg-zinc-900/30 px-6 py-2">
        <span className="text-xs text-zinc-500">
          <span className="font-semibold text-zinc-300">{realAlbums.length}</span> albums
        </span>
        <span className="text-xs text-zinc-500">
          <span className="font-semibold text-zinc-300">{totalSongs}</span> songs
        </span>
      </div>

      {/* Main content */}
      <div className="flex-1 overflow-y-auto px-6 py-6">
        {loading ? (
          <div className="flex items-center justify-center py-24">
            <RitualCard className="w-full max-w-sm text-center">
              <p className="text-xs uppercase tracking-[0.32em] text-zinc-500">Loading</p>
              <p className="mt-3 text-lg font-semibold text-zinc-200">Loading songs...</p>
              <div className="mx-auto mt-5 h-1.5 w-32 overflow-hidden rounded-full bg-zinc-800">
                <div className="ritual-pulse h-full w-1/2 rounded-full bg-red-600" />
              </div>
            </RitualCard>
          </div>
        ) : loadError ? (
          <div className="flex items-center justify-center py-24">
            <RitualCard className="w-full max-w-sm border-red-900/60 text-center">
              <p className="text-sm text-red-300">{loadError}</p>
              <RitualButton variant="danger" size="sm" className="mt-4" onClick={load}>
                Retry
              </RitualButton>
            </RitualCard>
          </div>
        ) : (
          <div className="mx-auto max-w-4xl space-y-4">
            {/* Search */}
            <div className="relative">
              <input
                value={searchQuery}
                onChange={e => setSearchQuery(e.target.value)}
                placeholder="Search songs..."
                className="w-full rounded-xl border border-zinc-700/60 bg-zinc-900/60 px-4 py-2.5 text-sm text-zinc-100 outline-none transition-colors placeholder:text-zinc-600 focus:border-red-600/60"
              />
              {searchQuery && (
                <button
                  onClick={() => setSearchQuery('')}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-xs text-zinc-500 hover:text-zinc-300"
                >
                  ✕
                </button>
              )}
            </div>

            {albums.length === 0 ? (
              <RitualCard className="text-center py-12">
                <p className="text-xs uppercase tracking-[0.28em] text-zinc-600">Empty</p>
                <p className="mt-2 text-sm text-zinc-500">No albums or songs yet.</p>
                <p className="mt-1 text-xs text-zinc-600">Create an album or add songs to get started.</p>
                <div className="mt-5 flex justify-center gap-2">
                  <RitualButton variant="neutral" size="sm" onClick={handleOpenNewAlbum}>+ New Album</RitualButton>
                  <RitualButton variant="primary" size="sm" onClick={handleOpenNewSong}>+ New Song</RitualButton>
                </div>
              </RitualCard>
            ) : (
              <>
                {albums.map(album => (
                  <AlbumSection
                    key={album.albumId ?? 'unassigned'}
                    album={album}
                    searchQuery={searchQuery}
                    onEditAlbum={album.albumId ? () => handleOpenEditAlbum(album) : undefined}
                    onDeleteAlbum={album.albumId ? () => setConfirmAlbum(album) : undefined}
                    onEditSong={handleOpenEditSong}
                    onDeleteSong={songId => {
                      const song = album.songs.find(s => s.songId === songId);
                      if (song) setConfirmSong({ songId: song.songId, title: song.title });
                    }}
                  />
                ))}

                {searchQuery && albums.every(a => {
                  const q = searchQuery.trim().toLowerCase();
                  return a.songs.filter(s => s.title.toLowerCase().includes(q)).length === 0;
                }) && (
                  <RitualCard className="text-center py-10">
                    <p className="text-xs uppercase tracking-[0.28em] text-zinc-600">No Results</p>
                    <p className="mt-2 text-sm text-zinc-500">No songs match "{searchQuery}"</p>
                  </RitualCard>
                )}
              </>
            )}
          </div>
        )}
      </div>

      {/* Modals */}
      <AlbumFormModal
        isOpen={albumModalOpen}
        album={editingAlbum}
        isSubmitting={isBusy}
        submitError={albumSubmitError}
        onClose={() => setAlbumModalOpen(false)}
        onSubmit={handleSubmitAlbum}
      />

      <SongFormModal
        isOpen={songModalOpen}
        song={editingSong}
        albums={albums}
        isSubmitting={isBusy}
        submitError={songSubmitError}
        onClose={() => setSongModalOpen(false)}
        onSubmit={handleSubmitSong}
      />

      <ConfirmDialog
        isOpen={confirmAlbum !== null}
        title="Delete Album"
        message={`Delete album "${confirmAlbum?.title}"? Songs will not be deleted; they will become Unassigned.`}
        isConfirming={isDeleting}
        onCancel={() => setConfirmAlbum(null)}
        onConfirm={handleConfirmDeleteAlbum}
      />

      <ConfirmDialog
        isOpen={confirmSong !== null}
        title="Delete Song"
        message={`Delete song "${confirmSong?.title}"? This will also remove it from any setlists.`}
        isConfirming={isDeleting}
        onCancel={() => setConfirmSong(null)}
        onConfirm={handleConfirmDeleteSong}
      />
    </div>
  );
}
