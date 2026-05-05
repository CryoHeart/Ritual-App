import { useState, type FormEvent } from 'react';
import { useAuth } from '../context/AuthContext';
import { loginApi } from '../api/authApi';
import ritualHeaderCombined from '../assets/ritual-full.png';
import loginBackground from '../assets/login-background.png';
import { RitualButton } from '../components/ui/RitualButton';

interface LoginPageProps {
  onSwitchToRegister: () => void;
}

export function LoginPage({ onSwitchToRegister }: LoginPageProps) {
  const { login } = useAuth();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    setLoading(true);
    try {
      const user = await loginApi(email, password);
      login(user);
    } catch {
      setError('Invalid email or password.');
    } finally {
      setLoading(false);
    }
  }

  return (
    <div
      className="relative flex h-screen overflow-hidden flex-col items-center justify-center bg-cover bg-center bg-no-repeat text-white antialiased"
      style={{ backgroundImage: `url(${loginBackground})` }}
    >
      <div className="relative z-10 flex w-full max-w-5xl flex-col gap-8 px-6">
        {/* Logo */}
        <div className="flex justify-center">
          <img
            src={ritualHeaderCombined}
            alt="Ritual"
            className="h-auto w-[min(92vw,880px)] object-contain drop-shadow-[0_0_18px_rgba(239,68,68,0.45)]"
          />
        </div>

        {/* Card */}
        <form
          onSubmit={handleSubmit}
          className="mx-auto flex w-full max-w-sm flex-col gap-5 rounded-2xl border border-zinc-800 bg-zinc-950/80 p-8 shadow-2xl backdrop-blur"
        >
          <h1 className="text-center text-sm font-black uppercase tracking-[0.3em] text-zinc-400">
            Sign in
          </h1>

          <div className="flex flex-col gap-1.5">
            <label className="text-xs font-bold uppercase tracking-[0.2em] text-zinc-500">
              Email
            </label>
            <input
              type="email"
              autoComplete="email"
              required
              value={email}
              onChange={e => setEmail(e.target.value)}
              className="rounded-lg border border-zinc-700 bg-zinc-900 px-4 py-2.5 text-base text-zinc-100 placeholder-zinc-600 outline-none transition focus:border-red-700 focus:ring-1 focus:ring-red-900"
              placeholder="you@example.com"
            />
          </div>

          <div className="flex flex-col gap-1.5">
            <label className="text-xs font-bold uppercase tracking-[0.2em] text-zinc-500">
              Password
            </label>
            <input
              type="password"
              autoComplete="current-password"
              required
              value={password}
              onChange={e => setPassword(e.target.value)}
              className="rounded-lg border border-zinc-700 bg-zinc-900 px-4 py-2.5 text-base text-zinc-100 placeholder-zinc-600 outline-none transition focus:border-red-700 focus:ring-1 focus:ring-red-900"
              placeholder="••••••••"
            />
          </div>

          {error && (
            <p className="rounded-lg border border-red-900 bg-red-950/60 px-4 py-2 text-sm text-red-300">
              {error}
            </p>
          )}

          <RitualButton type="submit" variant="primary" size="lg" disabled={loading}>
            {loading ? 'Signing in…' : 'Sign in'}
          </RitualButton>

          <button
            type="button"
            onClick={onSwitchToRegister}
            className="text-center text-sm uppercase tracking-[0.2em] text-zinc-400 transition hover:text-zinc-100"
          >
            Need an account? Register
          </button>
        </form>
      </div>
    </div>
  );
}
