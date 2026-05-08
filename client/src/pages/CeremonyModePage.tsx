import { useEffect, useState } from 'react';
import loginBackground from '../assets/login-background.png';
import { AppShell } from '../components/AppShell';
import type { SetlistDetails } from '../types/setlist';
import { CeremonyControls } from '../components/CeremonyControls';
import { CeremonyProgress } from '../components/CeremonyProgress';
import { CeremonySongCard } from '../components/CeremonySongCard';
import { RitualButton } from '../components/ui/RitualButton';
import { RitualCard } from '../components/ui/RitualCard';

interface CeremonyModePageProps {
  setlist: SetlistDetails;
  bandName?: string;
  onEnd: () => void;
}

function formatMmSs(totalSeconds?: number): string {
  if (!totalSeconds || totalSeconds <= 0) return '--:--';
  const minutes = Math.floor(totalSeconds / 60);
  const seconds = totalSeconds % 60;
  return `${minutes}:${seconds.toString().padStart(2, '0')}`;
}


export function CeremonyModePage({ setlist, bandName, onEnd }: CeremonyModePageProps) {
  const [currentIndex, setCurrentIndex] = useState(0);

  const totalSongs = setlist.songs.length;
  const isEmpty = totalSongs === 0;
  const safeIndex = Math.min(currentIndex, Math.max(totalSongs - 1, 0));
  const currentSetlistSong = setlist.songs[safeIndex];
  const nextSong = safeIndex + 1 < totalSongs ? setlist.songs[safeIndex + 1] : null;
  const currentDuration = currentSetlistSong?.durationSeconds;
  const currentTuning = currentSetlistSong?.tuning;
  const currentNotes =
    currentSetlistSong?.notes ??
    currentSetlistSong?.performanceNotes ??
    currentSetlistSong?.transitionNotes;

  const nextNotes =
    nextSong?.notes ??
    nextSong?.performanceNotes ??
    nextSong?.transitionNotes;

  const totalSetlistSeconds = setlist.totalDurationSeconds;
  const isFirstSong = safeIndex === 0;
  const isLastSong = safeIndex === totalSongs - 1;

  useEffect(() => {
    setCurrentIndex(0);
  }, [setlist.setlistId]);

  useEffect(() => {
    function onKeyDown(event: KeyboardEvent): void {
      if (event.key === 'Escape') {
        event.preventDefault();
        onEnd();
        return;
      }

      if (isEmpty) return;

      if (event.key === 'ArrowRight') {
        event.preventDefault();
        setCurrentIndex(index => Math.min(index + 1, totalSongs - 1));
      }

      if (event.key === 'ArrowLeft') {
        event.preventDefault();
        setCurrentIndex(index => Math.max(index - 1, 0));
      }
    }

    window.addEventListener('keydown', onKeyDown);
    return () => window.removeEventListener('keydown', onKeyDown);
  }, [isEmpty, onEnd, totalSongs]);

  return (
    <AppShell
      selectedBandName={bandName}
      backgroundImageSrc={loginBackground}
      centerSlot={
        <div className="leading-tight">
          <h1 className="text-xl font-bold uppercase tracking-[0.2em] text-zinc-100">Live mode</h1>
          <p className="mt-1 text-xs font-semibold uppercase tracking-[0.2em] text-zinc-400">{setlist.name}</p>
        </div>
      }
      onLogoClick={onEnd}
    >
      <div className="h-full overflow-y-auto bg-[radial-gradient(circle_at_15%_-10%,rgba(127,29,29,0.32),transparent_42%),radial-gradient(circle_at_90%_0%,rgba(39,39,42,0.4),transparent_40%),transparent] text-zinc-100">
        <div className="mx-auto flex min-h-full max-w-7xl flex-col px-6 py-8 lg:px-10">
          {isEmpty ? (
            <RitualCard className="flex flex-1 flex-col items-center justify-center gap-6 rounded-2xl px-8 py-16 text-center">
              <p className="text-sm uppercase tracking-[0.35em] text-red-500">Setlist Empty</p>
              <h2 className="max-w-2xl text-3xl font-semibold text-zinc-100">This setlist has no songs yet.</h2>
              <p className="max-w-xl text-zinc-400">Add songs to the setlist in Dashboard to begin Live mode.</p>
              <RitualButton onClick={onEnd} variant="danger" size="lg">
                End Ritual
              </RitualButton>
            </RitualCard>
          ) : (
            <>
              <CeremonyProgress
                currentIndex={safeIndex}
                totalSongs={totalSongs}
                totalDurationLabel={formatMmSs(totalSetlistSeconds)}
              />

              <CeremonySongCard
                currentTitle={currentSetlistSong.title}
                currentDurationLabel={formatMmSs(currentDuration)}
                currentTuning={currentTuning}
                currentNotes={currentNotes}
                nextTitle={nextSong ? nextSong.title : 'End of Setlist'}
                nextNotes={nextNotes}
                currentIndex={safeIndex}
                totalSongs={totalSongs}
              />

              <CeremonyControls
                isFirstSong={isFirstSong}
                isLastSong={isLastSong}
                onPrevious={() => setCurrentIndex(index => Math.max(index - 1, 0))}
                onNext={() => setCurrentIndex(index => Math.min(index + 1, totalSongs - 1))}
                onEnd={onEnd}
              />
            </>
          )}
        </div>
      </div>
    </AppShell>
  );
}