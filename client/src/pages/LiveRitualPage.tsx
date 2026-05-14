import { useEffect, useState, useCallback, useRef } from 'react';
import type { LiveSession } from '../types/liveSession';
import {
  getLiveSession,
  nextLiveSessionSong,
  previousLiveSessionSong,
  endLiveSession,
} from '../api/liveSessionsApi';
import {
  createLiveRitualConnection,
  registerLiveRitualEvents,
  joinBandGroup,
  joinLiveSessionGroup,
  leaveBandGroup,
  leaveLiveSessionGroup,
} from '../realtime/liveRitualConnection';
import type { HubConnection } from '@microsoft/signalr';
import loginBackground from '../assets/login-background.png';
import { AppShell } from '../components/AppShell';
import { CeremonyProgress } from '../components/CeremonyProgress';
import { CeremonySongCard } from '../components/CeremonySongCard';
import { CeremonyControls } from '../components/CeremonyControls';
import { RitualButton } from '../components/ui/RitualButton';
import { RitualCard } from '../components/ui/RitualCard';

function formatMmSs(totalSeconds?: number): string {
  if (!totalSeconds || totalSeconds <= 0) return '--:--';
  const minutes = Math.floor(totalSeconds / 60);
  const seconds = totalSeconds % 60;
  return `${minutes}:${seconds.toString().padStart(2, '0')}`;
}

interface LiveRitualPageProps {
  bandId: string;
  liveSessionId: string;
  onExit: () => void;
}

