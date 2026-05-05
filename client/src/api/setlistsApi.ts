import { apiFetch } from './httpClient';
import type {
  CreateSetlistRequest,
  SetlistSummary,
  SetlistDetails,
  UpdateSetlistRequest
} from '../types/setlist';

export function getSetlists(bandId: string): Promise<SetlistSummary[]> {
  return apiFetch<SetlistSummary[]>(`/api/bands/${bandId}/setlists`);
}

export function getSetlistDetails(bandId: string, setlistId: string): Promise<SetlistDetails> {
  return apiFetch<SetlistDetails>(`/api/bands/${bandId}/setlists/${setlistId}`);
}

export function createSetlist(bandId: string, payload: CreateSetlistRequest): Promise<SetlistSummary> {
  return apiFetch<SetlistSummary>(`/api/bands/${bandId}/setlists`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload)
  });
}

export function updateSetlist(
  bandId: string,
  setlistId: string,
  payload: UpdateSetlistRequest
): Promise<SetlistSummary> {
  return apiFetch<SetlistSummary>(`/api/bands/${bandId}/setlists/${setlistId}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload)
  });
}

export function deleteSetlist(bandId: string, setlistId: string): Promise<void> {
  return apiFetch<void>(`/api/bands/${bandId}/setlists/${setlistId}`, {
    method: 'DELETE'
  });
}

export function addSongToSetlist(bandId: string, setlistId: string, songId: string): Promise<SetlistDetails> {
  return apiFetch<SetlistDetails>(`/api/bands/${bandId}/setlists/${setlistId}/songs`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ songId })
  });
}

export function removeSongFromSetlist(bandId: string, setlistId: string, setlistSongId: string): Promise<SetlistDetails> {
  return apiFetch<SetlistDetails>(`/api/bands/${bandId}/setlists/${setlistId}/songs/${setlistSongId}`, {
    method: 'DELETE'
  });
}

export function reorderSetlistSongs(
  bandId: string,
  setlistId: string,
  items: { setlistSongId: string; positionIndex: number }[]
): Promise<SetlistDetails> {
  return apiFetch<SetlistDetails>(`/api/bands/${bandId}/setlists/${setlistId}/songs/reorder`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ items })
  });
}
