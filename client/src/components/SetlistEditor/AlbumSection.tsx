import { useState } from 'react';
import type { AlbumWithSongs } from '../../types/album';
import { LibrarySongRow } from './LibrarySongRow';

interface Props {
  album: AlbumWithSongs;
  setlistSongIds: Set<string>;
  onAdd: (songId: string) => void;
}

export function AlbumSection({ album, setlistSongIds, onAdd }: Props) {
  const [collapsed, setCollapsed] = useState(false);

  return (
    <div>
      <button
        onClick={() => setCollapsed(c => !c)}
        className="flex w-full items-center gap-2 bg-zinc-900/60 px-4 py-3 text-left transition-colors hover:bg-zinc-800/50"
      >
        <span className="text-xs font-bold uppercase tracking-[0.22em] text-red-400">
          {album.title}
        </span>
        {album.releaseYear && (
          <span className="text-xs text-zinc-600">{album.releaseYear}</span>
        )}
        <span className="ml-auto text-xs text-zinc-600">{album.songs.length} tracks</span>
        <span className="ml-2 text-xs text-zinc-600">{collapsed ? '▶' : '▼'}</span>
      </button>
      {!collapsed && (
        <div className="divide-y divide-zinc-800/50">
          {album.songs.map(song => (
            <LibrarySongRow
              key={song.songId}
              song={song}
              isAdded={setlistSongIds.has(song.songId)}
              onAdd={() => onAdd(song.songId)}
            />
          ))}
        </div>
      )}
    </div>
  );
}
