import { apiFetch } from './httpClient';
import type { Setlist } from '../types/setlist';

export function getSetlists(bandId: string): Promise<Setlist[]> {
  return apiFetch<Setlist[]>(`/api/bands/${bandId}/setlists`);
}

export function getSetlist(bandId: string, setlistId: string): Promise<Setlist> {
  return apiFetch<Setlist>(`/api/bands/${bandId}/setlists/${setlistId}`);
}
