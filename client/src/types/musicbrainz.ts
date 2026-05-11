export interface MusicBrainzArtist {
  id: string;
  name: string;
  country?: string;
  disambiguation?: string;
  type?: string;
  score?: number;
}

export interface MusicBrainzReleaseGroup {
  id: string;
  title: string;
  firstReleaseDate?: string;
  primaryType?: string;
  secondaryTypes?: string[];
  artistCredit?: string;
  artistId?: string;
  score?: number;
}

export interface MusicBrainzRelease {
  id: string;
  title: string;
  date?: string;
  country?: string;
  status?: string;
  trackCount?: number;
}

export interface MusicBrainzTrack {
  id: string;
  recordingId?: string;
  title: string;
  lengthMs?: number;
  position?: number;
  number?: string;
  artistCredit?: string;
}

export interface MusicBrainzRecording {
  id: string;
  title: string;
  lengthMs?: number;
  artistCredit?: string;
  artistId?: string;
  releaseTitle?: string;
  score?: number;
}

export interface ImportMusicBrainzSelectionRequest {
  userId: string;
  bandId: string;
  artist: MusicBrainzArtistSelection;
  albums: ImportedAlbumDto[];
}

export interface MusicBrainzArtistSelection {
  id: string;
  name: string;
  country?: string;
}

export interface ImportedAlbumDto {
  musicBrainzReleaseGroupId: string;
  musicBrainzReleaseId?: string;
  title: string;
  releaseYear?: number;
  songs: ImportedSongDto[];
}

export interface ImportedSongDto {
  musicBrainzRecordingId?: string;
  title: string;
  durationSeconds?: number;
  trackNumber?: number;
}

export interface MusicBrainzImportSummary {
  importedBands: number;
  importedAlbums: number;
  importedSongs: number;
  skippedDuplicates: number;
}
