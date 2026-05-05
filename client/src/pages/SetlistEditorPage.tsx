import { useEffect, useState } from 'react';
import { getAlbumsWithSongs } from '../api/albumsApi';
import {
  addSongToSetlist,
  getSetlistDetails,
  removeSongFromSetlist,
  reorderSetlistSongs
} from '../api/setlistsApi';
import { AlbumSongLibrary } from '../components/SetlistEditor/AlbumSongLibrary';
import { SetlistEditorHeader } from '../components/SetlistEditor/SetlistEditorHeader';
import { SetlistSongList } from '../components/SetlistEditor/SetlistSongList';
import { RitualCard } from '../components/ui/RitualCard';
import type { AlbumWithSongs } from '../types/album';
import type { SetlistDetails } from '../types/setlist';

interface Props {
  bandId: string;
  bandName?: string;
  setlistId: string;
  onBack: () => void;
}

export function SetlistEditorPage({ bandId, bandName, setlistId, onBack }: Props) {
  const [setlist, setSetlist] = useState<SetlistDetails | null>(null);
  const [localSongs, setLocalSongs] = useState<SetlistDetails['songs']>([]);
  const [savedSongs, setSavedSongs] = useState<SetlistDetails['songs']>([]);
  const [albums, setAlbums] = useState<AlbumWithSongs[]>([]);
  const [loading, setLoading] = useState(true);
  const [songOp, setSongOp] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const isDirty = (() => {
    if (localSongs.length !== savedSongs.length) return true;
    return localSongs.some((s, i) => s.songId !== savedSongs[i]?.songId);
  })();

  useEffect(() => {
    setLoading(true);
    setError(null);
    Promise.all([
      getSetlistDetails(bandId, setlistId),
      getAlbumsWithSongs(bandId)
    ])
      .then(([sl, alb]) => {
        setSetlist(sl);
        setLocalSongs(sl.songs);
        setSavedSongs(sl.songs);
        setAlbums(alb);
      })
      .catch(e => setError(e instanceof Error ? e.message : 'Failed to load editor data.'))
      .finally(() => setLoading(false));
  }, [bandId, setlistId]);

  const handleAdd = (songId: string) => {
    const albumSong = albums.flatMap(a => a.songs).find(s => s.songId === songId);
    if (!albumSong) return;
    setLocalSongs(prev => [
      ...prev,
      {
        setlistSongId: `temp-${songId}`,
        songId: albumSong.songId,
        title: albumSong.title,
        bpm: albumSong.bpm,
        durationSeconds: albumSong.durationSeconds,
        positionIndex: prev.length,
        tuning: albumSong.tuning,
        songKey: albumSong.songKey,
        notes: albumSong.notes,
        albumId: albumSong.albumId ?? undefined,
        albumTitle: albumSong.albumTitle,
        albumTrackNumber: albumSong.albumTrackNumber,
      }
    ]);
  };

  const handleRemove = (setlistSongId: string) => {
    setLocalSongs(prev =>
      prev
        .filter(s => s.setlistSongId !== setlistSongId)
        .map((s, i) => ({ ...s, positionIndex: i }))
    );
  };

  const handleMoveUp = (setlistSongId: string) => {
    setLocalSongs(prev => {
      const songs = [...prev];
      const idx = songs.findIndex(s => s.setlistSongId === setlistSongId);
      if (idx <= 0) return prev;
      [songs[idx - 1], songs[idx]] = [songs[idx], songs[idx - 1]];
      return songs.map((s, i) => ({ ...s, positionIndex: i }));
    });
  };

  const handleMoveDown = (setlistSongId: string) => {
    setLocalSongs(prev => {
      const songs = [...prev];
      const idx = songs.findIndex(s => s.setlistSongId === setlistSongId);
      if (idx < 0 || idx >= songs.length - 1) return prev;
      [songs[idx], songs[idx + 1]] = [songs[idx + 1], songs[idx]];
      return songs.map((s, i) => ({ ...s, positionIndex: i }));
    });
  };

  const handleSaveOrder = async () => {
    if (!setlist || !isDirty) return;
    setSongOp(true);
    setError(null);
    try {
      const desiredOrder = localSongs.map(s => s.songId);

      // Removes: songs in savedSongs not present in local
      const toRemove = savedSongs.filter(s => !localSongs.find(l => l.songId === s.songId));
      for (const s of toRemove) {
        await removeSongFromSetlist(bandId, setlistId, s.setlistSongId);
      }

      // Adds: songs with temp IDs
      const toAdd = localSongs.filter(s => s.setlistSongId.startsWith('temp-'));
      for (const s of toAdd) {
        await addSongToSetlist(bandId, setlistId, s.songId);
      }

      // Fetch fresh state, then reorder to match desired order
      let fresh = await getSetlistDetails(bandId, setlistId);
      const ordered = desiredOrder
        .map((songId, i) => {
          const entry = fresh.songs.find(s => s.songId === songId);
          return entry ? { setlistSongId: entry.setlistSongId, positionIndex: i } : null;
        })
        .filter((x): x is { setlistSongId: string; positionIndex: number } => x !== null);

      if (ordered.length > 0) {
        fresh = await reorderSetlistSongs(bandId, setlistId, ordered);
      }

      setSetlist(fresh);
      setLocalSongs(fresh.songs);
      setSavedSongs(fresh.songs);
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Failed to save.');
    } finally {
      setSongOp(false);
    }
  };

  const setlistSongIds = new Set(localSongs.map(s => s.songId));

  if (loading) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-zinc-950">
        <RitualCard className="w-full max-w-lg text-center">
          <p className="text-sm uppercase tracking-[0.32em] text-zinc-500">Loading Editor</p>
          <p className="mt-3 text-xl font-semibold text-zinc-200">Preparing setlist data...</p>
          <div className="mx-auto mt-5 h-1.5 w-32 overflow-hidden rounded-full bg-zinc-800">
            <div className="ritual-pulse h-full w-1/2 rounded-full bg-red-600" />
          </div>
        </RitualCard>
      </div>
    );
  }

  if (!setlist) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-zinc-950">
        <RitualCard className="w-full max-w-lg border-red-900/60 text-center">
          <p className="text-base text-red-300">{error ?? 'Setlist not found.'}</p>
          <button onClick={onBack} className="mt-4 text-sm text-zinc-400 underline hover:text-zinc-100">
            Go back
          </button>
        </RitualCard>
      </div>
    );
  }

  return (
    <div className="flex h-screen flex-col bg-zinc-950">
      <SetlistEditorHeader setlist={setlist} bandName={bandName} onBack={onBack} />

      {error && (
        <div className="border-b border-red-900/50 bg-red-950/30 px-6 py-3">
          <div className="mx-auto w-full max-w-6xl px-2 text-base text-red-300">{error}</div>
        </div>
      )}

      <div className="mx-auto flex min-h-0 w-full max-w-6xl flex-1 overflow-hidden px-8 py-6">
        <div className="flex min-h-0 flex-1 flex-col overflow-hidden rounded-2xl border border-zinc-800/70 xl:flex-row">
        {/* Left: current setlist */}
        <div className="flex min-h-0 flex-1 flex-col overflow-hidden border-r border-zinc-800/60 bg-zinc-950/40">
          <div className="border-b border-zinc-800/60 bg-zinc-900/40 px-6 py-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-semibold uppercase tracking-[0.22em] text-zinc-400">
                  Ritual Sequence
                </p>
                <p className="mt-0.5 text-sm text-zinc-500">{localSongs.length} songs in order</p>
              </div>
              {isDirty && (
                <button
                  onClick={handleSaveOrder}
                  disabled={songOp}
                  className="rounded-lg bg-red-700 px-5 py-2 text-sm font-semibold uppercase tracking-[0.18em] text-white shadow hover:bg-red-600 disabled:opacity-50"
                >
                  Save Order
                </button>
              )}
            </div>
          </div>
          <div className="min-h-0 flex-1 overflow-y-auto">
            <SetlistSongList
              songs={localSongs}
              onRemove={handleRemove}
              onMoveUp={handleMoveUp}
              onMoveDown={handleMoveDown}
              isLoading={songOp}
            />
          </div>
        </div>

        {/* Right: song library */}
        <div className="flex min-h-0 w-full flex-col overflow-hidden bg-zinc-950/30 xl:w-[460px] xl:shrink-0">
          <div className="border-b border-zinc-800/60 bg-zinc-900/40 px-6 py-4">
            <p className="text-sm font-semibold uppercase tracking-[0.22em] text-zinc-400">
              Song Library
            </p>
            <p className="mt-0.5 text-sm text-zinc-500">Grouped by album</p>
          </div>
          <div className="min-h-0 flex-1 overflow-y-auto">
            <AlbumSongLibrary
              albums={albums}
              setlistSongIds={setlistSongIds}
              onAdd={handleAdd}
              isLoading={songOp}
            />
          </div>
        </div>
      </div>
      </div>

    </div>
  );
}
