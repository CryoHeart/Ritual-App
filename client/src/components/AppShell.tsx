import type { ReactNode } from 'react';
import ritualHeaderCombined from '../assets/ritual-full.png';
import { useAuth } from '../context/AuthContext';

interface Props {
  children: ReactNode;
  selectedBandName?: string;
  rightSlot?: ReactNode;
  centerSlot?: ReactNode;
  backgroundImageSrc?: string;
  onLogoClick?: () => void;
}

export function AppShell({ children, selectedBandName, rightSlot, centerSlot, backgroundImageSrc, onLogoClick }: Props) {
  const { user, logout } = useAuth();
  const bandName = selectedBandName ?? user?.displayName;
  return (
    <div className="relative flex h-screen flex-col overflow-hidden bg-[#090909] text-white antialiased">
      {backgroundImageSrc ? (
        <div
          className="pointer-events-none absolute inset-0 bg-cover bg-center bg-no-repeat opacity-55"
          style={{ backgroundImage: `url(${backgroundImageSrc})` }}
        />
      ) : null}
      <div className="pointer-events-none absolute inset-0">
        <div className="absolute -top-24 left-[-16%] h-64 w-64 rounded-full bg-red-900/20 blur-3xl" />
        <div className="absolute right-[-10%] top-20 h-72 w-72 rounded-full bg-zinc-600/10 blur-3xl" />
      </div>

      <header className="relative z-10 border-b border-zinc-800/80 bg-zinc-950/85 backdrop-blur">
        <div className="relative mx-auto flex w-full max-w-7xl items-center justify-between gap-4 px-6 py-4 lg:px-10">
          {centerSlot ? (
            <div className="pointer-events-none absolute inset-0 flex items-center justify-center px-40 text-center">
              {centerSlot}
            </div>
          ) : null}
          {/* Left: logo + title */}
          <div className="flex items-center">
            {onLogoClick ? (
              <button onClick={onLogoClick} className="focus:outline-none" aria-label="Go to dashboard">
                <img
                  src={ritualHeaderCombined}
                  alt="Ritual"
                  className="h-24 w-[720px] object-contain object-left drop-shadow-[0_0_28px_rgba(239,68,68,0.85)] brightness-125"
                />
              </button>
            ) : (
              <img
                src={ritualHeaderCombined}
                alt="Ritual"
                className="h-24 w-[720px] object-contain object-left drop-shadow-[0_0_28px_rgba(239,68,68,0.85)] brightness-125"
              />
            )}
          </div>

          {/* Right: band badge, extra slot, logout */}
          <div className="flex flex-wrap items-center gap-3">
            {bandName ? (
              <div className="rounded-lg border border-zinc-700 bg-zinc-900 px-4 py-2">
                <p className="text-[9px] font-black uppercase tracking-[0.28em] text-zinc-300">Band</p>
                <p className="text-sm font-bold uppercase tracking-[0.12em] text-zinc-100">{bandName}</p>
              </div>
            ) : null}
            {rightSlot}
            <button
              onClick={logout}
              className="rounded-lg border border-red-800 bg-red-950/60 px-4 py-2 text-sm font-bold uppercase tracking-[0.12em] text-red-300 transition hover:border-red-600 hover:bg-red-900/60 hover:text-red-100"
            >
              Log out
            </button>
          </div>
        </div>
      </header>

      <main className="relative z-10 min-h-0 flex-1 overflow-hidden">{children}</main>
    </div>
  );
}
