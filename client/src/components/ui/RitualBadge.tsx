import type { ReactNode } from 'react';

interface RitualBadgeProps {
  children: ReactNode;
  tone?: 'neutral' | 'accent' | 'danger';
  className?: string;
}

const toneClasses: Record<NonNullable<RitualBadgeProps['tone']>, string> = {
  neutral: 'border-zinc-700/80 bg-zinc-900/80 text-zinc-300',
  accent: 'border-red-900/60 bg-red-950/40 text-red-300',
  danger: 'border-red-700/80 bg-red-900/60 text-red-100'
};

export function RitualBadge({ children, tone = 'neutral', className = '' }: RitualBadgeProps) {
  return (
    <span
      className={`inline-flex items-center rounded-md border px-2.5 py-1 text-[10px] font-semibold uppercase tracking-[0.24em] ${toneClasses[tone]} ${className}`.trim()}
    >
      {children}
    </span>
  );
}