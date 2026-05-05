import type { SetlistSong } from '../../types/setlist';
import { SetlistSongRow } from './SetlistSongRow';

interface Props {
  songs: SetlistSong[];
  onRemove: (setlistSongId: string) => void;
  onMoveUp: (setlistSongId: string) => void;
  onMoveDown: (setlistSongId: string) => void;
  isLoading?: boolean;
}

export function SetlistSongList({ songs, onRemove, onMoveUp, onMoveDown, isLoading }: Props) {
  if (songs.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center py-16 text-center">
        <p className="text-xs uppercase tracking-[0.28em] text-zinc-600">Empty Setlist</p>
        <p className="mt-2 text-sm text-zinc-500">Add songs from the library on the right.</p>
      </div>
    );
  }

  return (
    <div className={`divide-y divide-zinc-800/80 transition-opacity ${isLoading ? 'pointer-events-none opacity-50' : ''}`}>
      {songs.map((song, idx) => (
        <SetlistSongRow
          key={song.setlistSongId}
          song={song}
          isFirst={idx === 0}
          isLast={idx === songs.length - 1}
          onRemove={() => onRemove(song.setlistSongId)}
          onMoveUp={() => onMoveUp(song.setlistSongId)}
          onMoveDown={() => onMoveDown(song.setlistSongId)}
        />
      ))}
    </div>
  );
}
