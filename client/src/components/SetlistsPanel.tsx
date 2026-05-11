import { useState } from 'react';
import { exportSetlistDocx, exportSetlistPdf } from '../api/setlistExportsApi';
import type { Setlist } from '../types/setlist';
import { RitualButton } from './ui/RitualButton';

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
  bandId: string;
  setlists: Setlist[];
  isLoading: boolean;
  onCreate: () => void;
  onEdit: (setlistId: string) => void;
  onDelete: (setlistId: string) => void;
  onBeginRitual: (setlistId: string) => void;
}

export function SetlistsPanel({ bandId, setlists, isLoading, onCreate, onEdit, onDelete, onBeginRitual }: Props) {
  const [exportingId, setExportingId] = useState<string | null>(null);
  const [exportError, setExportError] = useState<string | null>(null);

  const handleExport = async (setlistId: string, format: 'pdf' | 'docx') => {
    setExportingId(`${setlistId}-${format}`);
    setExportError(null);
    try {
      if (format === 'pdf') {
        await exportSetlistPdf(bandId, setlistId);
      } else {
        await exportSetlistDocx(bandId, setlistId);
      }
    } catch (e) {
      setExportError(e instanceof Error ? e.message : 'Export failed. Please try again.');
    } finally {
      setExportingId(null);
    }
  };

  if (isLoading) {
    return (
      <div className="py-12 text-center text-sm text-zinc-500">Loading setlists...</div>
    );
  }

  if (setlists.length === 0) {
    return (
      <div className="py-14 text-center text-sm text-zinc-500">
        <p>No setlists found.</p>
        <RitualButton variant="primary" size="sm" className="mt-4" onClick={onCreate}>
          Create Setlist
        </RitualButton>
      </div>
    );
  }

  return (
    <div className="flex flex-col divide-y divide-zinc-800/70">
      {exportError && (
        <div className="mx-5 mt-3 rounded-lg border border-red-900/60 bg-red-950/40 px-3 py-2 text-xs text-red-300">
          {exportError}
        </div>
      )}
      {setlists.map(sl => (
        <div key={sl.setlistId} className="flex flex-col gap-3 px-5 py-4 transition-colors hover:bg-zinc-800/20">
          <div className="flex items-start justify-between gap-3">
            <div className="min-w-0">
              <p className="truncate text-sm font-bold text-zinc-100">{sl.name}</p>
              {sl.description && (
                <p className="mt-0.5 truncate text-xs text-zinc-500">{sl.description}</p>
              )}
            </div>
            <div className="flex shrink-0 items-center gap-1.5 text-xs text-zinc-500">
              <span className="font-mono tabular-nums">{fmtDuration(sl.totalDurationSeconds)}</span>
              <span className="text-zinc-700">·</span>
              <span>{sl.totalSongs} songs</span>
            </div>
          </div>

          <div className="flex items-center gap-2">
            <RitualButton
              variant="neutral"
              size="sm"
              onClick={() => onEdit(sl.setlistId)}
              className="flex-1"
            >
              Edit
            </RitualButton>
            <RitualButton
              variant="primary"
              size="sm"
              onClick={() => onBeginRitual(sl.setlistId)}
              disabled={sl.totalSongs === 0}
              className="flex-1"
            >
              Begin Ritual
            </RitualButton>
            <RitualButton
              variant="ghost"
              size="sm"
              disabled={!!exportingId}
              onClick={() => handleExport(sl.setlistId, 'pdf')}
              className="flex-1"
            >
              {exportingId === `${sl.setlistId}-pdf` ? 'Exporting…' : 'PDF'}
            </RitualButton>
            <RitualButton
              variant="ghost"
              size="sm"
              disabled={!!exportingId}
              onClick={() => handleExport(sl.setlistId, 'docx')}
              className="flex-1"
            >
              {exportingId === `${sl.setlistId}-docx` ? 'Exporting…' : 'Word'}
            </RitualButton>
            <RitualButton
              variant="danger"
              size="sm"
              onClick={() => onDelete(sl.setlistId)}
              className="flex-1"
            >
              Delete
            </RitualButton>
          </div>
        </div>
      ))}
    </div>
  );
}
