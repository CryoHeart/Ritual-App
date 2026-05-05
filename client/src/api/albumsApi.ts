import { apiFetch } from './httpClient';
import type { Album, AlbumWithSongs, CreateAlbumRequest, UpdateAlbumRequest } from '../types/album';

export function getAlbums(bandId: string): Promise<Album[]> {
  return apiFetch<Album[]>(`/api/bands/${bandId}/albums`);
}

export function getAlbumsWithSongs(bandId: string): Promise<AlbumWithSongs[]> {
  return apiFetch<AlbumWithSongs[]>(`/api/bands/${bandId}/albums-with-songs`);
}

export function createAlbum(bandId: string, request: CreateAlbumRequest): Promise<Album> {
  return apiFetch<Album>(`/api/bands/${bandId}/albums`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request)
  });
}

export function updateAlbum(bandId: string, albumId: string, request: UpdateAlbumRequest): Promise<Album> {
  return apiFetch<Album>(`/api/bands/${bandId}/albums/${albumId}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request)
  });
}

export function deleteAlbum(bandId: string, albumId: string): Promise<void> {
  return apiFetch<void>(`/api/bands/${bandId}/albums/${albumId}`, {
    method: 'DELETE'
  });
}
