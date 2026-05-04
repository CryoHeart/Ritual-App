interface Props {
  label: string;
  value: string | number;
  accent?: boolean;
}

export function StatCard({ label, value, accent }: Props) {
  return (
    <div className="rounded-xl border border-zinc-800 bg-zinc-900/60 px-6 py-5">
      <p className="text-[10px] font-semibold uppercase tracking-[0.25em] text-zinc-500">
        {label}
      </p>
      <p className={`mt-2 text-3xl font-bold tabular-nums tracking-tight ${accent ? 'text-red-500' : 'text-white'}`}>
        {value}
      </p>
    </div>
  );
}
