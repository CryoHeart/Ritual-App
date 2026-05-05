import type { ReactNode } from 'react';

interface SectionTitleProps {
  eyebrow?: string;
  title: string;
  subtitle?: string;
  action?: ReactNode;
  className?: string;
}

export function SectionTitle({ eyebrow, title, subtitle, action, className = '' }: SectionTitleProps) {
  return (
    <div className={`flex items-start justify-between gap-4 ${className}`.trim()}>
      <div>
        {eyebrow && (
          <p className="text-[10px] font-semibold uppercase tracking-[0.36em] text-red-500">{eyebrow}</p>
        )}
        <h2 className="mt-1.5 text-xl font-semibold tracking-tight text-zinc-100 sm:text-2xl">{title}</h2>
        {subtitle && <p className="mt-1.5 max-w-2xl text-sm text-zinc-400">{subtitle}</p>}
      </div>
      {action && <div className="shrink-0">{action}</div>}
    </div>
  );
}