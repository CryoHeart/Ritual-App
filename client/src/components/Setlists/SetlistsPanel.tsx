import { useMemo, useState } from 'react';
import type {
  CreateSetlistRequest,
  SetlistSummary,
  UpdateSetlistRequest
} from '../../types/setlist';
import { ConfirmDialog } from '../ui/ConfirmDialog';
import { RitualButton } from '../ui/RitualButton';
import { SetlistCard } from './SetlistCard';
import { SetlistFormModal } from './SetlistFormModal';

interface SetlistsPanelProps {
  setlists: SetlistSummary[];
  selectedSetlistId: string | null;
  isLoading: boolean;
  panelMessage?: string | null;
  onCreateSetlist: (payload: CreateSetlistRequest) => Promise<void>;
  onUpdateSetlist: (setlistId: string, payload: UpdateSetlistRequest) => Promise<void>;
  onDeleteSetlist: (setlistId: string) => Promise<void>;
  onEditSetlistSongs: (setlistId: string) => void;
  onBeginRitual: (setlistId: string) => void;
}

export function SetlistsPanel({
  setlists,
  selectedSetlistId,
  isLoading,
  panelMessage,
  onCreateSetlist,
  onUpdateSetlist,
  onDeleteSetlist,
  onEditSetlistSongs,
  onBeginRitual
}: SetlistsPanelProps) {
  const [expandedId, setExpandedId] = useState<string | null>(null);
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [editingSetlist, setEditingSetlist] = useState<SetlistSummary | null>(null);
  const [pendingDelete, setPendingDelete] = useState<SetlistSummary | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);
  const [deleteError, setDeleteError] = useState<string | null>(null);

  const modalMode = useMemo<'create' | 'edit'>(() => (editingSetlist ? 'edit' : 'create'), [editingSetlist]);

  const openCreateModal = () => {
    setEditingSetlist(null);
    setFormError(null);
    setIsFormOpen(true);
  };

  const openEditModal = (setlist: SetlistSummary) => {
    setEditingSetlist(setlist);
    setFormError(null);
    setIsFormOpen(true);
  };

  const closeFormModal = () => {
    if (isSubmitting) {
      return;
    }

    setIsFormOpen(false);
    setEditingSetlist(null);
    setFormError(null);
  };

  const handleSubmit = async (payload: CreateSetlistRequest | UpdateSetlistRequest) => {
    setIsSubmitting(true);
    setFormError(null);

    try {
      if (modalMode === 'create') {
        await onCreateSetlist(payload);
      } else if (editingSetlist) {
        await onUpdateSetlist(editingSetlist.setlistId, payload);
      }

      setIsFormOpen(false);
      setEditingSetlist(null);
    } catch (error) {
      setFormError(error instanceof Error ? error.message : 'Failed to save setlist.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleConfirmDelete = async () => {
    if (!pendingDelete) {
      return;
    }

    setDeleteError(null);
    setIsDeleting(true);

    try {
      await onDeleteSetlist(pendingDelete.setlistId);
      setPendingDelete(null);
    } catch (error) {
      setDeleteError(error instanceof Error ? error.message : 'Failed to delete setlist.');
    } finally {
      setIsDeleting(false);
    }
  };

  return (
    <>
      <div className="flex h-full min-h-0 w-full flex-col">
        <div className="border-b border-zinc-800/70 px-5 py-4">
          <RitualButton variant="primary" size="sm" className="w-full" onClick={openCreateModal}>
            New Ritual
          </RitualButton>
        </div>

        {panelMessage && (
          <div className="border-b border-red-900/45 bg-red-950/20 px-5 py-3 text-xs text-red-300">
            {panelMessage}
          </div>
        )}

        <div className="min-h-0 flex-1 overflow-y-auto">
          {isLoading ? (
            <div className="py-12 text-center text-sm text-zinc-500">Loading setlists...</div>
          ) : setlists.length === 0 ? (
            <div className="py-14 text-center text-sm text-zinc-500">No setlists yet. Create your first ritual set.</div>
          ) : (
            <div className="space-y-3 px-5 py-4">
              {setlists.map(setlist => (
                <SetlistCard
                  key={setlist.setlistId}
                  setlist={setlist}
                  isSelected={selectedSetlistId === setlist.setlistId}
                  isExpanded={expandedId === setlist.setlistId}
                  onSelect={id => setExpandedId(prev => (prev === id ? null : id))}
                  onEdit={openEditModal}
                  onDelete={setPendingDelete}
                  onOpenEditor={onEditSetlistSongs}
                  onBeginRitual={onBeginRitual}
                />
              ))}
            </div>
          )}
        </div>
      </div>

      <SetlistFormModal
        isOpen={isFormOpen}
        mode={modalMode}
        setlist={editingSetlist}
        isSubmitting={isSubmitting}
        submitError={formError}
        onClose={closeFormModal}
        onSubmit={handleSubmit}
      />

      <ConfirmDialog
        isOpen={!!pendingDelete}
        title="Delete Ritual"
        message={
          pendingDelete
            ? `Delete "${pendingDelete.name}"? This removes the setlist and its song order.`
            : ''
        }
        confirmLabel="Delete Ritual"
        isConfirming={isDeleting}
        onCancel={() => {
          if (!isDeleting) {
            setPendingDelete(null);
            setDeleteError(null);
          }
        }}
        onConfirm={handleConfirmDelete}
      />

      {deleteError && (
        <div className="border-t border-red-900/50 bg-red-950/20 px-5 py-3 text-xs text-red-300">
          {deleteError}
        </div>
      )}
    </>
  );
}
