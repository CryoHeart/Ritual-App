import { useEffect, useState } from 'react';
import { getActiveLiveSession } from '../api/liveSessionsApi';
import type { LiveSession } from '../types/liveSession';

interface OngoingRitualPanelProps {
  bandId: string;
  onEnterLiveMode: (liveSessionId: string) => void;
}

export default function OngoingRitualPanel({ bandId, onEnterLiveMode }: OngoingRitualPanelProps) {
  const [session, setSession] = useState<LiveSession | null | undefined>(undefined);

  useEffect(() => {
    let cancelled = false;
    getActiveLiveSession(bandId)
      .then((s) => { if (!cancelled) setSession(s); })
      .catch(() => { if (!cancelled) setSession(null); });
    return () => { cancelled = true; };
  }, [bandId]);

  if (session === undefined || session === null) return null;

  return (
    <div className="bg-indigo-900 border border-indigo-600 rounded-xl p-4 flex items-center justify-between">
      <div>
        <p className="text-xs text-indigo-300 uppercase tracking-wider mb-1">Live Ritual in Progress</p>
        <p className="font-semibold text-white">{session.setlistName}</p>
        {session.currentSong && (
          <p className="text-sm text-indigo-200 mt-0.5">
            Current: {session.currentSong.title}
          </p>
        )}
      </div>
      <button
        onClick={() => onEnterLiveMode(session.liveSessionId)}
        className="bg-indigo-500 hover:bg-indigo-400 text-white px-4 py-2 rounded-lg font-medium text-sm"
      >
        Enter Live Mode
      </button>
    </div>
  );
}
