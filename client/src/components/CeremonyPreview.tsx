import type { SetlistDetails } from '../types/setlist';

interface Props {
  setlist: SetlistDetails | null;
}

export function CeremonyPreview({ setlist }: Props) {
  if (!setlist || setlist.songs.length === 0) return null;

  const current = setlist.songs[0];
  const next = setlist.songs[1] ?? null;

  return (
    <div className="overflow-hidden rounded-2xl border border-zinc-800/90 bg-[linear-gradient(180deg,rgba(29,29,31,0.9),rgba(17,17,19,0.96))] shadow-[0_16px_40px_rgba(0,0,0,0.35)]">
      {/* Header */}
      <div className="flex items-center justify-between border-b border-zinc-800 px-5 py-3.5">
        <p className="text-[10px] font-semibold uppercase tracking-[0.28em] text-zinc-400">
          Ceremony Preview
        </p>
        <p className="text-[10px] uppercase tracking-[0.22em] text-zinc-500">{setlist.name}</p>
      </div>

      <div className="grid grid-cols-1 gap-4 p-5 sm:grid-cols-3">
        {/* Now playing */}
        <div className="sm:col-span-2 rounded-xl border border-red-900/40 bg-red-950/20 p-5">
          <p className="mb-2 text-[10px] font-semibold uppercase tracking-[0.3em] text-red-400">
            Now
          </p>
          <p className="text-2xl font-semibold leading-tight text-white">{current.title}</p>
          <div className="mt-3 flex items-center gap-4 text-xs text-zinc-500">
            {current.bpm != null && <span>{current.bpm} BPM</span>}
            {current.performanceNotes && (
              <span className="truncate text-zinc-500">{current.performanceNotes}</span>
            )}
          </div>
        </div>

        {/* Up next */}
        <div className="rounded-xl border border-zinc-800 bg-zinc-900/60 p-5">
          <p className="mb-2 text-[10px] font-semibold uppercase tracking-[0.3em] text-zinc-500">
            Next
          </p>
          {next ? (
            <>
              <p className="text-lg font-semibold text-zinc-200">{next.title}</p>
              {next.bpm != null && (
                <p className="mt-2 text-xs text-zinc-500">{next.bpm} BPM</p>
              )}
            </>
          ) : (
            <p className="text-sm text-zinc-500">End of setlist</p>
          )}
        </div>
      </div>

      {/* Progress bar */}
      <div className="flex gap-1 px-5 pb-5">
        {setlist.songs.map((song, i) => (
          <div
            key={song.songId}
            title={song.title}
            className={`h-1 flex-1 rounded-full ${i === 0 ? 'bg-red-600' : 'bg-zinc-800'}`}
          />
        ))}
      </div>
    </div>
  );
}
