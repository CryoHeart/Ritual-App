import { useMemo, useState } from 'react';
import {
  getMusicBrainzArtistAlbums,
  getMusicBrainzReleaseGroupReleases,
  getMusicBrainzReleaseTracks,
  importMusicBrainzSelection,
  searchMusicBrainzArtists,
  searchMusicBrainzRecordings,
  searchMusicBrainzReleaseGroups
} from '../api/musicbrainzApi';
import { getAlbumsWithSongs } from '../api/albumsApi';
import loginBackground from '../assets/login-background.png';
import { AppShell } from '../components/AppShell';
import { RitualButton } from '../components/ui/RitualButton';
import { RitualCard } from '../components/ui/RitualCard';
import type { AlbumWithSongs } from '../types/album';
import type {
  ImportedAlbumDto,
  ImportedSongDto,
  MusicBrainzArtist,
  MusicBrainzImportSummary,
  MusicBrainzRecording,
  MusicBrainzRelease,
  MusicBrainzReleaseGroup,
  MusicBrainzTrack
} from '../types/musicbrainz';

type SearchType = 'artist' | 'album' | 'song';

type AlbumSelectionState = {
  releaseGroup: MusicBrainzReleaseGroup;
  selected: boolean;
  alreadyImported: boolean;
  releases: MusicBrainzRelease[];
  releaseLoading: boolean;
  releaseError: string | null;
  selectedReleaseId: string | null;
  tracks: MusicBrainzTrack[];
  tracksLoading: boolean;
  tracksError: string | null;
  selectedTrackIds: Record<string, boolean>;
};

interface Props {
  bandId: string;
  bandName?: string;
  userId?: string;
  onBack: () => void;
}

