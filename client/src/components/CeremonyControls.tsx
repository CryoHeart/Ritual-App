import { RitualBadge } from './ui/RitualBadge';
import { RitualButton } from './ui/RitualButton';

interface CeremonyControlsProps {
  isFirstSong: boolean;
  isLastSong: boolean;
  onPrevious: () => void;
  onNext: () => void;
  onEnd: () => void;
}

export function CeremonyControls({ isFirstSong, isLastSong, onPrevious, onNext, onEnd }: CeremonyControlsProps) {
  return (
    <footer className="mt-8">
      <div className="mb-3 flex flex-wrap items-center gap-2 text-[10px] uppercase tracking-[0.2em] text-zinc-400">
        <RitualBadge>← Previous</RitualBadge>
        <RitualBadge>→ Next</RitualBadge>
        <RitualBadge tone="accent">Esc End</RitualBadge>
      </div>

      <div className="grid grid-cols-1 gap-3 sm:grid-cols-3">
        <RitualButton onClick={onPrevious} disabled={isFirstSong} size="lg" variant="neutral">
          Previous
        </RitualButton>
        <RitualButton onClick={onEnd} size="lg" variant="danger">
          End Ritual
        </RitualButton>
        <RitualButton onClick={onNext} disabled={isLastSong} size="lg" variant="neutral">
          Next
        </RitualButton>
      </div>
    </footer>
  );
}