import { apiFetch } from './httpClient';
import type { Song, CreateSongRequest, UpdateSongRequest } from '../types/song';

export function getSongs(bandId: string): Promise<Song[]> {
  return apiFetch<Song[]>(`/api/bands/${bandId}/songs`);
}

export function getSong(bandId: string, songId: string): Promise<Song> {
  return apiFetch<Song>(`/api/bands/${bandId}/songs/${songId}`);
}

export function createSong(bandId: string, request: CreateSongRequest): Promise<Song> {
  return apiFetch<Song>(`/api/bands/${bandId}/songs`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request)
  });
}

export function updateSong(bandId: string, songId: string, request: UpdateSongRequest): Promise<Song> {
  return apiFetch<Song>(`/api/bands/${bandId}/songs/${songId}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request)
  });
}

export function deleteSong(bandId: string, songId: string): Promise<void> {
  return apiFetch<void>(`/api/bands/${bandId}/songs/${songId}`, {
    method: 'DELETE'
  });
}
