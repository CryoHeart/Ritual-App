import { useEffect, useState, type FormEvent } from 'react';
import type { Album, CreateAlbumRequest, UpdateAlbumRequest } from '../../types/album';
import { RitualButton } from '../ui/RitualButton';
import { RitualCard } from '../ui/RitualCard';

interface Props {
  isOpen: boolean;
  album: Album | null;
  isSubmitting?: boolean;
  submitError?: string | null;
  onClose: () => void;
  onSubmit: (payload: CreateAlbumRequest | UpdateAlbumRequest) => Promise<void>;
}

export function AlbumFormModal({ isOpen, album, isSubmitting = false, submitError, onClose, onSubmit }: Props) {
  const [title, setTitle] = useState('');
  const [releaseYear, setReleaseYear] = useState('');
  const [sortOrder, setSortOrder] = useState('0');
  const [formError, setFormError] = useState<string | null>(null);

  useEffect(() => {
    if (!isOpen) return;
    setTitle(album?.title ?? '');
    setReleaseYear(album?.releaseYear != null ? String(album.releaseYear) : '');
    setSortOrder(album != null ? String(album.sortOrder) : '0');
    setFormError(null);
  }, [isOpen, album]);

  if (!isOpen) return null;

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();

    const trimmedTitle = title.trim();
    if (!trimmedTitle) {
      setFormError('Album title is required.');
      return;
    }
    if (trimmedTitle.length > 150) {
      setFormError('Album title cannot exceed 150 characters.');
      return;
    }

    const parsedYear = releaseYear.trim() === '' ? undefined : Number(releaseYear.trim());
    if (parsedYear != null && (!Number.isInteger(parsedYear) || parsedYear < 1900 || parsedYear > 2100)) {
      setFormError('Release year must be between 1900 and 2100.');
      return;
    }

    const parsedSort = Number(sortOrder.trim());
    if (!Number.isInteger(parsedSort) || parsedSort < 0) {
      setFormError('Sort order must be 0 or greater.');
      return;
    }

    setFormError(null);
    await onSubmit({ title: trimmedTitle, releaseYear: parsedYear, sortOrder: parsedSort });
  };

  const isEdit = album != null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-zinc-950/80 px-4 backdrop-blur-sm">
      <RitualCard className="w-full max-w-lg border-zinc-700/90">
        <p className="text-[10px] font-semibold uppercase tracking-[0.36em] text-red-500">Album</p>
        <h3 className="mt-2 text-xl font-semibold text-zinc-100">{isEdit ? 'Edit Album' : 'New Album'}</h3>

        <form className="mt-5 space-y-4" onSubmit={handleSubmit}>
          <label className="block">
            <span className="text-xs uppercase tracking-[0.2em] text-zinc-400">Title</span>
            <input
              value={title}
              onChange={e => setTitle(e.target.value)}
              maxLength={150}
              disabled={isSubmitting}
              className="mt-1.5 w-full rounded-xl border border-zinc-700 bg-zinc-900 px-3 py-2.5 text-sm text-zinc-100 outline-none transition-colors focus:border-red-600"
            />
          </label>

          <div className="grid grid-cols-2 gap-4">
            <label className="block">
              <span className="text-xs uppercase tracking-[0.2em] text-zinc-400">Release Year</span>
              <input
                value={releaseYear}
                onChange={e => setReleaseYear(e.target.value)}
                inputMode="numeric"
                placeholder="Optional"
                disabled={isSubmitting}
                className="mt-1.5 w-full rounded-xl border border-zinc-700 bg-zinc-900 px-3 py-2.5 text-sm text-zinc-100 outline-none transition-colors placeholder:text-zinc-700 focus:border-red-600"
              />
            </label>
            <label className="block">
              <span className="text-xs uppercase tracking-[0.2em] text-zinc-400">Sort Order</span>
              <input
                value={sortOrder}
                onChange={e => setSortOrder(e.target.value)}
                inputMode="numeric"
                disabled={isSubmitting}
                className="mt-1.5 w-full rounded-xl border border-zinc-700 bg-zinc-900 px-3 py-2.5 text-sm text-zinc-100 outline-none transition-colors focus:border-red-600"
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
              {isSubmitting ? 'Saving...' : isEdit ? 'Save Changes' : 'Create Album'}
            </RitualButton>
          </div>
        </form>
      </RitualCard>
    </div>
  );
}