export function MusicBrainzImportComponent({ bandId, bandName, userId, onBack }: Props) {
  const [searchType, setSearchType] = useState<SearchType>('artist');
  const [query, setQuery] = useState(bandName ?? '');
  const [searchLoading, setSearchLoading] = useState(false);
  const [searchError, setSearchError] = useState<string | null>(null);

  const [artistResults, setArtistResults] = useState<MusicBrainzArtist[]>([]);
  const [releaseGroupResults, setReleaseGroupResults] = useState<MusicBrainzReleaseGroup[]>([]);
  const [recordingResults, setRecordingResults] = useState<MusicBrainzRecording[]>([]);

  const [selectedArtist, setSelectedArtist] = useState<MusicBrainzArtist | null>(null);
  const [albumsLoading, setAlbumsLoading] = useState(false);
  const [albumsError, setAlbumsError] = useState<string | null>(null);
  const [albums, setAlbums] = useState<AlbumSelectionState[]>([]);

  const [existingLibrary, setExistingLibrary] = useState<AlbumWithSongs[]>([]);
  const [libraryLoading, setLibraryLoading] = useState(false);
  const [libraryError, setLibraryError] = useState<string | null>(null);

  const [importLoading, setImportLoading] = useState(false);
  const [importError, setImportError] = useState<string | null>(null);
  const [importSummary, setImportSummary] = useState<MusicBrainzImportSummary | null>(null);

  const existingRecordingIds = useMemo(
    () =>
      new Set(
        existingLibrary
          .flatMap(a => a.songs)
          .map(s => s.musicBrainzRecordingId)
          .filter((id): id is string => Boolean(id))
      ),
    [existingLibrary]
  );

  const existingSongTitleKeys = useMemo(
    () => new Set(existingLibrary.flatMap(a => a.songs).map(s => normalizeText(s.title)).filter(Boolean)),
    [existingLibrary]
  );

  const selectedAlbumCount = albums.filter(a => a.selected && !a.alreadyImported).length;

  const selectedTrackCount = albums.reduce((sum, album) => {
    const count = Object.values(album.selectedTrackIds).filter(Boolean).length;
    return sum + count;
  }, 0);

  const canImport = Boolean(userId && selectedArtist && selectedAlbumCount > 0 && !importLoading);

  const reloadExistingLibrary = async (): Promise<AlbumWithSongs[]> => {
    setLibraryLoading(true);
    setLibraryError(null);
    try { 
      const data = await getAlbumsWithSongs(bandId);
      setExistingLibrary(data);
      return data;
    } catch (error) {
      setLibraryError(error instanceof Error ? error.message : 'Failed to load existing songs and albums.');
      return [];
    } finally {
      setLibraryLoading(false);
    }
  };

  const runSearch = async () => {
    const trimmed = query.trim();
    if (!trimmed) {
      setSearchError('Enter a search query.');
      return;
    }

    setSearchLoading(true);
    setSearchError(null);
    setImportSummary(null);
    setArtistResults([]);
    setReleaseGroupResults([]);
    setRecordingResults([]);

    try {
      if (searchType === 'artist') {
        setArtistResults(await searchMusicBrainzArtists(trimmed));
      } else if (searchType === 'album') {
        setReleaseGroupResults(await searchMusicBrainzReleaseGroups(trimmed));
      } else {
        setRecordingResults(await searchMusicBrainzRecordings(trimmed));
      }
    } catch (error) {
      setSearchError(error instanceof Error ? error.message : 'Search failed.');
    } finally {
      setSearchLoading(false);
    }
  };

  const selectArtist = async (artist: MusicBrainzArtist) => {
    setSelectedArtist(artist);
    setAlbums([]);
    setAlbumsError(null);
    setImportError(null);
    setImportSummary(null);
    const library = await reloadExistingLibrary();
    await loadArtistAlbums(artist, library);
  };

  const loadArtistAlbums = async (artist: MusicBrainzArtist, librarySnapshot?: AlbumWithSongs[]) => {
    setAlbumsLoading(true);
    setAlbumsError(null);
    try {
      const library = librarySnapshot ?? existingLibrary;
      const albumIds = new Set(
        library
          .map(a => a.musicBrainzReleaseGroupId)
          .filter((id): id is string => Boolean(id))
      );
      const albumTitleKeys = new Set(library.map(a => normalizeText(a.title)).filter(Boolean));

      const releaseGroups = await getMusicBrainzArtistAlbums(artist.id);
      const mapped: AlbumSelectionState[] = releaseGroups.map(rg => ({
        releaseGroup: rg,
        selected: false,
        alreadyImported: isAlbumDuplicate(rg, albumIds, albumTitleKeys),
        releases: [],
        releaseLoading: false,
        releaseError: null,
        selectedReleaseId: null,
        tracks: [],
        tracksLoading: false,
        tracksError: null,
        selectedTrackIds: {}
      }));
      setAlbums(mapped);
    } catch (error) {
      setAlbumsError(error instanceof Error ? error.message : 'Failed to load artist albums.');
    } finally {
      setAlbumsLoading(false);
    }
  };

  const toggleAlbumSelection = (releaseGroupId: string, selected: boolean) => {
    setAlbums(prev =>
      prev.map(album => {
        if (album.releaseGroup.id !== releaseGroupId || album.alreadyImported) {
          return album;
        }

        if (!selected) {
          return { ...album, selected: false };
        }

        const defaultTracks =
          album.tracks.length === 0
            ? album.selectedTrackIds
            : Object.fromEntries(
                album.tracks
                  .filter(track => !isTrackDuplicate(track, existingRecordingIds, existingSongTitleKeys))
                  .map(track => [track.id, true])
              );

        return {
          ...album,
          selected: true,
          selectedTrackIds: defaultTracks
        };
      })
    );

    // Auto-load tracks when selecting an album that hasn't fetched them yet.
    // loadAlbumTracks will then mark all non-duplicate tracks as selected.
    if (selected) {
      const target = albums.find(a => a.releaseGroup.id === releaseGroupId);
      if (target && !target.alreadyImported && target.tracks.length === 0 && !target.releaseLoading) {
        loadAlbumTracks(releaseGroupId);
      }
    }
  };

  const loadAlbumTracks = async (releaseGroupId: string, releaseIdOverride?: string) => {
    const target = albums.find(a => a.releaseGroup.id === releaseGroupId);
    if (!target) {
      return;
    }

    setAlbums(prev =>
      prev.map(album =>
        album.releaseGroup.id === releaseGroupId
          ? { ...album, releaseLoading: true, releaseError: null, tracksError: null }
          : album
      )
    );

    try {
      const releases = await getMusicBrainzReleaseGroupReleases(releaseGroupId);
      const selectedReleaseId = releaseIdOverride ?? target.selectedReleaseId ?? releases[0]?.id ?? null;

      let tracks: MusicBrainzTrack[] = [];
      if (selectedReleaseId) {
        tracks = await getMusicBrainzReleaseTracks(selectedReleaseId);
      }

      const selectedTrackIds = Object.fromEntries(
        tracks
          .filter(track => !isTrackDuplicate(track, existingRecordingIds, existingSongTitleKeys))
          .map(track => [track.id, true])
      );

      setAlbums(prev =>
        prev.map(album =>
          album.releaseGroup.id === releaseGroupId
            ? {
                ...album,
                releases,
                releaseLoading: false,
                selectedReleaseId,
                tracks,
                tracksLoading: false,
                selectedTrackIds
              }
            : album
        )
      );
    } catch (error) {
      setAlbums(prev =>
        prev.map(album =>
          album.releaseGroup.id === releaseGroupId
            ? {
                ...album,
                releaseLoading: false,
                tracksLoading: false,
                releaseError: error instanceof Error ? error.message : 'Failed to load release/track data.'
              }
            : album
        )
      );
    }
  };

  const setReleaseForAlbum = async (releaseGroupId: string, releaseId: string) => {
    setAlbums(prev =>
      prev.map(album =>
        album.releaseGroup.id === releaseGroupId
          ? { ...album, selectedReleaseId: releaseId, tracksLoading: true, tracksError: null }
          : album
      )
    );

    await loadAlbumTracks(releaseGroupId, releaseId);
  };

  const toggleTrackSelection = (releaseGroupId: string, trackId: string, selected: boolean) => {
    setAlbums(prev =>
      prev.map(album =>
        album.releaseGroup.id === releaseGroupId
          ? {
              ...album,
              selectedTrackIds: {
                ...album.selectedTrackIds,
                [trackId]: selected
              }
            }
          : album
      )
    );
  };

  const importSelected = async () => {
    if (!selectedArtist || !userId) {
      setImportError('You must be logged in to import data.');
      return;
    }

    const selectedAlbums = buildImportAlbums(albums, existingRecordingIds, existingSongTitleKeys);
    if (selectedAlbums.length === 0) {
      setImportError('No importable albums or songs selected.');
      return;
    }

    setImportLoading(true);
    setImportError(null);
    setImportSummary(null);

    try {
      const summary = await importMusicBrainzSelection({
        userId,
        bandId,
        artist: {
          id: selectedArtist.id,
          name: selectedArtist.name,
          country: selectedArtist.country
        },
        albums: selectedAlbums
      });

      setImportSummary(summary);
      const updatedLibrary = await reloadExistingLibrary();
      await loadArtistAlbums(selectedArtist, updatedLibrary);
    } catch (error) {
      setImportError(error instanceof Error ? error.message : 'Import failed.');
    } finally {
      setImportLoading(false);
    }
  };

  return (
    <AppShell
      selectedBandName={bandName}
      backgroundImageSrc={loginBackground}
      centerSlot={<h1 className="text-xl font-bold uppercase tracking-[0.2em] text-zinc-100">MusicBrainz import</h1>}
      onLogoClick={onBack}
    >
      <div className="mx-auto flex h-full w-full max-w-6xl flex-col gap-4 px-4 py-4 lg:px-8">
        <div className="flex justify-end">
          <button
            onClick={onBack}
            className="rounded-lg border border-zinc-700 bg-zinc-900/70 px-4 py-2 text-xs font-semibold uppercase tracking-[0.2em] text-zinc-200 transition hover:border-zinc-500 hover:text-white"
          >
            Back
          </button>
        </div>

        <RitualCard>
          <h2 className="text-sm font-black uppercase tracking-[0.2em] text-zinc-200">Search MusicBrainz</h2>
          <p className="mt-2 text-sm text-zinc-500">Search by artist, album, or song. Then select an artist to import into this band account.</p>
          <div className="mt-4 grid gap-3 md:grid-cols-[180px_1fr_auto]">
            <select
              value={searchType}
              onChange={e => {
                const next = e.target.value as SearchType;
                setSearchType(next);
                if (next === 'artist') setQuery(bandName ?? '');
                else setQuery('');
              }}
              className="rounded-lg border border-zinc-700 bg-zinc-900 px-4 py-2.5 text-sm text-zinc-100 outline-none transition focus:border-red-700"
            >
              <option value="artist">Artist</option>
              <option value="album">Album</option>
              <option value="song">Song</option>
            </select>
            <input
              value={query}
              onChange={e => setQuery(e.target.value)}
              placeholder={searchType === 'album' ? 'Album title' : searchType === 'song' ? 'Song title' : ''}
              className="w-full rounded-lg border border-zinc-700 bg-zinc-900 px-4 py-2.5 text-sm text-zinc-100 outline-none transition focus:border-red-700"
            />
            <RitualButton variant="primary" disabled={searchLoading} onClick={runSearch}>
              {searchLoading ? 'Searching...' : 'Search'}
            </RitualButton>
          </div>
          {searchError ? <p className="mt-3 text-sm text-red-300">{searchError}</p> : null}

          <div className="mt-4 max-h-56 overflow-y-auto rounded-xl border border-zinc-800 bg-zinc-950/55">
            {searchType === 'artist' ? (
              artistResults.length === 0 ? (
                <p className="px-4 py-3 text-sm text-zinc-500">No artist results yet.</p>
              ) : (
                <ul className="divide-y divide-zinc-800">
                  {artistResults.map(artist => (
                    <li key={artist.id} className="flex items-center justify-between px-4 py-3">
                      <div>
                        <p className="text-sm font-semibold text-zinc-100">{artist.name}</p>
                        <p className="text-xs text-zinc-500">{artist.country ?? 'Unknown country'} {artist.disambiguation ? `- ${artist.disambiguation}` : ''}</p>
                      </div>
                      <RitualButton size="sm" variant="neutral" onClick={() => selectArtist(artist)}>
                        Select artist
                      </RitualButton>
                    </li>
                  ))}
                </ul>
              )
            ) : null}

            {searchType === 'album' ? (
              releaseGroupResults.length === 0 ? (
                <p className="px-4 py-3 text-sm text-zinc-500">No album results yet.</p>
              ) : (
                <ul className="divide-y divide-zinc-800">
                  {releaseGroupResults.map(group => (
                    <li key={group.id} className="flex items-center justify-between gap-3 px-4 py-3">
                      <div>
                        <p className="text-sm font-semibold text-zinc-100">{group.title}</p>
                        <p className="text-xs text-zinc-500">{group.artistCredit ?? 'Unknown artist'} {group.firstReleaseDate ? `- ${group.firstReleaseDate}` : ''}</p>
                      </div>
                      <RitualButton
                        size="sm"
                        variant="neutral"
                        disabled={!group.artistId}
                        onClick={() =>
                          group.artistId
                            ? selectArtist({
                                id: group.artistId,
                                name: group.artistCredit ?? 'Selected artist'
                              })
                            : undefined
                        }
                      >
                        Select artist
                      </RitualButton>
                    </li>
                  ))}
                </ul>
              )
            ) : null}

            {searchType === 'song' ? (
              recordingResults.length === 0 ? (
                <p className="px-4 py-3 text-sm text-zinc-500">No song results yet.</p>
              ) : (
                <ul className="divide-y divide-zinc-800">
                  {recordingResults.map(recording => (
                    <li key={recording.id} className="flex items-center justify-between gap-3 px-4 py-3">
                      <div>
                        <p className="text-sm font-semibold text-zinc-100">{recording.title}</p>
                        <p className="text-xs text-zinc-500">{recording.artistCredit ?? 'Unknown artist'} {recording.releaseTitle ? `- ${recording.releaseTitle}` : ''}</p>
                      </div>
                      <RitualButton
                        size="sm"
                        variant="neutral"
                        disabled={!recording.artistId}
                        onClick={() =>
                          recording.artistId
                            ? selectArtist({
                                id: recording.artistId,
                                name: recording.artistCredit ?? 'Selected artist'
                              })
                            : undefined
                        }
                      >
                        Select artist
                      </RitualButton>
                    </li>
                  ))}
                </ul>
              )
            ) : null}
          </div>
        </RitualCard>

        <div className="grid min-h-0 flex-1 gap-4 lg:grid-cols-[1.1fr_1fr]">
          <RitualCard padded={false} className="flex min-h-0 flex-col overflow-hidden">
            <div className="shrink-0 border-b border-zinc-800 px-4 py-3">
              <p className="text-sm font-black uppercase tracking-[0.2em] text-zinc-200">Albums</p>
              <p className="mt-1 text-xs text-zinc-500">
                {selectedArtist ? `Artist: ${selectedArtist.name}` : 'Select an artist to load albums'}
              </p>
            </div>
            <div className="min-h-0 flex-1 space-y-3 overflow-y-auto px-4 py-3">
              {libraryLoading ? <p className="text-sm text-zinc-500">Loading existing library...</p> : null}
              {libraryError ? <p className="text-sm text-red-300">{libraryError}</p> : null}
              {albumsLoading ? <p className="text-sm text-zinc-500">Loading artist albums...</p> : null}
              {albumsError ? <p className="text-sm text-red-300">{albumsError}</p> : null}
              {!albumsLoading && albums.length === 0 ? <p className="text-sm text-zinc-500">No albums loaded.</p> : null}

              {albums.map(album => (
                <div key={album.releaseGroup.id} className="rounded-lg border border-zinc-800 bg-zinc-950/60 p-3">
                  <div className="flex flex-wrap items-center justify-between gap-3">
                    <label className="flex items-center gap-3 text-sm text-zinc-100">
                      <input
                        type="checkbox"
                        checked={album.selected}
                        disabled={album.alreadyImported}
                        onChange={e => toggleAlbumSelection(album.releaseGroup.id, e.target.checked)}
                      />
                      <span className="font-semibold">{album.releaseGroup.title}</span>
                    </label>
                    {album.alreadyImported ? (
                      <span className="rounded border border-emerald-800 bg-emerald-950/40 px-2 py-1 text-xs uppercase tracking-[0.15em] text-emerald-300">
                        Already imported
                      </span>
                    ) : null}
                  </div>

                  <p className="mt-1 text-xs text-zinc-500">
                    {album.releaseGroup.firstReleaseDate ?? 'Unknown release date'}
                  </p>

                  <div className="mt-3 flex flex-wrap items-center gap-2">
                    <RitualButton
                      size="sm"
                      variant="ghost"
                      disabled={album.releaseLoading || album.alreadyImported}
                      onClick={() => loadAlbumTracks(album.releaseGroup.id)}
                    >
                      {album.releaseLoading ? 'Loading...' : 'Preview songs'}
                    </RitualButton>

                    {album.releases.length > 0 ? (
                      <select
                        value={album.selectedReleaseId ?? ''}
                        onChange={e => setReleaseForAlbum(album.releaseGroup.id, e.target.value)}
                        className="rounded-md border border-zinc-700 bg-zinc-900 px-2 py-1 text-xs text-zinc-200"
                      >
                        {album.releases.map(release => (
                          <option key={release.id} value={release.id}>
                            {release.title} {release.date ? `(${release.date})` : ''}
                          </option>
                        ))}
                      </select>
                    ) : null}
                  </div>

                  {album.releaseError ? <p className="mt-2 text-xs text-red-300">{album.releaseError}</p> : null}
                </div>
              ))}
            </div>
          </RitualCard>

          <RitualCard padded={false} className="flex min-h-0 flex-col overflow-hidden">
            <div className="shrink-0 border-b border-zinc-800 px-4 py-3">
              <div className="flex items-center justify-between gap-3">
                <p className="text-sm font-black uppercase tracking-[0.2em] text-zinc-200">Track preview</p>
                <RitualButton variant="primary" disabled={!canImport} onClick={importSelected}>
                  {importLoading ? 'Importing...' : 'Import selected'}
                </RitualButton>
              </div>
              <p className="mt-1 text-xs text-zinc-500">Choose tracks per album, or leave all selected for full album import.</p>
            </div>
            <div className="min-h-0 flex-1 overflow-y-auto px-4 py-3">
              {albums
                .filter(album => album.selected && !album.alreadyImported)
                .map(album => (
                  <div key={album.releaseGroup.id} className="mb-4 rounded-lg border border-zinc-800 bg-zinc-950/60 p-3">
                    <p className="text-sm font-semibold text-zinc-100">{album.releaseGroup.title}</p>
                    {album.tracks.length === 0 ? (
                      <p className="mt-2 text-xs text-zinc-500">{album.releaseLoading ? 'Loading tracks...' : 'No songs loaded yet. Click Preview songs to see them.'}</p>
                    ) : (
                      <div className="mt-2 space-y-1">
                        {album.tracks.map(track => {
                          const duplicate = isTrackDuplicate(track, existingRecordingIds, existingSongTitleKeys);
                          return (
                            <label key={track.id} className="flex items-center justify-between gap-3 rounded px-2 py-1 text-sm hover:bg-zinc-900/65">
                              <span className="flex items-center gap-2">
                                <input
                                  type="checkbox"
                                  checked={Boolean(album.selectedTrackIds[track.id])}
                                  disabled={duplicate}
                                  onChange={e => toggleTrackSelection(album.releaseGroup.id, track.id, e.target.checked)}
                                />
                                <span className="text-zinc-200">
                                  {track.position ?? '-'} {track.title}
                                </span>
                              </span>
                              {duplicate ? (
                                <span className="text-xs uppercase tracking-[0.12em] text-emerald-300">Already imported</span>
                              ) : null}
                            </label>
                          );
                        })}
                      </div>
                    )}
                  </div>
                ))}

              {selectedAlbumCount === 0 ? (
                <p className="text-sm text-zinc-500">Select one or more albums to prepare import.</p>
              ) : null}

              {importError ? <p className="mt-2 text-sm text-red-300">{importError}</p> : null}
              {importSummary ? (
                <div className="mt-3 rounded-lg border border-emerald-800 bg-emerald-950/35 px-3 py-2 text-sm text-emerald-200">
                  Imported bands: {importSummary.importedBands} | Imported albums: {importSummary.importedAlbums} | Imported songs: {importSummary.importedSongs} | Skipped duplicates: {importSummary.skippedDuplicates}
                </div>
              ) : null}

              <div className="mt-4 flex flex-wrap items-center gap-3 border-t border-zinc-800 pt-3">
                <p className="text-xs uppercase tracking-[0.16em] text-zinc-500">
                  Selected albums: {selectedAlbumCount} | Selected songs: {selectedTrackCount}
                </p>
              </div>
            </div>
          </RitualCard>
        </div>
      </div>
    </AppShell>
  );
}

