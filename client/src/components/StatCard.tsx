import { RitualCard } from './ui/RitualCard';

interface Props {
  label: string;
  value: string | number;
  accent?: boolean;
}

export function StatCard({ label, value, accent }: Props) {
  return (
    <RitualCard className="h-full">
      <p className="text-[10px] font-semibold uppercase tracking-[0.25em] text-zinc-500">
        {label}
      </p>
      <p className={`mt-3 text-4xl font-semibold tabular-nums tracking-tight ${accent ? 'text-red-400' : 'text-zinc-100'}`}>
        {value}
      </p>
    </RitualCard>
  );
}
