import type { Song } from '../types/song';

function fmt(seconds?: number): string {
  if (!seconds) return '—';
  const m = Math.floor(seconds / 60);
  const s = seconds % 60;
  return `${m}:${s.toString().padStart(2, '0')}`;
}

interface Props {
  songs: Song[];
}

export function SongList({ songs }: Props) {
  if (songs.length === 0) {
    return (
      <div className="py-10 text-center text-sm text-zinc-600">
        No songs found.
      </div>
    );
  }

  return (
    <div className="divide-y divide-zinc-800/40">
      {songs.map((song, i) => (
        <div
          key={song.id}
          className="flex items-center gap-4 px-5 py-3 hover:bg-zinc-800/30 transition-colors"
        >
          <span className="w-6 shrink-0 text-right font-mono text-xs text-zinc-600">
            {String(i + 1).padStart(2, '0')}
          </span>

          <div className="min-w-0 flex-1">
            <p className="truncate text-sm font-medium text-white">{song.title}</p>
            {(song.songKey || song.tuning) && (
              <p className="mt-0.5 text-xs text-zinc-500">
                {[song.songKey, song.tuning].filter(Boolean).join(' · ')}
              </p>
            )}
          </div>

          <div className="flex shrink-0 items-center gap-4 text-xs text-zinc-500">
            {song.bpm != null && <span>{song.bpm} BPM</span>}
            <span className="w-10 text-right tabular-nums">{fmt(song.durationSeconds)}</span>
          </div>
        </div>
      ))}
    </div>
  );
}
