import { useEffect, useMemo, useState, type FormEvent } from 'react';
import type { AlbumWithSongs } from '../../types/album';
import type { Song, CreateSongRequest, UpdateSongRequest } from '../../types/song';
import { RitualButton } from '../ui/RitualButton';
import { RitualCard } from '../ui/RitualCard';

interface Props {
  isOpen: boolean;
  song: Song | null;
  albums: AlbumWithSongs[];
  isSubmitting?: boolean;
  submitError?: string | null;
  onClose: () => void;
  onSubmit: (payload: CreateSongRequest | UpdateSongRequest) => Promise<void>;
}

export function SongFormModal({ isOpen, song, albums, isSubmitting = false, submitError, onClose, onSubmit }: Props) {
  const [title, setTitle] = useState('');
  const [albumId, setAlbumId] = useState('');
  const [minutes, setMinutes] = useState('');
  const [seconds, setSeconds] = useState('');
  const [albumTrackNumber, setAlbumTrackNumber] = useState('');
  const [bpm, setBpm] = useState('');
  const [tuning, setTuning] = useState('');
  const [songKey, setSongKey] = useState('');
  const [notes, setNotes] = useState('');
  const [formError, setFormError] = useState<string | null>(null);

  useEffect(() => {
    if (!isOpen) return;
    setTitle(song?.title ?? '');
    setAlbumId(song?.albumId ?? '');
    const total = song?.durationSeconds ?? 0;
    setMinutes(total > 0 ? String(Math.floor(total / 60)) : '');
    setSeconds(total > 0 ? String(total % 60) : '');
    setAlbumTrackNumber(song?.albumTrackNumber != null ? String(song.albumTrackNumber) : '');
    setBpm(song?.bpm != null ? String(song.bpm) : '');
    setTuning(song?.tuning ?? '');
    setSongKey(song?.songKey ?? '');
    setNotes(song?.notes ?? '');
    setFormError(null);
  }, [isOpen, song]);

  const selectableAlbums = useMemo(
    () => albums.filter(a => a.albumId !== null && a.title.toLowerCase() !== 'unassigned'),
    [albums]
  );

  if (!isOpen) return null;

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();

    const trimmedTitle = title.trim();
    if (!trimmedTitle) {
      setFormError('Song title is required.');
      return;
    }
    if (trimmedTitle.length > 150) {
      setFormError('Song title cannot exceed 150 characters.');
      return;
    }

    const parsedMinutes = minutes.trim() === '' ? 0 : Number(minutes);
    const parsedSeconds = seconds.trim() === '' ? 0 : Number(seconds);
    if (!Number.isInteger(parsedMinutes) || parsedMinutes < 0) {
      setFormError('Minutes must be a non-negative whole number.');
      return;
    }
    if (!Number.isInteger(parsedSeconds) || parsedSeconds < 0 || parsedSeconds > 59) {
      setFormError('Seconds must be between 0 and 59.');
      return;
    }

    const durationSeconds = parsedMinutes === 0 && parsedSeconds === 0
      ? undefined
      : parsedMinutes * 60 + parsedSeconds;

    const parsedTrack = albumTrackNumber.trim() === '' ? undefined : Number(albumTrackNumber);
    if (parsedTrack != null && (!Number.isInteger(parsedTrack) || parsedTrack <= 0)) {
      setFormError('Track number must be a positive whole number.');
      return;
    }

    const parsedBpm = bpm.trim() === '' ? undefined : Number(bpm);
    if (parsedBpm != null && (!Number.isInteger(parsedBpm) || parsedBpm < 40 || parsedBpm > 300)) {
      setFormError('BPM must be between 40 and 300.');
      return;
    }

    setFormError(null);
    await onSubmit({
      title: trimmedTitle,
      albumId: albumId.trim() || undefined,
      durationSeconds,
      albumTrackNumber: parsedTrack,
      bpm: parsedBpm,
      tuning: tuning.trim() || undefined,
      songKey: songKey.trim() || undefined,
      notes: notes.trim() || undefined
    });
  };

  const isEdit = song != null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-zinc-950/80 px-4 backdrop-blur-sm">
      <RitualCard className="w-full max-w-2xl border-zinc-700/90">
        <p className="text-[10px] font-semibold uppercase tracking-[0.36em] text-red-500">Song</p>
        <h3 className="mt-2 text-2xl font-semibold text-zinc-100">{isEdit ? 'Edit Song' : 'New Song'}</h3>

        <form className="mt-5 space-y-4" onSubmit={handleSubmit}>
          <div className="grid gap-4 md:grid-cols-2">
            <label className="block md:col-span-2">
              <span className="text-xs uppercase tracking-[0.2em] text-zinc-400">Title</span>
              <input
                value={title}
                onChange={e => setTitle(e.target.value)}
                maxLength={150}
                disabled={isSubmitting}
                className="mt-1.5 w-full rounded-xl border border-zinc-700 bg-zinc-900 px-3 py-2.5 text-sm text-zinc-100 outline-none transition-colors focus:border-red-600"
              />
            </label>

            <label className="block">
              <span className="text-xs uppercase tracking-[0.2em] text-zinc-400">Album</span>
              <select
                value={albumId}
                onChange={e => setAlbumId(e.target.value)}
                disabled={isSubmitting}
                className="mt-1.5 w-full rounded-xl border border-zinc-700 bg-zinc-900 px-3 py-2.5 text-sm text-zinc-100 outline-none transition-colors focus:border-red-600"
              >
                <option value="">Unassigned</option>
                {selectableAlbums.map(a => (
                  <option key={a.albumId!} value={a.albumId!}>{a.title}</option>
                ))}
              </select>
            </label>

            <label className="block">
              <span className="text-xs uppercase tracking-[0.2em] text-zinc-400">Track Number</span>
              <input
                value={albumTrackNumber}
                onChange={e => setAlbumTrackNumber(e.target.value)}
                inputMode="numeric"
                disabled={isSubmitting}
                className="mt-1.5 w-full rounded-xl border border-zinc-700 bg-zinc-900 px-3 py-2.5 text-sm text-zinc-100 outline-none transition-colors focus:border-red-600"
              />
            </label>

            <div className="grid grid-cols-2 gap-2">
              <label className="block">
                <span className="text-xs uppercase tracking-[0.2em] text-zinc-400">Minutes</span>
                <input
                  value={minutes}
                  onChange={e => setMinutes(e.target.value)}
                  inputMode="numeric"
                  disabled={isSubmitting}
                  className="mt-1.5 w-full rounded-xl border border-zinc-700 bg-zinc-900 px-3 py-2.5 text-sm text-zinc-100 outline-none transition-colors focus:border-red-600"
                />
              </label>
              <label className="block">
                <span className="text-xs uppercase tracking-[0.2em] text-zinc-400">Seconds</span>
                <input
                  value={seconds}
                  onChange={e => setSeconds(e.target.value)}
                  inputMode="numeric"
                  disabled={isSubmitting}
                  className="mt-1.5 w-full rounded-xl border border-zinc-700 bg-zinc-900 px-3 py-2.5 text-sm text-zinc-100 outline-none transition-colors focus:border-red-600"
                />
              </label>
            </div>

            <label className="block">
              <span className="text-xs uppercase tracking-[0.2em] text-zinc-400">BPM</span>
              <input
                value={bpm}
                onChange={e => setBpm(e.target.value)}
                inputMode="numeric"
                disabled={isSubmitting}
                className="mt-1.5 w-full rounded-xl border border-zinc-700 bg-zinc-900 px-3 py-2.5 text-sm text-zinc-100 outline-none transition-colors focus:border-red-600"
              />
            </label>

            <label className="block">
              <span className="text-xs uppercase tracking-[0.2em] text-zinc-400">Tuning</span>
              <input
                value={tuning}
                onChange={e => setTuning(e.target.value)}
                maxLength={50}
                disabled={isSubmitting}
                className="mt-1.5 w-full rounded-xl border border-zinc-700 bg-zinc-900 px-3 py-2.5 text-sm text-zinc-100 outline-none transition-colors focus:border-red-600"
              />
            </label>

            <label className="block">
              <span className="text-xs uppercase tracking-[0.2em] text-zinc-400">Key</span>
              <input
                value={songKey}
                onChange={e => setSongKey(e.target.value)}
                maxLength={50}
                disabled={isSubmitting}
                className="mt-1.5 w-full rounded-xl border border-zinc-700 bg-zinc-900 px-3 py-2.5 text-sm text-zinc-100 outline-none transition-colors focus:border-red-600"
              />
            </label>

            <label className="block md:col-span-2">
              <span className="text-xs uppercase tracking-[0.2em] text-zinc-400">Notes</span>
              <textarea
                value={notes}
                onChange={e => setNotes(e.target.value)}
                rows={3}
                disabled={isSubmitting}
                className="mt-1.5 w-full resize-none rounded-xl border border-zinc-700 bg-zinc-900 px-3 py-2.5 text-sm text-zinc-100 outline-none transition-colors focus:border-red-600"
              />
            </label>
          </div>

          {(formError || submitError) && (
            <p className="rounded-lg border border-red-900/50 bg-red-950/30 px-3 py-2 text-xs text-red-300">
              {formError ?? submitError}
            </p>
          )}

          <div className="flex items-center justify-end gap-2 pt-1">
            <RitualButton variant="neutral" size="sm" type="button" onClick={onClose} disabled={isSubmitting}>
              Cancel
            </RitualButton>
            <RitualButton variant="primary" size="sm" type="submit" disabled={isSubmitting}>
              {isSubmitting ? 'Saving...' : isEdit ? 'Save Changes' : 'Add Song'}
            </RitualButton>
          </div>
        </form>
      </RitualCard>
    </div>
  );
}
