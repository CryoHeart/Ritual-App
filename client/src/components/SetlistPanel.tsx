import type { Setlist, SetlistDetails } from '../types/setlist';
import { RitualBadge } from './ui/RitualBadge';
import { RitualButton } from './ui/RitualButton';

function fmt(seconds?: number): string {
  if (!seconds) return '—';
  const m = Math.floor(seconds / 60);
  const s = seconds % 60;
  return `${m}:${s.toString().padStart(2, '0')}`;
}

function totalDuration(setlist: SetlistDetails): string {
  const total = setlist.totalDurationSeconds;
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
  selectedSetlistId: string | null;
  selectedSetlist: SetlistDetails | null;
  isLoading: boolean;
  onSelectSetlistId: (setlistId: string) => void;
  onBeginRitual: () => void;
}

export function SetlistPanel({ setlists, selectedSetlistId, selectedSetlist, isLoading, onSelectSetlistId, onBeginRitual }: Props) {
  if (setlists.length === 0) {
    return (
      <div className="py-14 text-center text-sm text-zinc-500">
        No setlists found.
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-4 p-5">
      {/* Setlist tabs */}
      <div className="flex flex-wrap gap-2">
        {setlists.map(sl => (
          <button
            key={sl.setlistId}
            onClick={() => onSelectSetlistId(sl.setlistId)}
            className={`rounded-lg border px-3.5 py-2 text-xs font-semibold uppercase tracking-[0.14em] transition-all ${
              selectedSetlistId === sl.setlistId
                ? 'border-red-700 bg-red-950/60 text-red-200 shadow-[0_0_16px_rgba(127,29,29,0.4)]'
                : 'border-zinc-700 bg-zinc-900 text-zinc-400 hover:border-zinc-500 hover:text-zinc-100'
            }`}
          >
            {sl.name}
          </button>
        ))}
      </div>

      {isLoading && (
        <div className="py-12 text-center text-sm text-zinc-500">
          Loading setlist details...
        </div>
      )}

      {selectedSetlist && (
        <>
          {/* Song order */}
          <div className="overflow-hidden rounded-xl border border-zinc-800/90 bg-zinc-950/70">
            {selectedSetlist.totalSongs === 0 ? (
              <div className="py-10 text-center text-sm text-zinc-500">
                This setlist is empty.
              </div>
            ) : (
              <div className="max-h-72 divide-y divide-zinc-800/80 overflow-y-auto">
                {selectedSetlist.songs.map(song => (
                  <div
                    key={song.songId}
                    className="flex items-start gap-3 px-4 py-3.5 transition-colors hover:bg-zinc-800/35"
                  >
                    <span className="inline-flex w-8 shrink-0 items-center justify-center rounded-md border border-zinc-700 bg-zinc-900 py-1 text-xs font-mono text-red-300">
                      {song.positionIndex + 1}
                    </span>
                    <div className="min-w-0 flex-1">
                      <p className="truncate text-sm font-semibold text-zinc-100">{song.title}</p>
                      {(song.transitionNotes || song.performanceNotes) && (
                        <p className="mt-1 truncate text-xs text-zinc-500">
                          {song.transitionNotes || song.performanceNotes}
                        </p>
                      )}
                    </div>
                    <div className="flex shrink-0 items-center gap-2 text-xs text-zinc-500">
                      {song.bpm != null && <RitualBadge>{song.bpm} BPM</RitualBadge>}
                      <span className="w-11 text-right font-mono tabular-nums text-zinc-300">{fmt(song.durationSeconds)}</span>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>

          {/* Summary row */}
          <div className="flex items-center justify-between px-1 text-xs uppercase tracking-[0.18em] text-zinc-500">
            <span>{selectedSetlist.totalSongs} Songs</span>
            <span className="font-mono tabular-nums">{totalDuration(selectedSetlist)}</span>
          </div>

          {/* Begin Ritual */}
          <RitualButton
            onClick={onBeginRitual}
            disabled={selectedSetlist.totalSongs === 0}
            variant="primary"
            size="lg"
            className="ritual-pulse w-full"
          >
            Begin Ritual
          </RitualButton>
        </>
      )}
    </div>
  );
}
