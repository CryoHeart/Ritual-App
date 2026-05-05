export interface Song {
  songId: string;
  bandId: string;
  title: string;
  bpm?: number;
  durationSeconds?: number;
  albumId?: string;
  albumTitle?: string;
  albumTrackNumber?: number;
  tuning?: string;
  songKey?: string;
  notes?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface CreateSongRequest {
  title: string;
  albumId?: string;
  albumTrackNumber?: number;
  durationSeconds?: number;
  bpm?: number;
  tuning?: string;
  songKey?: string;
  notes?: string;
}

export interface UpdateSongRequest {
  title: string;
  albumId?: string;
  durationSeconds?: number;
  albumTrackNumber?: number;
  bpm?: number;
  tuning?: string;
  songKey?: string;
  notes?: string;
}
