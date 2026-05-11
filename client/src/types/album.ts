export interface AlbumSong {
  songId: string;
  bandId: string;
  title: string;
  musicBrainzRecordingId?: string;
  bpm?: number;
  durationSeconds?: number;
  albumId?: string | null;
  albumTitle?: string;
  albumTrackNumber?: number;
  tuning?: string;
  songKey?: string;
  notes?: string;
}

export interface Album {
  albumId: string | null;
  bandId: string;
  title: string;
  musicBrainzReleaseGroupId?: string;
  musicBrainzReleaseId?: string;
  releaseYear?: number;
  sortOrder: number;
  createdAt?: string;
  updatedAt?: string;
}

export interface AlbumWithSongs extends Album {
  albumId: string | null;
  songs: AlbumSong[];
}

export interface CreateAlbumRequest {
  title: string;
  releaseYear?: number;
  sortOrder: number;
}

export interface UpdateAlbumRequest {
  title: string;
  releaseYear?: number;
  sortOrder: number;
}
