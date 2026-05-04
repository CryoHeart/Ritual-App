import { apiFetch } from './httpClient';
import type { Song } from '../types/song';

export function getSongs(bandId: string): Promise<Song[]> {
  return apiFetch<Song[]>(`/api/bands/${bandId}/songs`);
}

export function getSong(bandId: string, songId: string): Promise<Song> {
  return apiFetch<Song>(`/api/bands/${bandId}/songs/${songId}`);
}
