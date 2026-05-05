import { RitualButton } from './RitualButton';
import { RitualCard } from './RitualCard';

interface ConfirmDialogProps {
  isOpen: boolean;
  title: string;
  message: string;
  confirmLabel?: string;
  cancelLabel?: string;
  isConfirming?: boolean;
  onCancel: () => void;
  onConfirm: () => void;
}

export function ConfirmDialog({
  isOpen,
  title,
  message,
  confirmLabel = 'Delete',
  cancelLabel = 'Cancel',
  isConfirming = false,
  onCancel,
  onConfirm
}: ConfirmDialogProps) {
  if (!isOpen) {
    return null;
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-zinc-950/80 px-4 backdrop-blur-sm">
      <RitualCard className="w-full max-w-lg border-red-900/50">
        <p className="text-[10px] font-semibold uppercase tracking-[0.36em] text-red-500">Confirm</p>
        <h3 className="mt-2 text-xl font-semibold text-zinc-100">{title}</h3>
        <p className="mt-3 text-sm leading-relaxed text-zinc-300">{message}</p>

        <div className="mt-6 flex items-center justify-end gap-2">
          <RitualButton variant="neutral" size="sm" onClick={onCancel} disabled={isConfirming}>
            {cancelLabel}
          </RitualButton>
          <RitualButton variant="danger" size="sm" onClick={onConfirm} disabled={isConfirming}>
            {isConfirming ? 'Deleting...' : confirmLabel}
          </RitualButton>
        </div>
      </RitualCard>
    </div>
  );
}
