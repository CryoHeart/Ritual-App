import { useEffect, useState } from 'react';
import { getBands } from '../api/bandsApi';
import { getSongs } from '../api/songsApi';
import {
  createSetlist,
  deleteSetlist,
  getSetlistDetails,
  getSetlists,
  updateSetlist
} from '../api/setlistsApi';
import { AppShell } from '../components/AppShell';
import { SetlistsPanel } from '../components/Setlists/SetlistsPanel';
import { SongList } from '../components/SongList';
import { StatCard } from '../components/StatCard';
import { RitualBadge } from '../components/ui/RitualBadge';
import { RitualButton } from '../components/ui/RitualButton';
import { RitualCard } from '../components/ui/RitualCard';
import { SectionTitle } from '../components/ui/SectionTitle';
import loginBackground from '../assets/login-background.png';
import { useAuth } from '../context/AuthContext';
import type { Band } from '../types/band';
import type {
  CreateSetlistRequest,
  SetlistDetails,
  SetlistSummary,
  UpdateSetlistRequest
} from '../types/setlist';
import type { Song } from '../types/song';
import { CeremonyModePage } from './CeremonyModePage';
import { SetlistEditorPage } from './SetlistEditorPage';
import { SongManagementPage } from './SongManagementPage';

export function DashboardPage() {
  const { user } = useAuth();
  const [bands, setBands] = useState<Band[]>([]);
  const [selectedBand, setSelectedBand] = useState<Band | null>(null);
  const [songs, setSongs] = useState<Song[]>([]);
  const [setlists, setSetlists] = useState<SetlistSummary[]>([]);
  const [selectedSetlistId, setSelectedSetlistId] = useState<string | null>(null);

  const [selectedSetlistIdForEdit, setSelectedSetlistIdForEdit] = useState<string | null>(null);
  const [ceremonySetlist, setCeremonySetlist] = useState<SetlistDetails | null>(null);
  const [songManagementOpen, setSongManagementOpen] = useState(false);

  const [initialLoading, setInitialLoading] = useState(true);
  const [bandLoading, setBandLoading] = useState(false);
  const [setlistsLoading, setSetlistsLoading] = useState(false);
  const [fatalError, setFatalError] = useState<string | null>(null);
  const [setlistsPanelMessage, setSetlistsPanelMessage] = useState<string | null>(null);

  useEffect(() => {
    getBands(user?.userId)
      .then(data => {
        setBands(data);
        if (data.length > 0) setSelectedBand(data[0]);
      })
      .catch(e => setFatalError(e instanceof Error ? e.message : 'Failed to load bands.'))
      .finally(() => setInitialLoading(false));
  }, [user?.userId]);

  useEffect(() => {
    if (!selectedBand) return;

    setBandLoading(true);
    setFatalError(null);
    setSetlistsPanelMessage(null);
    setCeremonySetlist(null);
    setSelectedSetlistIdForEdit(null);
    setSongManagementOpen(false);
    setSelectedSetlistId(null);
    setSongs([]);
    setSetlists([]);

    Promise.all([getSongs(selectedBand.id), getSetlists(selectedBand.id)])
      .then(([songsData, setlistsData]) => {
        setSongs(songsData);
        setSetlists(setlistsData);
        setSelectedSetlistId(setlistsData[0]?.setlistId ?? null);
      })
      .catch(e => setFatalError(e instanceof Error ? e.message : 'Failed to load band data.'))
      .finally(() => setBandLoading(false));
  }, [selectedBand]);

  const refreshSetlists = async (bandId: string, preferredSelection?: string | null) => {
    const updated = await getSetlists(bandId);
    setSetlists(updated);

    const selectionCandidate = preferredSelection ?? selectedSetlistId;
    if (selectionCandidate && updated.some(s => s.setlistId === selectionCandidate)) {
      setSelectedSetlistId(selectionCandidate);
    } else {
      setSelectedSetlistId(updated[0]?.setlistId ?? null);
    }

    return updated;
  };

  const handleBeginRitual = async (setlistId: string) => {
    if (!selectedBand) return;

    setSelectedSetlistId(setlistId);
    setSetlistsPanelMessage(null);

    const summary = setlists.find(s => s.setlistId === setlistId);
    if (summary && summary.totalSongs === 0) {
      setSetlistsPanelMessage('This ritual has no songs yet. Add songs in the editor first.');
      return;
    }

    setSetlistsLoading(true);
    try {
      const details = await getSetlistDetails(selectedBand.id, setlistId);
      setCeremonySetlist(details);
    } catch (e) {
      setSetlistsPanelMessage(e instanceof Error ? e.message : 'Failed to load setlist for ceremony.');
    } finally {
      setSetlistsLoading(false);
    }
  };

  const handleEndRitual = () => {
    setCeremonySetlist(null);
  };

  const handleEditSetlistSongs = (setlistId: string) => {
    setSelectedSetlistId(setlistId);
    setSelectedSetlistIdForEdit(setlistId);
  };

  const handleBackFromEditor = () => {
    const editedSetlistId = selectedSetlistIdForEdit;
    setSelectedSetlistIdForEdit(null);

    if (selectedBand) {
      refreshSetlists(selectedBand.id, editedSetlistId)
        .catch(() => {});
    }
  };

  const handleBackFromSongManagement = () => {
    setSongManagementOpen(false);
    // Refresh song count
    if (selectedBand) {
      Promise.all([getSongs(selectedBand.id), getSetlists(selectedBand.id)])
        .then(([songsData, setlistsData]) => {
          setSongs(songsData);
          setSetlists(setlistsData);
        })
        .catch(() => {});
    }
  };

  const handleCreateSetlist = async (payload: CreateSetlistRequest) => {
    if (!selectedBand) return;

    setSetlistsLoading(true);
    setSetlistsPanelMessage(null);
    try {
      const created = await createSetlist(selectedBand.id, payload);
      await refreshSetlists(selectedBand.id, created.setlistId);
    } catch (e) {
      throw e instanceof Error ? e : new Error('Failed to create setlist.');
    } finally {
      setSetlistsLoading(false);
    }
  };

  const handleUpdateSetlist = async (setlistId: string, payload: UpdateSetlistRequest) => {
    if (!selectedBand) return;

    setSetlistsLoading(true);
    setSetlistsPanelMessage(null);
    try {
      const updated = await updateSetlist(selectedBand.id, setlistId, payload);
      const preferred = selectedSetlistId ?? updated.setlistId;
      await refreshSetlists(selectedBand.id, preferred);
    } catch (e) {
      throw e instanceof Error ? e : new Error('Failed to update setlist.');
    } finally {
      setSetlistsLoading(false);
    }
  };

  const handleDeleteSetlist = async (setlistId: string) => {
    if (!selectedBand) return;

    setSetlistsLoading(true);
    setSetlistsPanelMessage(null);
    try {
      await deleteSetlist(selectedBand.id, setlistId);
      const updated = await getSetlists(selectedBand.id);
      setSetlists(updated);

      if (selectedSetlistId === setlistId) {
        setSelectedSetlistId(updated[0]?.setlistId ?? null);
      } else if (selectedSetlistId && !updated.some(s => s.setlistId === selectedSetlistId)) {
        setSelectedSetlistId(updated[0]?.setlistId ?? null);
      }
    } catch (e) {
      throw e instanceof Error ? e : new Error('Failed to delete setlist.');
    } finally {
      setSetlistsLoading(false);
    }
  };

  if (songManagementOpen && selectedBand) {
    return (
      <SongManagementPage
        bandId={selectedBand.id}
        bandName={selectedBand.name}
        onBack={handleBackFromSongManagement}
      />
    );
  }

  if (ceremonySetlist) {
    return (
      <CeremonyModePage
        setlist={ceremonySetlist}
        bandName={selectedBand?.name}
        onEnd={handleEndRitual}
      />
    );
  }

  if (selectedSetlistIdForEdit && selectedBand) {
    return (
      <SetlistEditorPage
        bandId={selectedBand.id}
        bandName={selectedBand.name}
        setlistId={selectedSetlistIdForEdit}
        onBack={handleBackFromEditor}
      />
    );
  }

  if (initialLoading) {
    return (
      <AppShell selectedBandName={selectedBand?.name} backgroundImageSrc={loginBackground}>
        <div className="mx-auto flex min-h-[calc(100vh-72px)] w-full max-w-7xl items-center justify-center px-6 lg:px-10">
          <RitualCard className="w-full max-w-xl text-center">
            <p className="text-xs uppercase tracking-[0.32em] text-zinc-500">Initializing</p>
            <p className="mt-3 text-lg font-semibold text-zinc-200">Preparing stage control data...</p>
            <div className="mx-auto mt-5 h-1.5 w-32 overflow-hidden rounded-full bg-zinc-800">
              <div className="ritual-pulse h-full w-1/2 rounded-full bg-red-600" />
            </div>
          </RitualCard>
        </div>
      </AppShell>
    );
  }

  if (fatalError) {
    return (
      <AppShell selectedBandName={selectedBand?.name} backgroundImageSrc={loginBackground}>
        <div className="mx-auto flex min-h-[calc(100vh-72px)] w-full max-w-7xl items-center justify-center px-6 lg:px-10">
          <RitualCard className="w-full max-w-xl border-red-900/60 text-center">
            <RitualBadge tone="danger">Error</RitualBadge>
            <p className="mt-4 text-xl font-semibold text-zinc-100">Connection to control data failed</p>
            <p className="mt-2 text-sm text-red-300">{fatalError}</p>
            <RitualButton onClick={() => window.location.reload()} variant="danger" className="mt-6">
              Retry
            </RitualButton>
          </RitualCard>
        </div>
      </AppShell>
    );
  }

  if (bands.length === 0) {
    return (
      <AppShell backgroundImageSrc={loginBackground}>
        <div className="mx-auto flex min-h-[calc(100vh-72px)] w-full max-w-7xl items-center justify-center px-6 lg:px-10">
          <RitualCard className="w-full max-w-xl text-center">
            <p className="text-xs uppercase tracking-[0.32em] text-zinc-500">No Band Data</p>
            <p className="mt-3 text-lg font-semibold text-zinc-100">No band found for your account.</p>
            <p className="mt-2 text-sm text-zinc-400">Your band workspace will appear here once it has been set up.</p>
          </RitualCard>
        </div>
      </AppShell>
    );
  }

  const bandSelector = bands.length > 1 ? (
    <label className="flex items-center gap-2 text-xs uppercase tracking-[0.18em] text-zinc-500">
      Band
      <select
        value={selectedBand?.id ?? ''}
        onChange={e => {
          const band = bands.find(b => b.id === e.target.value);
          if (band) setSelectedBand(band);
        }}
        className="rounded-lg border border-zinc-700 bg-zinc-900 px-3 py-2 text-xs font-semibold uppercase tracking-[0.12em] text-zinc-100 focus:border-red-600 focus:outline-none"
      >
        {bands.map(b => (
          <option key={b.id} value={b.id}>{b.name}</option>
        ))}
      </select>
    </label>
  ) : null;

  return (
    <AppShell selectedBandName={selectedBand?.name} rightSlot={bandSelector} backgroundImageSrc={loginBackground}>
      <div className="mx-auto flex h-full w-full max-w-6xl flex-col gap-4 overflow-hidden px-4 py-4 lg:px-8">
        <RitualCard className="overflow-hidden border-red-900/35 bg-[radial-gradient(circle_at_top,rgba(127,29,29,0.28),rgba(10,10,10,0.96)_56%)]">
          <div className="flex flex-col gap-5 lg:flex-row lg:items-stretch lg:justify-between">
            <div className="lg:flex lg:flex-1 lg:items-center">
              <div>
                <p className="text-2xl font-black uppercase tracking-[0.12em] text-zinc-100 sm:text-4xl">{selectedBand?.name ?? 'Band'}</p>
                <p className="mt-2 text-xl font-black uppercase tracking-[0.16em] text-red-300 sm:text-2xl">Command Center</p>
                <p className="mt-2 max-w-2xl text-sm text-zinc-300">
                  {selectedBand?.description || "Control songs, setlists, and ceremony flow for tonight's performance."}
                </p>
              </div>
            </div>

            <div className="w-full lg:w-[420px] lg:shrink-0">
              <div className="grid w-full gap-3 sm:grid-cols-2">
                <StatCard label="Songs" value={songs.length} />
                <StatCard label="Setlists" value={setlists.length} accent />
              </div>

              {selectedBand && (
                <RitualButton
                  variant="primary"
                  className="mt-3 w-full"
                  onClick={() => setSongManagementOpen(true)}
                >
                  Manage songs and albums
                </RitualButton>
              )}
            </div>
          </div>
        </RitualCard>

        {bandLoading ? (
          <RitualCard className="py-16 text-center">
            <p className="text-xs uppercase tracking-[0.3em] text-zinc-500">Loading Band Context</p>
            <p className="mt-3 text-lg text-zinc-300">Syncing songs and setlists...</p>
            <div className="mx-auto mt-6 h-1.5 w-44 overflow-hidden rounded-full bg-zinc-800">
              <div className="ritual-pulse h-full w-1/2 rounded-full bg-red-600" />
            </div>
          </RitualCard>
        ) : (
          <>
            <div className="grid min-h-0 flex-1 grid-cols-1 gap-4 md:grid-cols-2">
              <RitualCard padded={false} className="flex min-h-0 flex-col overflow-hidden">
                <div className="border-b border-zinc-800 px-5 py-4">
                  <SectionTitle
                    eyebrow="Library"
                    title="Songs"
                    subtitle="Title, tuning, and duration"
                    className="items-end"
                  />
                </div>
                <div className="min-h-0 flex-1 overflow-y-auto">
                  <SongList songs={songs} />
                </div>
              </RitualCard>

              <RitualCard padded={false} className="flex min-h-0 flex-col overflow-hidden">
                <div className="border-b border-zinc-800 px-5 py-4">
                  <SectionTitle
                    eyebrow="Performance"
                    title="Setlists"
                    subtitle="Create, edit, arrange, and launch rituals"
                    className="items-end"
                  />
                </div>
                <div className="min-h-0 flex-1">
                  <SetlistsPanel
                    setlists={setlists}
                    selectedSetlistId={selectedSetlistId}
                    isLoading={setlistsLoading}
                    panelMessage={setlistsPanelMessage}
                    onCreateSetlist={handleCreateSetlist}
                    onUpdateSetlist={handleUpdateSetlist}
                    onDeleteSetlist={handleDeleteSetlist}
                    onEditSetlistSongs={handleEditSetlistSongs}
                    onBeginRitual={handleBeginRitual}
                  />
                </div>
              </RitualCard>
            </div>
          </>
        )}
      </div>
    </AppShell>
  );
}