export default function LiveRitualPage({ bandId, liveSessionId, onExit }: LiveRitualPageProps) {
  const [session, setSession] = useState<LiveSession | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const connectionRef = useRef<HubConnection | null>(null);

  const refreshSession = useCallback(async () => {
    try {
      const s = await getLiveSession(bandId, liveSessionId);
      setSession(s);
    } catch (e) {
      setError(String(e));
    }
  }, [bandId, liveSessionId]);

  useEffect(() => {
    let cancelled = false;

    (async () => {
      try {
        const s = await getLiveSession(bandId, liveSessionId);
        if (!cancelled) setSession(s);
      } catch (e) {
        if (!cancelled) setError(String(e));
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();

    const connection = createLiveRitualConnection();
    connectionRef.current = connection;

    registerLiveRitualEvents(connection, {
      onRitualUpdated: (s) => { if (!cancelled) setSession(s); },
      onRitualEnded: (s) => { if (!cancelled) setSession(s); },
    });

    connection.start().then(() => {
      joinBandGroup(connection, bandId);
      joinLiveSessionGroup(connection, liveSessionId);
    });

    return () => {
      cancelled = true;
      leaveBandGroup(connection, bandId).catch(() => {});
      leaveLiveSessionGroup(connection, liveSessionId).catch(() => {});
      connection.stop();
    };
  }, [bandId, liveSessionId]);

  const handleNext = async () => {
    try { await nextLiveSessionSong(bandId, liveSessionId); await refreshSession(); }
    catch (e) { setError(String(e)); }
  };

  const handlePrevious = async () => {
    try { await previousLiveSessionSong(bandId, liveSessionId); await refreshSession(); }
    catch (e) { setError(String(e)); }
  };

  const handleEnd = async () => {
    try {
      await endLiveSession(bandId, liveSessionId);
      onExit();
    } catch (e) { setError(String(e)); }
  };

  useEffect(() => {
    function onKeyDown(event: KeyboardEvent): void {
      if (event.key === 'Escape') {
        event.preventDefault();
        onExit();
        return;
      }
      if (!session?.canControl || session.status === 'ended') return;
      if (event.key === 'ArrowRight') {
        event.preventDefault();
        if (session.currentPositionIndex < session.totalSongs - 1) handleNext();
      }
      if (event.key === 'ArrowLeft') {
        event.preventDefault();
        if (session.currentPositionIndex > 0) handlePrevious();
      }
    }
    window.addEventListener('keydown', onKeyDown);
    return () => window.removeEventListener('keydown', onKeyDown);
  }, [session, onExit]);

  if (loading) {
    return (
      <AppShell backgroundImageSrc={loginBackground}>
        <div className="flex h-full items-center justify-center text-sm uppercase tracking-[0.3em] text-zinc-400">
          Loading live ritual...
        </div>
      </AppShell>
    );
  }

  if (error) {
    return (
      <AppShell backgroundImageSrc={loginBackground}>
        <div className="flex h-full flex-col items-center justify-center gap-4 p-8">
          <p className="text-sm uppercase tracking-[0.3em] text-red-400">Error</p>
          <p className="text-zinc-300">{error}</p>
          <RitualButton onClick={onExit} variant="neutral">Exit</RitualButton>
        </div>
      </AppShell>
    );
  }

  if (!session) {
    return (
      <AppShell backgroundImageSrc={loginBackground}>
        <div className="flex h-full items-center justify-center text-sm uppercase tracking-[0.3em] text-zinc-400">
          Session not found.
        </div>
      </AppShell>
    );
  }

  const ended = session.status === 'ended';
  const currentNotes = session.currentSong?.notes ?? session.currentSong?.performanceNotes;
  const nextNotes = session.nextSong?.notes ?? session.nextSong?.performanceNotes ?? session.nextSong?.transitionNotes ?? undefined;

  return (
    <AppShell
      backgroundImageSrc={loginBackground}
      centerSlot={
        <div className="leading-tight">
          <h1 className="text-xl font-bold uppercase tracking-[0.2em] text-zinc-100">Live Ritual</h1>
          <p className="mt-1 text-xs font-semibold uppercase tracking-[0.2em] text-zinc-400">{session.setlistName}</p>
        </div>
      }
      onLogoClick={onExit}
    >
      <div className="h-full overflow-y-auto bg-[radial-gradient(circle_at_15%_-10%,rgba(127,29,29,0.32),transparent_42%),radial-gradient(circle_at_90%_0%,rgba(39,39,42,0.4),transparent_40%),transparent] text-zinc-100">
        <div className="mx-auto flex min-h-full max-w-7xl flex-col px-6 py-8 lg:px-10">

          {!session.canControl && (
            <div className="mb-4 flex items-center gap-2 rounded-xl border border-zinc-700/60 bg-zinc-900/50 px-4 py-2 text-xs uppercase tracking-[0.25em] text-zinc-400">
              <span>👁</span>
              <span>Viewer Mode — waiting for Ritual Leader to advance</span>
            </div>
          )}

          {ended ? (
            <RitualCard className="flex flex-1 flex-col items-center justify-center gap-6 rounded-2xl px-8 py-16 text-center">
              <p className="text-sm uppercase tracking-[0.35em] text-red-500">Ritual Ended</p>
              <h2 className="max-w-2xl text-3xl font-semibold text-zinc-100">The session has concluded.</h2>
              <RitualButton onClick={onExit} variant="danger" size="lg">
                Return to Dashboard
              </RitualButton>
            </RitualCard>
          ) : (
            <>
              <CeremonyProgress
                currentIndex={session.currentPositionIndex}
                totalSongs={session.totalSongs}
                totalDurationLabel={formatMmSs(session.totalDurationSeconds)}
              />

              <CeremonySongCard
                currentTitle={session.currentSong?.title ?? '—'}
                currentDurationLabel={formatMmSs(session.currentSong?.durationSeconds)}
                currentTuning={session.currentSong?.tuning ?? undefined}
                currentNotes={currentNotes ?? undefined}
                nextTitle={session.nextSong ? session.nextSong.title : 'End of Setlist'}
                nextNotes={nextNotes}
                currentIndex={session.currentPositionIndex}
                totalSongs={session.totalSongs}
              />

              {session.canControl && (
                <CeremonyControls
                  isFirstSong={session.currentPositionIndex === 0}
                  isLastSong={session.currentPositionIndex >= session.totalSongs - 1}
                  onPrevious={handlePrevious}
                  onNext={handleNext}
                  onEnd={handleEnd}
                />
              )}
            </>
          )}
        </div>
      </div>
    </AppShell>
  );
}