function isAlbumDuplicate(
  releaseGroup: MusicBrainzReleaseGroup,
  existingAlbumIds: Set<string>,
  existingAlbumTitleKeys: Set<string>
): boolean {
  if (existingAlbumIds.has(releaseGroup.id)) {
    return true;
  }

  return existingAlbumTitleKeys.has(normalizeText(releaseGroup.title));
}

function isTrackDuplicate(
  track: MusicBrainzTrack,
  existingRecordingIds: Set<string>,
  existingSongTitleKeys: Set<string>
): boolean {
  if (track.recordingId && existingRecordingIds.has(track.recordingId)) {
    return true;
  }

  return existingSongTitleKeys.has(normalizeText(track.title));
}

function buildImportAlbums(
  albums: AlbumSelectionState[],
  existingRecordingIds: Set<string>,
  existingSongTitleKeys: Set<string>
): ImportedAlbumDto[] {
  const payload: ImportedAlbumDto[] = [];

  for (const album of albums) {
    if (!album.selected || album.alreadyImported) {
      continue;
    }

    const selectedRelease = album.releases.find(r => r.id === album.selectedReleaseId);

    const songs: ImportedSongDto[] = album.tracks
      .filter(track => Boolean(album.selectedTrackIds[track.id]))
      .filter(track => !isTrackDuplicate(track, existingRecordingIds, existingSongTitleKeys))
      .map(track => ({
        musicBrainzRecordingId: track.recordingId,
        title: track.title,
        durationSeconds: track.lengthMs ? Math.max(1, Math.round(track.lengthMs / 1000)) : undefined,
        trackNumber: track.position
      }));

    const releaseYear = extractYear(album.releaseGroup.firstReleaseDate ?? selectedRelease?.date);

    payload.push({
      musicBrainzReleaseGroupId: album.releaseGroup.id,
      musicBrainzReleaseId: selectedRelease?.id,
      title: album.releaseGroup.title,
      releaseYear,
      songs
    });
  }

  return payload;
}

function extractYear(dateString?: string): number | undefined {
  if (!dateString || dateString.length < 4) {
    return undefined;
  }

  const candidate = Number.parseInt(dateString.slice(0, 4), 10);
  if (!Number.isFinite(candidate) || candidate < 1900 || candidate > 2100) {
    return undefined;
  }

  return candidate;
}

function normalizeText(value?: string): string {
  return (value ?? '')
    .trim()
    .toLowerCase()
    .replace(/[^a-z0-9]/g, '');
}
