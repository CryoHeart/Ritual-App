import { apiFetch } from './httpClient';
import type { Band } from '../types/band';

export function getBands(): Promise<Band[]> {
  return apiFetch<Band[]>('/api/bands');
}

export function getBand(bandId: string): Promise<Band> {
  return apiFetch<Band>(`/api/bands/${bandId}`);
}
