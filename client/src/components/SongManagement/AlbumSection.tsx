import { useState } from 'react';
import type { AlbumWithSongs } from '../../types/album';
import { SongRow } from './SongRow';

interface Props {
  album: AlbumWithSongs;
  searchQuery: string;
  onEditAlbum?: () => void;
  onDeleteAlbum?: () => void;
  onDeleteAllSongs?: () => void;
  onEditSong: (songId: string) => void;
  onDeleteSong: (songId: string) => void;
}

export function AlbumSection({ album, searchQuery, onEditAlbum, onDeleteAlbum, onDeleteAllSongs, onEditSong, onDeleteSong }: Props) {
  const [collapsed, setCollapsed] = useState(false);

  const isUnassigned = album.albumId === null;
  const q = searchQuery.trim().toLowerCase();
  const visibleSongs = q
    ? album.songs.filter(s => s.title.toLowerCase().includes(q))
    : album.songs;

  if (q && visibleSongs.length === 0) return null;

  return (
    <div className="border border-zinc-800/60 rounded-xl overflow-hidden">
      {/* Album header */}
      <div className="flex items-center gap-3 bg-zinc-900/70 px-4 py-3">
        <button
          onClick={() => setCollapsed(c => !c)}
          className="flex min-w-0 flex-1 items-center gap-3 text-left"
        >
          <span className={`text-sm font-bold uppercase tracking-[0.18em] ${isUnassigned ? 'text-zinc-500' : 'text-red-400'}`}>
            {album.title}
          </span>
          {album.releaseYear != null && (
            <span className="shrink-0 text-xs text-zinc-600">{album.releaseYear}</span>
          )}
          <span className="shrink-0 text-xs text-zinc-600">
            {album.songs.length} {album.songs.length === 1 ? 'song' : 'songs'}
          </span>
          <span className="ml-1 shrink-0 text-xs text-zinc-600">{collapsed ? '▶' : '▼'}</span>
        </button>

        {!isUnassigned && (
          <div className="flex shrink-0 items-center gap-1.5">
            {onEditAlbum && (
              <button
                onClick={onEditAlbum}
                className="rounded border border-zinc-700 bg-zinc-900 px-2.5 py-1 text-xs font-semibold uppercase tracking-[0.12em] text-zinc-300 transition-all hover:border-zinc-500 hover:text-zinc-100"
              >
                Edit
              </button>
            )}
            {onDeleteAlbum && (
              <button
                onClick={onDeleteAlbum}
                className="rounded border border-red-900/60 bg-red-950/30 px-2.5 py-1 text-xs font-semibold uppercase tracking-[0.12em] text-red-400 transition-all hover:border-red-600 hover:text-red-200"
              >
                Delete
              </button>
            )}
          </div>
        )}

        {isUnassigned && onDeleteAllSongs && album.songs.length > 0 && (
          <button
            onClick={onDeleteAllSongs}
            className="shrink-0 rounded border border-red-900/60 bg-red-950/30 px-2.5 py-1 text-xs font-semibold uppercase tracking-[0.12em] text-red-400 transition-all hover:border-red-600 hover:text-red-200"
          >
            Delete all
          </button>
        )}
      </div>

      {/* Songs */}
      {!collapsed && (
        <div className="bg-zinc-950/40">
          {visibleSongs.length === 0 ? (
            <p className="px-4 py-4 text-xs text-zinc-600">No songs in this album.</p>
          ) : (
            visibleSongs.map(song => (
              <SongRow
                key={song.songId}
                song={song}
                onEdit={() => onEditSong(song.songId)}
                onDelete={() => onDeleteSong(song.songId)}
              />
            ))
          )}
        </div>
      )}
    </div>
  );
}
