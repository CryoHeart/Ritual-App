import { apiFetch } from './httpClient';
import type { Band } from '../types/band';

export function getBands(userId?: string): Promise<Band[]> {
  const query = userId ? `?userId=${encodeURIComponent(userId)}` : '';
  return apiFetch<Band[]>(`/api/bands${query}`);
}

export function getBand(bandId: string): Promise<Band> {
  return apiFetch<Band>(`/api/bands/${bandId}`);
}
