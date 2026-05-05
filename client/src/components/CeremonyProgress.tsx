interface CeremonyProgressProps {
  currentIndex: number;
  totalSongs: number;
  totalDurationLabel: string;
}

export function CeremonyProgress({ currentIndex, totalSongs, totalDurationLabel }: CeremonyProgressProps) {
  const progressPercent = totalSongs === 0 ? 0 : ((currentIndex + 1) / totalSongs) * 100;

  return (
    <div className="mb-8 rounded-2xl border border-zinc-800/90 bg-zinc-900/60 p-4 shadow-[0_10px_30px_rgba(0,0,0,0.35)]">
      <div className="mb-2 flex items-center justify-between text-[10px] uppercase tracking-[0.24em] text-zinc-500">
        <span>Stage Progress</span>
        <span>Set Duration {totalDurationLabel}</span>
      </div>
      <div className="h-4 w-full rounded-lg border border-zinc-700 bg-zinc-950">
        <div
          className="h-full rounded-md bg-gradient-to-r from-red-900 via-red-700 to-red-500 transition-all duration-300"
          style={{ width: `${progressPercent}%` }}
        />
      </div>
      <div className="mt-3 flex items-center justify-between px-1 text-sm text-zinc-400">
        <span className="font-semibold text-zinc-100">{currentIndex + 1} / {totalSongs}</span>
        <span className="font-mono tabular-nums text-zinc-300">{totalDurationLabel}</span>
      </div>
    </div>
  );
}