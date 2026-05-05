import type { AlbumWithSongs } from '../../types/album';
import { AlbumSection } from './AlbumSection';

interface Props {
  albums: AlbumWithSongs[];
  setlistSongIds: Set<string>;
  onAdd: (songId: string) => void;
  isLoading?: boolean;
}

export function AlbumSongLibrary({ albums, setlistSongIds, onAdd, isLoading }: Props) {
  if (albums.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center py-16 text-center">
        <p className="text-xs uppercase tracking-[0.28em] text-zinc-600">No Albums</p>
        <p className="mt-2 text-sm text-zinc-500">No album data found for this band.</p>
      </div>
    );
  }

  return (
    <div className={`divide-y divide-zinc-800/60 transition-opacity ${isLoading ? 'pointer-events-none opacity-50' : ''}`}>
      {albums.map(album => (
        <AlbumSection
          key={album.albumId ?? 'unassigned'}
          album={album}
          setlistSongIds={setlistSongIds}
          onAdd={onAdd}
        />
      ))}
    </div>
  );
}
