import { useEffect, useState } from 'react';
import {
  createAlbum,
  deleteAlbum,
  getAlbumsWithSongs,
  updateAlbum
} from '../api/albumsApi';
import loginBackground from '../assets/login-background.png';
import { AppShell } from '../components/AppShell';
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
    <AppShell
      selectedBandName={bandName}
      backgroundImageSrc={loginBackground}
      centerSlot={<h1 className="text-xl font-bold uppercase tracking-[0.2em] text-zinc-100">Song management</h1>}
      onLogoClick={onBack}
    >
      <div className="mx-auto flex h-full w-full max-w-5xl flex-col px-8 py-6">
        <div className="mb-5 flex justify-end">
          <button
            onClick={onBack}
            className="rounded-lg border border-zinc-700 bg-zinc-900/70 px-4 py-2 text-xs font-semibold uppercase tracking-[0.2em] text-zinc-200 transition hover:border-zinc-500 hover:text-white"
          >
            Back
          </button>
        </div>

        <div className="mb-5 rounded-xl border border-zinc-800/50 bg-zinc-900/35 px-5 py-3">
          <div className="flex w-full items-center gap-8">
          <span className="text-sm text-zinc-500">
            <span className="font-semibold text-zinc-300">{realAlbums.length}</span> albums
          </span>
          <span className="text-sm text-zinc-500">
            <span className="font-semibold text-zinc-300">{totalSongs}</span> songs
          </span>
        </div>
      </div>

        <div className="min-h-0 flex-1 overflow-y-auto">
        {loading ? (
          <div className="flex items-center justify-center py-24">
            <RitualCard className="w-full max-w-md text-center">
              <p className="text-sm uppercase tracking-[0.32em] text-zinc-500">Loading</p>
              <p className="mt-3 text-xl font-semibold text-zinc-200">Loading songs...</p>
              <div className="mx-auto mt-5 h-1.5 w-32 overflow-hidden rounded-full bg-zinc-800">
                <div className="ritual-pulse h-full w-1/2 rounded-full bg-red-600" />
              </div>
            </RitualCard>
          </div>
        ) : loadError ? (
          <div className="flex items-center justify-center py-24">
            <RitualCard className="w-full max-w-md border-red-900/60 text-center">
              <p className="text-base text-red-300">{loadError}</p>
              <RitualButton variant="danger" size="sm" className="mt-4" onClick={load}>
                Retry
              </RitualButton>
            </RitualCard>
          </div>
        ) : (
          <div className="max-w-5xl space-y-5">
            {/* Search */}
            <div className="relative">
              <input
                value={searchQuery}
                onChange={e => setSearchQuery(e.target.value)}
                placeholder="Search songs..."
                className="w-full rounded-2xl border border-zinc-700/60 bg-zinc-900/60 px-5 py-3.5 text-base text-zinc-100 outline-none transition-colors placeholder:text-zinc-600 focus:border-red-600/60"
              />
              {searchQuery && (
                <button
                  onClick={() => setSearchQuery('')}
                  className="absolute right-4 top-1/2 -translate-y-1/2 text-sm text-zinc-500 hover:text-zinc-300"
                >
                  ✕
                </button>
              )}
            </div>

            <div className="grid w-full grid-cols-2 gap-3">
              <RitualButton variant="neutral" size="md" className="w-full" onClick={handleOpenNewAlbum}>
                + New Album
              </RitualButton>
              <RitualButton variant="primary" size="md" className="w-full" onClick={handleOpenNewSong}>
                + New Song
              </RitualButton>
            </div>

            {albums.length === 0 ? (
              <RitualCard className="text-center py-14">
                <p className="text-sm uppercase tracking-[0.28em] text-zinc-600">Empty</p>
                <p className="mt-2 text-base text-zinc-500">No albums or songs yet.</p>
                <p className="mt-1 text-sm text-zinc-600">Create an album or add songs to get started.</p>
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
                    <p className="text-sm uppercase tracking-[0.28em] text-zinc-600">No Results</p>
                    <p className="mt-2 text-base text-zinc-500">No songs match "{searchQuery}"</p>
                  </RitualCard>
                )}
              </>
            )}
          </div>
        )}
      </div>

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
    </AppShell>
  );
}
