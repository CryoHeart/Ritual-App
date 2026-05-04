import type { Setlist } from '../types/setlist';

interface Props {
  setlist: Setlist | null;
}

export function CeremonyPreview({ setlist }: Props) {
  if (!setlist || setlist.songs.length === 0) return null;

  const current = setlist.songs[0];
  const next = setlist.songs[1] ?? null;

  return (
    <div className="rounded-xl border border-zinc-800 bg-zinc-900/40 overflow-hidden">
      {/* Header */}
      <div className="flex items-center justify-between border-b border-zinc-800 px-5 py-3">
        <p className="text-[10px] font-semibold uppercase tracking-[0.25em] text-zinc-500">
          Ceremony Preview
        </p>
        <p className="text-[10px] uppercase tracking-widest text-zinc-600">{setlist.name}</p>
      </div>

      <div className="grid grid-cols-1 gap-4 p-5 sm:grid-cols-3">
        {/* Now playing */}
        <div className="sm:col-span-2 rounded-lg border border-red-900/40 bg-red-950/20 p-5">
          <p className="mb-2 text-[10px] font-semibold uppercase tracking-[0.3em] text-red-500">
            Now
          </p>
          <p className="text-2xl font-bold leading-tight text-white">{current.title}</p>
          <div className="mt-3 flex items-center gap-4 text-xs text-zinc-500">
            {current.bpm != null && <span>{current.bpm} BPM</span>}
            {current.performanceNotes && (
              <span className="truncate text-zinc-600">{current.performanceNotes}</span>
            )}
          </div>
        </div>

        {/* Up next */}
        <div className="rounded-lg border border-zinc-800 bg-zinc-900/60 p-5 opacity-60">
          <p className="mb-2 text-[10px] font-semibold uppercase tracking-[0.3em] text-zinc-500">
            Next
          </p>
          {next ? (
            <>
              <p className="text-lg font-semibold text-zinc-300">{next.title}</p>
              {next.bpm != null && (
                <p className="mt-2 text-xs text-zinc-600">{next.bpm} BPM</p>
              )}
            </>
          ) : (
            <p className="text-sm text-zinc-600">End of setlist</p>
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
