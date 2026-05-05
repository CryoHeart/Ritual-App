import { FitText } from './ui/FitText';

interface CeremonySongCardProps {
  currentTitle: string;
  currentDurationLabel: string;
  currentTuning?: string;
  currentNotes?: string;
  nextTitle: string;
  nextNotes?: string;
  currentIndex: number;
  totalSongs: number;
}

export function CeremonySongCard({
  currentTitle,
  currentDurationLabel,
  currentTuning,
  currentNotes,
  nextTitle,
  nextNotes,
  currentIndex,
  totalSongs
}: CeremonySongCardProps) {
  return (
    <div className="grid flex-1 grid-cols-1 gap-6 lg:grid-cols-3">
      <section className="lg:col-span-2 rounded-3xl border border-red-900/40 bg-[linear-gradient(180deg,rgba(35,10,10,0.75),rgba(15,10,10,0.82))] p-8 shadow-[0_12px_38px_rgba(80,12,12,0.35)] lg:p-12">
        <p className="text-[11px] font-semibold uppercase tracking-[0.4em] text-red-400">Now</p>
        <FitText
          text={currentTitle}
          maxPx={80}
          minPx={24}
          className="mt-4 font-semibold leading-none text-white"
        />
        <div className="mt-8 flex flex-wrap gap-x-8 gap-y-3 text-lg text-zinc-200 lg:text-2xl">
          <p>
            <span className="mr-2 text-zinc-400">Duration</span>
            <span className="tabular-nums">{currentDurationLabel}</span>
          </p>
          {currentTuning && (
            <p>
              <span className="mr-2 text-zinc-400">Tuning</span>
              <span>{currentTuning}</span>
            </p>
          )}
        </div>

        {currentNotes && (
          <div className="mt-8 rounded-2xl border border-zinc-700/80 bg-zinc-950/75 p-5">
            <p className="text-xs uppercase tracking-[0.35em] text-zinc-500">Notes</p>
            <p className="mt-3 text-lg leading-relaxed text-zinc-100 lg:text-2xl">{currentNotes}</p>
          </div>
        )}
      </section>

      <aside className="rounded-3xl border border-zinc-800/90 bg-zinc-900/55 p-8">
        <p className="text-xs uppercase tracking-[0.35em] text-zinc-500">Next</p>
        <p className={`mt-5 text-3xl font-semibold leading-tight lg:text-4xl ${nextTitle === 'End of Setlist' ? 'font-bold text-red-500' : 'text-zinc-100'}`}>{nextTitle}</p>
        {nextNotes && (
          <div className="mt-4 rounded-xl border border-zinc-700/60 bg-zinc-950/50 p-4">
            <p className="text-xs uppercase tracking-[0.3em] text-zinc-500">Notes</p>
            <p className="mt-2 text-base leading-relaxed text-zinc-300">{nextNotes}</p>
          </div>
        )}
        <p className="mt-10 text-sm uppercase tracking-[0.25em] text-zinc-500">Queue Position</p>
        <p className="mt-2 text-3xl font-semibold text-red-400">{currentIndex + 1} / {totalSongs}</p>
      </aside>
    </div>
  );
}