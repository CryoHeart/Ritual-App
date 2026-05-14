export interface LiveSessionSong {
  setlistSongId: string;
  songId: string;
  title: string;
  albumTitle?: string;
  albumTrackNumber?: number;
  durationSeconds?: number;
  bpm?: number;
  tuning?: string;
  songKey?: string;
  notes?: string;
  transitionNotes?: string;
  performanceNotes?: string;
  positionIndex: number;
}

export interface LiveSession {
  liveSessionId: string;
  bandId: string;
  setlistId: string;
  setlistName: string;
  status: string;
  currentPositionIndex: number;
  startedAt: string;
  endedAt?: string;
  startedByUserId?: string;
  startedByDisplayName?: string;
  currentSong?: LiveSessionSong;
  nextSong?: LiveSessionSong;
  totalSongs: number;
  totalDurationSeconds: number;
  canControl: boolean;
}
