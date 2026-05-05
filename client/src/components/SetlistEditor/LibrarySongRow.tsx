import type { AlbumSong } from '../../types/album';

function fmt(seconds?: number): string {
  if (!seconds) return '—';
  const m = Math.floor(seconds / 60);
  const s = seconds % 60;
  return `${m}:${s.toString().padStart(2, '0')}`;
}

interface Props {
  song: AlbumSong;
  isAdded: boolean;
  onAdd: () => void;
}

export function LibrarySongRow({ song, isAdded, onAdd }: Props) {
  return (
    <div className="flex items-center gap-3 px-4 py-2.5 transition-colors hover:bg-zinc-800/25">
      {/* Track number */}
      <span className="w-5 shrink-0 text-right text-xs font-mono tabular-nums text-zinc-600">
        {song.albumTrackNumber ?? '—'}
      </span>

      {/* Song info */}
      <div className="min-w-0 flex-1">
        <p className="truncate text-sm text-zinc-200">{song.title}</p>
        {(song.tuning || song.songKey) && (
          <p className="mt-0.5 truncate text-xs text-zinc-600">
            {[song.songKey, song.tuning].filter(Boolean).join(' · ')}
          </p>
        )}
      </div>

      {/* Duration */}
      <span className="shrink-0 w-12 text-right font-mono text-xs tabular-nums text-zinc-500">
        {fmt(song.durationSeconds)}
      </span>

      <div className="flex shrink-0 items-center gap-2">
        {isAdded ? (
          <span className="rounded border border-zinc-700 px-3 py-1 text-xs font-semibold uppercase tracking-[0.12em] text-zinc-600">
            Added
          </span>
        ) : (
          <button
            onClick={onAdd}
            className="rounded border border-red-800 bg-red-950/40 px-3 py-1 text-xs font-semibold uppercase tracking-[0.12em] text-red-300 transition-all hover:border-red-600 hover:bg-red-900/50 hover:text-red-100"
          >
            Add
          </button>
        )}
      </div>
    </div>
  );
}
