import type { SetlistDetails } from '../../types/setlist';
import { RitualBadge } from '../ui/RitualBadge';
import { RitualButton } from '../ui/RitualButton';

function fmtDuration(seconds: number): string {
  if (seconds === 0) return '—';
  const h = Math.floor(seconds / 3600);
  const m = Math.floor((seconds % 3600) / 60);
  const s = seconds % 60;
  if (h > 0) return `${h}h ${m}m`;
  if (m > 0) return `${m}m ${s}s`;
  return `${s}s`;
}

interface Props {
  setlist: SetlistDetails;
  bandName?: string;
  onBack: () => void;
}

export function SetlistEditorHeader({ setlist, bandName, onBack }: Props) {
  return (
    <div className="flex flex-col gap-3 border-b border-zinc-800 px-6 py-5 sm:flex-row sm:items-center sm:justify-between">
      <div className="flex items-center gap-4">
        <RitualButton variant="ghost" onClick={onBack} className="shrink-0">
          ← Back
        </RitualButton>
        <div className="min-w-0">
          <p className="text-xs uppercase tracking-[0.26em] text-zinc-500">Setlist Editor</p>
          <h2 className="mt-0.5 truncate text-xl font-bold text-zinc-100">{setlist.name}</h2>
          {setlist.description && (
            <p className="mt-0.5 truncate text-sm text-zinc-400">{setlist.description}</p>
          )}
        </div>
      </div>
      <div className="flex shrink-0 flex-wrap items-center gap-2">
        {bandName && <RitualBadge tone="accent">{bandName}</RitualBadge>}
        <RitualBadge>{setlist.totalSongs} Songs</RitualBadge>
        <RitualBadge>{fmtDuration(setlist.totalDurationSeconds)}</RitualBadge>
      </div>
    </div>
  );
}
