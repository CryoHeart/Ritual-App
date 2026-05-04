import type { Setlist } from '../types/setlist';

function fmt(seconds?: number): string {
  if (!seconds) return '—';
  const m = Math.floor(seconds / 60);
  const s = seconds % 60;
  return `${m}:${s.toString().padStart(2, '0')}`;
}

function totalDuration(setlist: Setlist): string {
  const total = setlist.songs.reduce((sum, s) => sum + (s.durationSeconds ?? 0), 0);
  if (total === 0) return '—';
  const h = Math.floor(total / 3600);
  const m = Math.floor((total % 3600) / 60);
  const s = total % 60;
  if (h > 0) return `${h}h ${m}m`;
  if (m > 0) return `${m}m ${s}s`;
  return `${s}s`;
}

interface Props {
  setlists: Setlist[];
  selectedSetlist: Setlist | null;
  onSelectSetlist: (setlist: Setlist) => void;
  onBeginRitual: () => void;
}

export function SetlistPanel({ setlists, selectedSetlist, onSelectSetlist, onBeginRitual }: Props) {
  if (setlists.length === 0) {
    return (
      <div className="py-10 text-center text-sm text-zinc-600">
        No setlists found.
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-4 p-4">
      {/* Setlist tabs */}
      <div className="flex flex-wrap gap-2">
        {setlists.map(sl => (
          <button
            key={sl.id}
            onClick={() => onSelectSetlist(sl)}
            className={`rounded-md px-3 py-1.5 text-xs font-medium transition-colors ${
              selectedSetlist?.id === sl.id
                ? 'bg-red-600 text-white'
                : 'bg-zinc-800 text-zinc-400 hover:bg-zinc-700 hover:text-white'
            }`}
          >
            {sl.name}
          </button>
        ))}
      </div>

      {selectedSetlist && (
        <>
          {/* Song order */}
          <div className="rounded-lg border border-zinc-800 overflow-hidden">
            {selectedSetlist.songs.length === 0 ? (
              <div className="py-8 text-center text-sm text-zinc-600">
                This setlist is empty.
              </div>
            ) : (
              <div className="divide-y divide-zinc-800/40 max-h-72 overflow-y-auto">
                {selectedSetlist.songs.map(song => (
                  <div
                    key={song.songId}
                    className="flex items-center gap-3 bg-zinc-900/80 px-4 py-3 hover:bg-zinc-800/40 transition-colors"
                  >
                    <span className="w-5 shrink-0 text-right font-mono text-xs text-red-600/70">
                      {song.order}
                    </span>
                    <div className="min-w-0 flex-1">
                      <p className="truncate text-sm font-medium text-white">{song.title}</p>
                      {song.transitionNotes && (
                        <p className="mt-0.5 truncate text-xs text-zinc-500">
                          {song.transitionNotes}
                        </p>
                      )}
                    </div>
                    <div className="flex shrink-0 items-center gap-3 text-xs text-zinc-500">
                      {song.bpm != null && <span>{song.bpm}</span>}
                      <span className="w-10 text-right tabular-nums">{fmt(song.durationSeconds)}</span>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>

          {/* Summary row */}
          <div className="flex items-center justify-between px-1 text-xs text-zinc-500">
            <span>{selectedSetlist.songs.length} songs</span>
            <span className="tabular-nums">{totalDuration(selectedSetlist)}</span>
          </div>

          {/* Begin Ritual */}
          <button
            onClick={onBeginRitual}
            disabled={selectedSetlist.songs.length === 0}
            className="w-full rounded-lg border border-red-700 bg-red-600 py-4 text-sm font-bold uppercase tracking-[0.25em] text-white shadow-lg shadow-red-950/40 transition-all hover:bg-red-700 hover:shadow-red-900/50 active:scale-[0.99] disabled:cursor-not-allowed disabled:opacity-40"
          >
            Begin Ritual
          </button>
        </>
      )}
    </div>
  );
}
