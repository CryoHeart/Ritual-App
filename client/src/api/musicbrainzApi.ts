import { apiFetch } from './httpClient';
import type {
  ImportMusicBrainzSelectionRequest,
  MusicBrainzArtist,
  MusicBrainzImportSummary,
  MusicBrainzRecording,
  MusicBrainzRelease,
  MusicBrainzReleaseGroup,
  MusicBrainzTrack
} from '../types/musicbrainz';

export function searchMusicBrainzArtists(query: string): Promise<MusicBrainzArtist[]> {
  return apiFetch<MusicBrainzArtist[]>(`/api/musicbrainz/search-artists?query=${encodeURIComponent(query)}`);
}

export function searchMusicBrainzReleaseGroups(query: string): Promise<MusicBrainzReleaseGroup[]> {
  return apiFetch<MusicBrainzReleaseGroup[]>(`/api/musicbrainz/search-release-groups?query=${encodeURIComponent(query)}`);
}

export function searchMusicBrainzRecordings(query: string): Promise<MusicBrainzRecording[]> {
  return apiFetch<MusicBrainzRecording[]>(`/api/musicbrainz/search-recordings?query=${encodeURIComponent(query)}`);
}

export function getMusicBrainzArtistAlbums(artistMbid: string): Promise<MusicBrainzReleaseGroup[]> {
  return apiFetch<MusicBrainzReleaseGroup[]>(`/api/musicbrainz/artist/${encodeURIComponent(artistMbid)}/albums`);
}

export function getMusicBrainzReleaseGroupReleases(releaseGroupMbid: string): Promise<MusicBrainzRelease[]> {
  return apiFetch<MusicBrainzRelease[]>(`/api/musicbrainz/release-group/${encodeURIComponent(releaseGroupMbid)}/releases`);
}

export function getMusicBrainzReleaseTracks(releaseMbid: string): Promise<MusicBrainzTrack[]> {
  return apiFetch<MusicBrainzTrack[]>(`/api/musicbrainz/release/${encodeURIComponent(releaseMbid)}/tracks`);
}

export function importMusicBrainzSelection(request: ImportMusicBrainzSelectionRequest): Promise<MusicBrainzImportSummary> {
  return apiFetch<MusicBrainzImportSummary>('/api/musicbrainz/import-selection', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request)
  });
}
