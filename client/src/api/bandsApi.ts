import { apiFetch } from './httpClient';
import type { Band } from '../types/band';

export function getBands(userId?: string): Promise<Band[]> {
  const query = userId ? `?userId=${encodeURIComponent(userId)}` : '';
  return apiFetch<Band[]>(`/api/bands${query}`);
}

export function getBand(bandId: string): Promise<Band> {
  return apiFetch<Band>(`/api/bands/${bandId}`);
}

export function updateBandName(bandId: string, name: string): Promise<Band> {
  return apiFetch<Band>(`/api/bands/${bandId}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ name }),
  });
}
