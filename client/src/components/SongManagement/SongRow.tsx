import type { AlbumSong } from '../../types/album';

function fmt(seconds?: number): string {
  if (!seconds) return '—';
  const m = Math.floor(seconds / 60);
  const s = seconds % 60;
  return `${m}:${s.toString().padStart(2, '0')}`;
}

interface Props {
  song: AlbumSong;
  onEdit: () => void;
  onDelete: () => void;
}

export function SongRow({ song, onEdit, onDelete }: Props) {
  return (
    <div className="flex items-center gap-3 border-b border-zinc-800/40 px-4 py-2.5 transition-colors last:border-b-0 hover:bg-zinc-800/20">
      <span className="w-6 shrink-0 text-right font-mono text-xs tabular-nums text-zinc-600">
        {song.albumTrackNumber ?? '—'}
      </span>

      <div className="min-w-0 flex-1">
        <p className="truncate text-sm text-zinc-200">{song.title}</p>
        {(song.tuning || song.songKey) && (
          <p className="mt-0.5 truncate text-xs text-zinc-600">
            {[song.songKey, song.tuning].filter(Boolean).join(' · ')}
          </p>
        )}
      </div>

      {song.bpm != null && (
        <span className="shrink-0 rounded border border-zinc-800 px-1.5 py-0.5 font-mono text-[10px] text-zinc-500">
          {song.bpm} BPM
        </span>
      )}

      <span className="w-12 shrink-0 text-right font-mono text-xs tabular-nums text-zinc-500">
        {fmt(song.durationSeconds)}
      </span>

      <div className="flex shrink-0 items-center gap-1.5">
        <button
          onClick={onEdit}
          className="rounded border border-zinc-700 bg-zinc-900/80 px-2.5 py-1 text-xs font-semibold uppercase tracking-[0.12em] text-zinc-300 transition-all hover:border-zinc-500 hover:text-zinc-100"
        >
          Edit
        </button>
        <button
          onClick={onDelete}
          className="rounded border border-red-900/60 bg-red-950/30 px-2.5 py-1 text-xs font-semibold uppercase tracking-[0.12em] text-red-400 transition-all hover:border-red-600 hover:text-red-200"
        >
          Delete
        </button>
      </div>
    </div>
  );
}
