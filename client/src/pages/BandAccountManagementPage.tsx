import { useState, type FormEvent } from 'react';
import loginBackground from '../assets/login-background.png';
import { updateEmailApi, updatePasswordApi } from '../api/authApi';
import { updateBandName } from '../api/bandsApi';
import { AppShell } from '../components/AppShell';
import { RitualButton } from '../components/ui/RitualButton';
import { RitualCard } from '../components/ui/RitualCard';
import { useAuth } from '../context/AuthContext';

interface Props {
  bandId: string;
  bandName?: string;
  onBandRenamed: (newName: string) => void;
  onBack: () => void;
}

export function BandAccountManagementPage({ bandId, bandName, onBandRenamed, onBack }: Props) {
  const { user, login } = useAuth();

  const [newBandName, setNewBandName] = useState(bandName ?? '');
  const [bandNameLoading, setBandNameLoading] = useState(false);
  const [bandNameError, setBandNameError] = useState<string | null>(null);
  const [bandNameSuccess, setBandNameSuccess] = useState<string | null>(null);

  const [newEmail, setNewEmail] = useState(user?.email ?? '');
  const [emailPassword, setEmailPassword] = useState('');
  const [emailLoading, setEmailLoading] = useState(false);
  const [emailError, setEmailError] = useState<string | null>(null);
  const [emailSuccess, setEmailSuccess] = useState<string | null>(null);

  const [currentPassword, setCurrentPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [passwordLoading, setPasswordLoading] = useState(false);
  const [passwordError, setPasswordError] = useState<string | null>(null);
  const [passwordSuccess, setPasswordSuccess] = useState<string | null>(null);

  const handleBandNameSubmit = async (event: FormEvent) => {
    event.preventDefault();
    setBandNameError(null);
    setBandNameSuccess(null);

    const trimmed = newBandName.trim();
    if (!trimmed) {
      setBandNameError('Band name is required.');
      return;
    }

    setBandNameLoading(true);
    try {
      const updated = await updateBandName(bandId, trimmed);
      setNewBandName(updated.name);
      onBandRenamed(updated.name);
      setBandNameSuccess('Band name updated.');
    } catch (error) {
      setBandNameError(error instanceof Error ? error.message : 'Failed to update band name.');
    } finally {
      setBandNameLoading(false);
    }
  };

  const handleEmailSubmit = async (event: FormEvent) => {
    event.preventDefault();
    setEmailError(null);
    setEmailSuccess(null);

    if (!user?.userId) {
      setEmailError('You must be logged in.');
      return;
    }

    const trimmed = newEmail.trim().toLowerCase();
    if (!trimmed) {
      setEmailError('Email is required.');
      return;
    }

    if (!emailPassword) {
      setEmailError('Current password is required.');
      return;
    }

    setEmailLoading(true);
    try {
      const updated = await updateEmailApi(user.userId, trimmed, emailPassword);
      login(updated);
      setEmailPassword('');
      setEmailSuccess('Email updated.');
    } catch (error) {
      setEmailError(error instanceof Error ? error.message : 'Failed to update email.');
    } finally {
      setEmailLoading(false);
    }
  };

  const handlePasswordSubmit = async (event: FormEvent) => {
    event.preventDefault();
    setPasswordError(null);
    setPasswordSuccess(null);

    if (!user?.userId) {
      setPasswordError('You must be logged in.');
      return;
    }

    if (!currentPassword) {
      setPasswordError('Current password is required.');
      return;
    }

    if (!newPassword) {
      setPasswordError('New password is required.');
      return;
    }

    if (newPassword.length < 8) {
      setPasswordError('New password must be at least 8 characters.');
      return;
    }

    if (newPassword !== confirmPassword) {
      setPasswordError('New password and confirmation do not match.');
      return;
    }

    setPasswordLoading(true);
    try {
      await updatePasswordApi(user.userId, currentPassword, newPassword);
      setCurrentPassword('');
      setNewPassword('');
      setConfirmPassword('');
      setPasswordSuccess('Password updated.');
    } catch (error) {
      setPasswordError(error instanceof Error ? error.message : 'Failed to update password.');
    } finally {
      setPasswordLoading(false);
    }
  };

  return (
    <AppShell
      selectedBandName={bandName}
      backgroundImageSrc={loginBackground}
      centerSlot={<h1 className="text-xl font-bold uppercase tracking-[0.2em] text-zinc-100">Band account management</h1>}
      onLogoClick={onBack}
    >
      <div className="mx-auto flex h-full w-full max-w-5xl flex-col px-8 py-6">
        <div className="mb-5 flex justify-end">
          <button
            onClick={onBack}
            className="rounded-lg border border-zinc-700 bg-zinc-900/70 px-4 py-2 text-xs font-semibold uppercase tracking-[0.2em] text-zinc-200 transition hover:border-zinc-500 hover:text-white"
          >
            Back
          </button>
        </div>

        <div className="min-h-0 flex-1 overflow-y-auto">
          <div className="space-y-5 pb-4">
            <RitualCard>
              <h2 className="text-sm font-black uppercase tracking-[0.2em] text-zinc-200">Band Name</h2>
              <p className="mt-2 text-sm text-zinc-500">Update how your band appears across the app.</p>
              <form onSubmit={handleBandNameSubmit} className="mt-4 space-y-3">
                <input
                  value={newBandName}
                  onChange={e => setNewBandName(e.target.value)}
                  maxLength={100}
                  className="w-full rounded-lg border border-zinc-700 bg-zinc-900 px-4 py-2.5 text-base text-zinc-100 outline-none transition focus:border-red-700 focus:ring-1 focus:ring-red-900"
                  placeholder="Band name"
                />
                {bandNameError ? <p className="text-sm text-red-300">{bandNameError}</p> : null}
                {bandNameSuccess ? <p className="text-sm text-emerald-300">{bandNameSuccess}</p> : null}
                <RitualButton type="submit" variant="primary" size="md" disabled={bandNameLoading}>
                  {bandNameLoading ? 'Saving...' : 'Save band name'}
                </RitualButton>
              </form>
            </RitualCard>

            <RitualCard>
              <h2 className="text-sm font-black uppercase tracking-[0.2em] text-zinc-200">Email</h2>
              <p className="mt-2 text-sm text-zinc-500">Change the account email used for signing in.</p>
              <form onSubmit={handleEmailSubmit} className="mt-4 space-y-3">
                <input
                  type="email"
                  value={newEmail}
                  onChange={e => setNewEmail(e.target.value)}
                  className="w-full rounded-lg border border-zinc-700 bg-zinc-900 px-4 py-2.5 text-base text-zinc-100 outline-none transition focus:border-red-700 focus:ring-1 focus:ring-red-900"
                  placeholder="New email"
                />
                <input
                  type="password"
                  value={emailPassword}
                  onChange={e => setEmailPassword(e.target.value)}
                  className="w-full rounded-lg border border-zinc-700 bg-zinc-900 px-4 py-2.5 text-base text-zinc-100 outline-none transition focus:border-red-700 focus:ring-1 focus:ring-red-900"
                  placeholder="Current password"
                />
                {emailError ? <p className="text-sm text-red-300">{emailError}</p> : null}
                {emailSuccess ? <p className="text-sm text-emerald-300">{emailSuccess}</p> : null}
                <RitualButton type="submit" variant="primary" size="md" disabled={emailLoading}>
                  {emailLoading ? 'Saving...' : 'Save email'}
                </RitualButton>
              </form>
            </RitualCard>

            <RitualCard>
              <h2 className="text-sm font-black uppercase tracking-[0.2em] text-zinc-200">Password</h2>
              <p className="mt-2 text-sm text-zinc-500">Use at least 8 characters for your new password.</p>
              <form onSubmit={handlePasswordSubmit} className="mt-4 space-y-3">
                <input
                  type="password"
                  value={currentPassword}
                  onChange={e => setCurrentPassword(e.target.value)}
                  className="w-full rounded-lg border border-zinc-700 bg-zinc-900 px-4 py-2.5 text-base text-zinc-100 outline-none transition focus:border-red-700 focus:ring-1 focus:ring-red-900"
                  placeholder="Current password"
                />
                <input
                  type="password"
                  value={newPassword}
                  onChange={e => setNewPassword(e.target.value)}
                  className="w-full rounded-lg border border-zinc-700 bg-zinc-900 px-4 py-2.5 text-base text-zinc-100 outline-none transition focus:border-red-700 focus:ring-1 focus:ring-red-900"
                  placeholder="New password"
                />
                <input
                  type="password"
                  value={confirmPassword}
                  onChange={e => setConfirmPassword(e.target.value)}
                  className="w-full rounded-lg border border-zinc-700 bg-zinc-900 px-4 py-2.5 text-base text-zinc-100 outline-none transition focus:border-red-700 focus:ring-1 focus:ring-red-900"
                  placeholder="Confirm new password"
                />
                {passwordError ? <p className="text-sm text-red-300">{passwordError}</p> : null}
                {passwordSuccess ? <p className="text-sm text-emerald-300">{passwordSuccess}</p> : null}
                <RitualButton type="submit" variant="primary" size="md" disabled={passwordLoading}>
                  {passwordLoading ? 'Saving...' : 'Save password'}
                </RitualButton>
              </form>
            </RitualCard>
          </div>
        </div>
      </div>
    </AppShell>
  );
}
