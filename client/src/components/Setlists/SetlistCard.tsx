import { useState } from 'react';
import { exportSetlistDocx, exportSetlistPdf } from '../../api/setlistExportsApi';
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
  bandId: string;
  setlist: SetlistSummary;
  isExpanded: boolean;
  onSelect: (id: string) => void;
  onEdit: (setlist: SetlistSummary) => void;
  onDelete: (setlist: SetlistSummary) => void;
  onOpenEditor: (setlistId: string) => void;
  onBeginRitual: (setlistId: string) => void;
}

export function SetlistCard({
  bandId,
  setlist,
  isExpanded,
  onSelect,
  onEdit,
  onDelete,
  onOpenEditor,
  onBeginRitual
}: SetlistCardProps) {
  const [exportingFormat, setExportingFormat] = useState<'pdf' | 'docx' | null>(null);
  const [exportError, setExportError] = useState<string | null>(null);

  const handleExport = async (format: 'pdf' | 'docx', e: React.MouseEvent) => {
    e.stopPropagation();
    setExportingFormat(format);
    setExportError(null);
    try {
      if (format === 'pdf') {
        await exportSetlistPdf(bandId, setlist.setlistId);
      } else {
        await exportSetlistDocx(bandId, setlist.setlistId);
      }
    } catch (err) {
      setExportError(err instanceof Error ? err.message : 'Export failed. Please try again.');
    } finally {
      setExportingFormat(null);
    }
  };

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
        <div className="mt-3" onClick={e => e.stopPropagation()}>
          <div className="grid grid-cols-2 gap-2 xl:grid-cols-4">
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

          <div className="mt-2 grid grid-cols-2 gap-2">
            <RitualButton
              variant="ghost"
              size="sm"
              disabled={!!exportingFormat}
              onClick={e => handleExport('pdf', e)}
            >
              {exportingFormat === 'pdf' ? 'Exporting…' : 'Export PDF'}
            </RitualButton>
            <RitualButton
              variant="ghost"
              size="sm"
              disabled={!!exportingFormat}
              onClick={e => handleExport('docx', e)}
            >
              {exportingFormat === 'docx' ? 'Exporting…' : 'Export Word'}
            </RitualButton>
          </div>

          {exportError && (
            <p className="mt-2 text-xs text-red-300">{exportError}</p>
          )}
        </div>
      )}
    </div>
  );
}
