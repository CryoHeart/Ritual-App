import { apiFetch } from './httpClient';
import type { Band } from '../types/band';

export interface BandMember {
  bandMemberId: string;
  userId: string;
  displayName: string;
  email: string;
  role: string;
  instrument?: string;
  joinedAt: string;
}

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

export function getBandMembers(bandId: string): Promise<BandMember[]> {
  return apiFetch<BandMember[]>(`/api/bands/${bandId}/members`);
}

export function addBandMember(bandId: string, email: string, role: string, instrument?: string): Promise<void> {
  return apiFetch<void>(`/api/bands/${bandId}/members`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, role, instrument }),
  });
}
