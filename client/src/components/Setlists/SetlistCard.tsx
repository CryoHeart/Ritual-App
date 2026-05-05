import type { SetlistSummary } from '../../types/setlist';
import { RitualButton } from '../ui/RitualButton';

function formatDuration(seconds: number): string {
  const safeSeconds = Math.max(0, seconds || 0);
  const hours = Math.floor(safeSeconds / 3600);
  const minutes = Math.floor((safeSeconds % 3600) / 60);
  const remaining = safeSeconds % 60;

  if (hours > 0) {
    return `${hours}:${minutes.toString().padStart(2, '0')}:${remaining.toString().padStart(2, '0')}`;
  }

  return `${minutes}:${remaining.toString().padStart(2, '0')}`;
}

interface SetlistCardProps {
  setlist: SetlistSummary;
  isSelected: boolean;
  isExpanded: boolean;
  onSelect: (id: string) => void;
  onEdit: (setlist: SetlistSummary) => void;
  onDelete: (setlist: SetlistSummary) => void;
  onOpenEditor: (setlistId: string) => void;
  onBeginRitual: (setlistId: string) => void;
}

export function SetlistCard({
  setlist,
  isSelected,
  isExpanded,
  onSelect,
  onEdit,
  onDelete,
  onOpenEditor,
  onBeginRitual
}: SetlistCardProps) {
  return (
    <div
      onClick={() => onSelect(setlist.setlistId)}
      className={`cursor-pointer rounded-xl border bg-zinc-950/45 p-4 transition-colors ${
        isExpanded ? 'border-red-700/70 shadow-[0_0_0_1px_rgba(127,29,29,0.45)]' : 'border-zinc-800/80 hover:border-zinc-700'
      }`}
    >
      <div className="flex items-start justify-between gap-3">
        <div className="min-w-0">
          <h4 className="truncate text-base font-bold text-zinc-100">{setlist.name}</h4>
          <p className="mt-1 line-clamp-2 min-h-[2.5rem] text-sm text-zinc-400">
            {setlist.description?.trim() || 'No ritual notes yet.'}
          </p>
        </div>

        <div className="shrink-0 text-right text-xs text-zinc-500">
          <p>{setlist.totalSongs} songs</p>
          <p className="font-mono tabular-nums text-zinc-300">{formatDuration(setlist.totalDurationSeconds)}</p>
        </div>
      </div>

      {isExpanded && (
        <div className="mt-3 grid grid-cols-2 gap-2 xl:grid-cols-4" onClick={e => e.stopPropagation()}>
          <RitualButton variant="ghost" size="sm" onClick={() => onEdit(setlist)}>
            Edit
          </RitualButton>
          <RitualButton variant="danger" size="sm" onClick={() => onDelete(setlist)}>
            Delete
          </RitualButton>
          <RitualButton variant="neutral" size="sm" onClick={() => onOpenEditor(setlist.setlistId)}>
            Open Editor
          </RitualButton>
          <RitualButton
            variant="primary"
            size="sm"
            disabled={setlist.totalSongs === 0}
            onClick={() => onBeginRitual(setlist.setlistId)}
          >
            Begin Ritual
          </RitualButton>
        </div>
      )}
    </div>
  );
}
