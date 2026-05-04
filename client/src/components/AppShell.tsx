import type { ReactNode } from 'react';

interface Props {
  children: ReactNode;
}

export function AppShell({ children }: Props) {
  return (
    <div className="min-h-screen bg-black text-white antialiased">
      {children}
    </div>
  );
}
