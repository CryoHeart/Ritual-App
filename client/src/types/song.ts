export interface Song {
  id: string;
  bandId: string;
  title: string;
  bpm?: number;
  durationSeconds?: number;
  tuning?: string;
  songKey?: string;
  notes?: string;
}
