import type { SetlistSong } from '../../types/setlist';
import { RitualButton } from '../ui/RitualButton';

function fmt(seconds?: number): string {
  if (!seconds) return '—';
  const m = Math.floor(seconds / 60);
  const s = seconds % 60;
  return `${m}:${s.toString().padStart(2, '0')}`;
}

interface Props {
  song: SetlistSong;
  isFirst: boolean;
  isLast: boolean;
  onRemove: () => void;
  onMoveUp: () => void;
  onMoveDown: () => void;
}

export function SetlistSongRow({ song, isFirst, isLast, onRemove, onMoveUp, onMoveDown }: Props) {
  return (
    <div className="flex items-center gap-3 px-4 py-3 transition-colors hover:bg-zinc-800/30">
      {/* Position */}
      <span className="inline-flex w-7 shrink-0 items-center justify-center rounded border border-zinc-700 bg-zinc-900 py-0.5 text-xs font-mono text-red-300">
        {song.positionIndex + 1}
      </span>

      {/* Song info */}
      <div className="min-w-0 flex-1">
        <p className="truncate text-sm font-semibold text-zinc-100">{song.title}</p>
        {song.albumTitle && (
          <p className="mt-0.5 truncate text-xs text-zinc-500">
            {song.albumTitle}
            {song.albumTrackNumber != null ? ` · #${song.albumTrackNumber}` : ''}
          </p>
        )}
      </div>

      {/* Duration */}
      <span className="shrink-0 w-12 text-right font-mono text-xs tabular-nums text-zinc-400">
        {fmt(song.durationSeconds)}
      </span>

      {/* Controls */}
      <div className="flex shrink-0 items-center gap-1">
        <button
          onClick={onMoveUp}
          disabled={isFirst}
          title="Move up"
          className="flex h-7 w-7 items-center justify-center rounded border border-zinc-700 bg-zinc-900 text-zinc-400 transition-colors hover:border-zinc-500 hover:text-zinc-100 disabled:cursor-not-allowed disabled:opacity-30"
        >
          ↑
        </button>
        <button
          onClick={onMoveDown}
          disabled={isLast}
          title="Move down"
          className="flex h-7 w-7 items-center justify-center rounded border border-zinc-700 bg-zinc-900 text-zinc-400 transition-colors hover:border-zinc-500 hover:text-zinc-100 disabled:cursor-not-allowed disabled:opacity-30"
        >
          ↓
        </button>
        <RitualButton variant="danger" size="sm" onClick={onRemove} className="ml-1">
          Remove
        </RitualButton>
      </div>
    </div>
  );
}
