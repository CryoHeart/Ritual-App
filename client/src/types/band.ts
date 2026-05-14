export interface Band {
  id: string;
  name: string;
  description?: string;
  country?: string;
  musicBrainzArtistId?: string;
  role?: string;
}

export type BandRole = 'RitualLeader' | 'BandMember';

export function isRitualLeader(band: Band | null | undefined): boolean {
  return band?.role === 'RitualLeader';
}

export function isBandMember(band: Band | null | undefined): boolean {
  return band?.role === 'RitualLeader' || band?.role === 'BandMember';
}
