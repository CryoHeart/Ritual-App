import type { ButtonHTMLAttributes, ReactNode } from 'react';

interface RitualButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  children: ReactNode;
  variant?: 'primary' | 'neutral' | 'ghost' | 'danger';
  size?: 'sm' | 'md' | 'lg';
}

const variantClasses = {
  primary:
    'border-red-700/90 bg-[linear-gradient(180deg,rgba(185,28,28,1),rgba(127,29,29,1))] text-white shadow-[0_10px_28px_rgba(120,16,16,0.55)] hover:border-red-500 hover:brightness-110',
  neutral:
    'border-zinc-700 bg-zinc-900 text-zinc-100 hover:border-zinc-500 hover:bg-zinc-800',
  ghost:
    'border-zinc-800 bg-zinc-950/70 text-zinc-300 hover:border-zinc-600 hover:text-zinc-100',
  danger:
    'border-red-800 bg-red-950/70 text-red-100 hover:border-red-600 hover:bg-red-900/80'
} as const;

const sizeClasses = {
  sm: 'px-3 py-1.5 text-xs tracking-[0.14em]',
  md: 'px-4 py-2.5 text-sm tracking-[0.18em]',
  lg: 'px-6 py-4 text-sm tracking-[0.24em]'
} as const;

export function RitualButton({
  children,
  className,
  variant = 'neutral',
  size = 'md',
  ...buttonProps
}: RitualButtonProps) {
  return (
    <button
      {...buttonProps}
      className={`inline-flex items-center justify-center rounded-xl border font-semibold uppercase transition-all duration-200 active:scale-[0.99] disabled:cursor-not-allowed disabled:opacity-45 ${variantClasses[variant]} ${sizeClasses[size]} ${className ?? ''}`.trim()}
    >
      {children}
    </button>
  );
}