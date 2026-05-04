import { useEffect, useState } from 'react';
import { getBands } from '../api/bandsApi';
import { getSongs } from '../api/songsApi';
import { getSetlists } from '../api/setlistsApi';
import { AppShell } from '../components/AppShell';
import { StatCard } from '../components/StatCard';
import { SongList } from '../components/SongList';
import { SetlistPanel } from '../components/SetlistPanel';
import { CeremonyPreview } from '../components/CeremonyPreview';
import type { Band } from '../types/band';
import type { Song } from '../types/song';
import type { Setlist } from '../types/setlist';

function fmtDuration(setlist: Setlist | null): string {
  if (!setlist || setlist.songs.length === 0) return '—';
  const total = setlist.songs.reduce((sum, s) => sum + (s.durationSeconds ?? 0), 0);
  if (total === 0) return '—';
  const h = Math.floor(total / 3600);
  const m = Math.floor((total % 3600) / 60);
  const s = total % 60;
  if (h > 0) return `${h}h ${m}m`;
  if (m > 0) return `${m}m ${s}s`;
  return `${s}s`;
}

export function DashboardPage() {
  const [bands, setBands] = useState<Band[]>([]);
  const [selectedBand, setSelectedBand] = useState<Band | null>(null);
  const [songs, setSongs] = useState<Song[]>([]);
  const [setlists, setSetlists] = useState<Setlist[]>([]);
  const [selectedSetlist, setSelectedSetlist] = useState<Setlist | null>(null);

  const [initialLoading, setInitialLoading] = useState(true);
  const [bandLoading, setBandLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Load bands on mount
  useEffect(() => {
    getBands()
      .then(data => {
        setBands(data);
        if (data.length > 0) setSelectedBand(data[0]);
      })
      .catch(e => setError(e instanceof Error ? e.message : 'Failed to load bands.'))
      .finally(() => setInitialLoading(false));
  }, []);

  // Load songs + setlists when band changes
  useEffect(() => {
    if (!selectedBand) return;

    setBandLoading(true);
    setSongs([]);
    setSetlists([]);
    setSelectedSetlist(null);

    Promise.all([getSongs(selectedBand.id), getSetlists(selectedBand.id)])
      .then(([songsData, setlistsData]) => {
        setSongs(songsData);
        setSetlists(setlistsData);
        if (setlistsData.length > 0) setSelectedSetlist(setlistsData[0]);
      })
      .catch(e => setError(e instanceof Error ? e.message : 'Failed to load band data.'))
      .finally(() => setBandLoading(false));
  }, [selectedBand]);

  const handleBeginRitual = () => {
    // TODO: Start a live session via POST /api/bands/{bandId}/setlists/{setlistId}/live-sessions/start
    // and navigate to the live session view once routing is added.
    alert('Live session coming soon!');
  };

  // ── Loading ───────────────────────────────────────────────────────────────
  if (initialLoading) {
    return (
      <AppShell>
        <div className="flex min-h-screen items-center justify-center">
          <p className="animate-pulse text-sm text-zinc-600">Loading RITUAL…</p>
        </div>
      </AppShell>
    );
  }

  // ── Error ─────────────────────────────────────────────────────────────────
  if (error) {
    return (
      <AppShell>
        <div className="flex min-h-screen items-center justify-center">
          <div className="text-center">
            <p className="text-sm text-red-500">{error}</p>
            <button
              onClick={() => window.location.reload()}
              className="mt-4 text-xs text-zinc-500 underline underline-offset-2 hover:text-zinc-300"
            >
              Retry
            </button>
          </div>
        </div>
      </AppShell>
    );
  }

  // ── Empty ─────────────────────────────────────────────────────────────────
  if (bands.length === 0) {
    return (
      <AppShell>
        <div className="flex min-h-screen items-center justify-center">
          <p className="text-sm text-zinc-600">No bands found. Add a band to get started.</p>
        </div>
      </AppShell>
    );
  }

  // ── Dashboard ─────────────────────────────────────────────────────────────
  return (
    <AppShell>
      <div className="mx-auto max-w-7xl px-6 py-10">

        {/* Header */}
        <header className="mb-8 flex items-start justify-between gap-4">
          <div>
            <p className="text-[10px] font-bold uppercase tracking-[0.5em] text-red-600">
              Ritual
            </p>
            <h1 className="mt-1 text-3xl font-bold tracking-tight text-white">
              {selectedBand?.name}
            </h1>
            {selectedBand?.description && (
              <p className="mt-1.5 text-sm text-zinc-500">{selectedBand.description}</p>
            )}
          </div>

          {bands.length > 1 && (
            <select
              value={selectedBand?.id ?? ''}
              onChange={e => {
                const band = bands.find(b => b.id === e.target.value);
                if (band) setSelectedBand(band);
              }}
              className="mt-1 rounded-md border border-zinc-700 bg-zinc-900 px-3 py-2 text-sm text-white focus:border-red-600 focus:outline-none"
            >
              {bands.map(b => (
                <option key={b.id} value={b.id}>{b.name}</option>
              ))}
            </select>
          )}
        </header>

        {bandLoading ? (
          <div className="flex items-center justify-center py-32">
            <p className="animate-pulse text-sm text-zinc-600">Loading…</p>
          </div>
        ) : (
          <>
            {/* Stats */}
            <div className="mb-6 grid grid-cols-3 gap-4">
              <StatCard label="Songs" value={songs.length} />
              <StatCard label="Setlists" value={setlists.length} />
              <StatCard label="Setlist Duration" value={fmtDuration(selectedSetlist)} accent />
            </div>

            {/* Two-column layout */}
            <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">

              {/* Songs */}
              <section className="overflow-hidden rounded-xl border border-zinc-800 bg-zinc-900">
                <div className="border-b border-zinc-800 px-5 py-3">
                  <h2 className="text-[10px] font-semibold uppercase tracking-[0.25em] text-zinc-500">
                    Songs
                  </h2>
                </div>
                <div className="max-h-[480px] overflow-y-auto">
                  <SongList songs={songs} />
                </div>
              </section>

              {/* Setlists */}
              <section className="overflow-hidden rounded-xl border border-zinc-800 bg-zinc-900">
                <div className="border-b border-zinc-800 px-5 py-3">
                  <h2 className="text-[10px] font-semibold uppercase tracking-[0.25em] text-zinc-500">
                    Setlists
                  </h2>
                </div>
                <SetlistPanel
                  setlists={setlists}
                  selectedSetlist={selectedSetlist}
                  onSelectSetlist={setSelectedSetlist}
                  onBeginRitual={handleBeginRitual}
                />
              </section>
            </div>

            {/* Ceremony preview */}
            {selectedSetlist && selectedSetlist.songs.length > 0 && (
              <div className="mt-6">
                <CeremonyPreview setlist={selectedSetlist} />
              </div>
            )}
          </>
        )}
      </div>
    </AppShell>
  );
}
