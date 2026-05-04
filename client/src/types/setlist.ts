export interface SetlistSong {
  songId: string;
  title: string;
  bpm?: number;
  durationSeconds?: number;
  order: number;
  transitionNotes?: string;
  performanceNotes?: string;
}

export interface Setlist {
  id: string;
  bandId: string;
  name: string;
  description?: string;
  songs: SetlistSong[];
}
