import type { ReactNode } from 'react';

interface RitualCardProps {
  children: ReactNode;
  className?: string;
  padded?: boolean;
}

export function RitualCard({ children, className = '', padded = true }: RitualCardProps) {
  return (
    <div
      className={`rounded-2xl border border-zinc-800/80 bg-[linear-gradient(180deg,rgba(30,30,32,0.9),rgba(17,17,19,0.94))] shadow-[0_16px_48px_rgba(0,0,0,0.45)] ${padded ? 'p-5 sm:p-6' : ''} ${className}`.trim()}
    >
      {children}
    </div>
  );
}