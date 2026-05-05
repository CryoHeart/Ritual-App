export interface SetlistSong {
  setlistSongId: string;
  songId: string;
  title: string;
  bpm?: number;
  durationSeconds?: number;
  positionIndex: number;
  transitionNotes?: string;
  performanceNotes?: string;
  tuning?: string;
  songKey?: string;
  notes?: string;
  albumId?: string;
  albumTitle?: string;
  albumTrackNumber?: number;
}

export interface CreateSetlistRequest {
  name: string;
  description?: string;
}

export interface UpdateSetlistRequest {
  name: string;
  description?: string;
}

export interface SetlistSummary {
  setlistId: string;
  bandId: string;
  name: string;
  description?: string;
  totalSongs: number;
  totalDurationSeconds: number;
  createdAt: string;
  updatedAt?: string;
}

export type Setlist = SetlistSummary;

// Detailed view (with songs)
export interface SetlistDetails {
  setlistId: string;
  bandId: string;
  name: string;
  description?: string;
  totalSongs: number;
  totalDurationSeconds: number;
  createdAt: string;
  updatedAt?: string;
  songs: SetlistSong[];
}
