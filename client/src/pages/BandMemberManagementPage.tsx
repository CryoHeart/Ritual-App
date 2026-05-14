import { useEffect, useState } from 'react';
import { getBandMembers, addBandMember, type BandMember } from '../api/bandsApi';

interface BandMemberManagementPageProps {
  bandId: string;
  bandName: string;
}

export default function BandMemberManagementPage({ bandId, bandName }: BandMemberManagementPageProps) {
  const [members, setMembers] = useState<BandMember[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [email, setEmail] = useState('');
  const [role, setRole] = useState<'RitualLeader' | 'BandMember'>('BandMember');
  const [instrument, setInstrument] = useState('');
  const [adding, setAdding] = useState(false);
  const [addError, setAddError] = useState<string | null>(null);
  const [addSuccess, setAddSuccess] = useState(false);

  const loadMembers = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await getBandMembers(bandId);
      setMembers(data);
    } catch (e) {
      setError(String(e));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { loadMembers(); }, [bandId]);

  const handleAdd = async (e: React.FormEvent) => {
    e.preventDefault();
    setAdding(true);
    setAddError(null);
    setAddSuccess(false);
    try {
      await addBandMember(bandId, email.trim(), role, instrument.trim() || undefined);
      setEmail('');
      setInstrument('');
      setRole('BandMember');
      setAddSuccess(true);
      await loadMembers();
    } catch (e) {
      setAddError(String(e));
    } finally {
      setAdding(false);
    }
  };

  return (
    <div className="p-6 max-w-2xl mx-auto">
      <h2 className="text-xl font-bold mb-6">Band Members — {bandName}</h2>

      <div className="bg-gray-900 rounded-xl p-5 mb-8">
        <h3 className="font-semibold mb-4">Add Member</h3>
        <form onSubmit={handleAdd} className="space-y-3">
          <div>
            <label className="block text-sm text-gray-400 mb-1">Email</label>
            <input
              type="email"
              required
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="w-full bg-gray-800 border border-gray-700 rounded px-3 py-2 text-sm"
              placeholder="member@example.com"
            />
          </div>
          <div>
            <label className="block text-sm text-gray-400 mb-1">Role</label>
            <select
              value={role}
              onChange={(e) => setRole(e.target.value as 'RitualLeader' | 'BandMember')}
              className="w-full bg-gray-800 border border-gray-700 rounded px-3 py-2 text-sm"
            >
              <option value="BandMember">Band Member</option>
              <option value="RitualLeader">Ritual Leader</option>
            </select>
          </div>
          <div>
            <label className="block text-sm text-gray-400 mb-1">Instrument (optional)</label>
            <input
              type="text"
              value={instrument}
              onChange={(e) => setInstrument(e.target.value)}
              className="w-full bg-gray-800 border border-gray-700 rounded px-3 py-2 text-sm"
              placeholder="Guitar, Drums, etc."
            />
          </div>
          {addError && <p className="text-red-400 text-sm">{addError}</p>}
          {addSuccess && <p className="text-green-400 text-sm">Member added successfully!</p>}
          <button
            type="submit"
            disabled={adding}
            className="bg-indigo-600 hover:bg-indigo-500 disabled:opacity-50 px-5 py-2 rounded font-medium text-sm"
          >
            {adding ? 'Adding...' : 'Add Member'}
          </button>
        </form>
      </div>

      <div>
        <h3 className="font-semibold mb-4">Current Members</h3>
        {loading ? (
          <p className="text-gray-500">Loading...</p>
        ) : error ? (
          <p className="text-red-400">{error}</p>
        ) : members.length === 0 ? (
          <p className="text-gray-500 italic">No members yet.</p>
        ) : (
          <ul className="space-y-2">
            {members.map((m) => (
              <li key={m.bandMemberId} className="bg-gray-900 rounded-lg px-4 py-3 flex justify-between items-center">
                <div>
                  <p className="font-medium">{m.displayName}</p>
                  <p className="text-sm text-gray-400">{m.email}{m.instrument ? ` — ${m.instrument}` : ''}</p>
                </div>
                <span className={`text-xs px-2 py-1 rounded ${m.role === 'RitualLeader' ? 'bg-indigo-700 text-indigo-100' : 'bg-gray-700 text-gray-300'}`}>
                  {m.role === 'RitualLeader' ? 'Ritual Leader' : 'Band Member'}
                </span>
              </li>
            ))}
          </ul>
        )}
      </div>
    </div>
  );
}
