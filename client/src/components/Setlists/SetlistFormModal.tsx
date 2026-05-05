import { useEffect, useState, type FormEvent } from 'react';
import type { CreateSetlistRequest, SetlistSummary, UpdateSetlistRequest } from '../../types/setlist';
import { RitualButton } from '../ui/RitualButton';
import { RitualCard } from '../ui/RitualCard';

interface SetlistFormModalProps {
  isOpen: boolean;
  mode: 'create' | 'edit';
  setlist?: SetlistSummary | null;
  isSubmitting?: boolean;
  submitError?: string | null;
  onClose: () => void;
  onSubmit: (payload: CreateSetlistRequest | UpdateSetlistRequest) => Promise<void>;
}

export function SetlistFormModal({
  isOpen,
  mode,
  setlist,
  isSubmitting = false,
  submitError,
  onClose,
  onSubmit
}: SetlistFormModalProps) {
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [nameError, setNameError] = useState<string | null>(null);

  useEffect(() => {
    if (!isOpen) {
      return;
    }

    setName(setlist?.name ?? '');
    setDescription(setlist?.description ?? '');
    setNameError(null);
  }, [isOpen, setlist]);

  if (!isOpen) {
    return null;
  }

  const title = mode === 'create' ? 'Create New Ritual' : 'Edit Ritual';
  const submitLabel = mode === 'create' ? 'Create Ritual' : 'Save Changes';

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const trimmedName = name.trim();
    if (!trimmedName) {
      setNameError('Setlist name is required.');
      return;
    }

    setNameError(null);
    await onSubmit({
      name: trimmedName,
      description: description.trim() || undefined
    });
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-zinc-950/80 px-4 backdrop-blur-sm">
      <RitualCard className="w-full max-w-2xl border-zinc-700/90">
        <p className="text-[10px] font-semibold uppercase tracking-[0.36em] text-red-500">Setlist</p>
        <h3 className="mt-2 text-2xl font-semibold text-zinc-100">{title}</h3>

        <form className="mt-5 space-y-4" onSubmit={handleSubmit}>
          <label className="block">
            <span className="text-xs uppercase tracking-[0.2em] text-zinc-400">Name</span>
            <input
              value={name}
              onChange={e => setName(e.target.value)}
              maxLength={150}
              disabled={isSubmitting}
              className="mt-1.5 w-full rounded-xl border border-zinc-700 bg-zinc-900 px-3 py-2.5 text-sm text-zinc-100 outline-none transition-colors focus:border-red-600"
              placeholder="Ritual name"
            />
            {nameError && <p className="mt-1.5 text-xs text-red-400">{nameError}</p>}
          </label>

          <label className="block">
            <span className="text-xs uppercase tracking-[0.2em] text-zinc-400">Description (Optional)</span>
            <textarea
              value={description}
              onChange={e => setDescription(e.target.value)}
              maxLength={500}
              rows={4}
              disabled={isSubmitting}
              className="mt-1.5 w-full resize-none rounded-xl border border-zinc-700 bg-zinc-900 px-3 py-2.5 text-sm text-zinc-100 outline-none transition-colors focus:border-red-600"
              placeholder="Add context for this ritual setlist"
            />
            <p className="mt-1 text-[11px] text-zinc-500">{description.length}/500</p>
          </label>

          {submitError && <p className="text-sm text-red-300">{submitError}</p>}

          <div className="flex items-center justify-end gap-2">
            <RitualButton type="button" variant="neutral" size="sm" onClick={onClose} disabled={isSubmitting}>
              Cancel
            </RitualButton>
            <RitualButton type="submit" variant="primary" size="sm" disabled={isSubmitting}>
              {isSubmitting ? 'Saving...' : submitLabel}
            </RitualButton>
          </div>
        </form>
      </RitualCard>
    </div>
  );
}
