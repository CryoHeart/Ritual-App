import { apiFetch } from './httpClient';
import type { LiveSession } from '../types/liveSession';

const base = (bandId: string) => `/api/bands/${bandId}/live-sessions`;

export function getActiveLiveSession(bandId: string): Promise<LiveSession | null> {
  return apiFetch<LiveSession | null>(`${base(bandId)}/active`);
}

export function getLiveSession(bandId: string, liveSessionId: string): Promise<LiveSession> {
  return apiFetch<LiveSession>(`${base(bandId)}/${liveSessionId}`);
}

export function startLiveSession(bandId: string, setlistId: string): Promise<LiveSession> {
  return apiFetch<LiveSession>(`/api/bands/${bandId}/setlists/${setlistId}/live-sessions/start`, {
    method: 'POST',
  });
}

export function nextLiveSessionSong(bandId: string, liveSessionId: string): Promise<LiveSession> {
  return apiFetch<LiveSession>(`${base(bandId)}/${liveSessionId}/next`, { method: 'POST' });
}

export function previousLiveSessionSong(bandId: string, liveSessionId: string): Promise<LiveSession> {
  return apiFetch<LiveSession>(`${base(bandId)}/${liveSessionId}/previous`, { method: 'POST' });
}

export function endLiveSession(bandId: string, liveSessionId: string): Promise<LiveSession> {
  return apiFetch<LiveSession>(`${base(bandId)}/${liveSessionId}/end`, { method: 'POST' });
}
