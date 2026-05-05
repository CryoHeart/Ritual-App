import { useEffect, useMemo, useState } from 'react';
import type { Song } from '../types/song';

function fmt(seconds?: number): string {
  if (!seconds) return '—';
  const m = Math.floor(seconds / 60);
  const s = seconds % 60;
  return `${m}:${s.toString().padStart(2, '0')}`;
}

interface Props {
  songs: Song[];
}

export function SongList({ songs }: Props) {
  const grouped = useMemo(() => {
    const map = new Map<string, { title: string; songs: Song[]; isUnassigned: boolean }>();

    for (const song of songs) {
      const rawTitle = song.albumTitle?.trim();
      const title = rawTitle && rawTitle.length > 0 ? rawTitle : 'Unassigned';
      const isUnassigned = title === 'Unassigned';
      const key = isUnassigned ? '__unassigned__' : `album:${title.toLowerCase()}`;

      if (!map.has(key)) {
        map.set(key, { title, songs: [], isUnassigned });
      }
      map.get(key)?.songs.push(song);
    }

    for (const group of map.values()) {
      group.songs.sort((a, b) => (a.albumTrackNumber ?? Infinity) - (b.albumTrackNumber ?? Infinity));
    }

    return Array.from(map.entries())
      .map(([key, value]) => ({ key, ...value }))
      .sort((a, b) => {
        if (a.isUnassigned && !b.isUnassigned) return 1;
        if (!a.isUnassigned && b.isUnassigned) return -1;
        return a.title.localeCompare(b.title);
      });
  }, [songs]);

  const [collapsedKeys, setCollapsedKeys] = useState<Set<string>>(
    () => new Set(grouped.map(group => group.key))
  );

  useEffect(() => {
    setCollapsedKeys(new Set(grouped.map(group => group.key)));
  }, [grouped]);

  if (songs.length === 0) {
    return (
      <div className="py-14 text-center text-sm text-zinc-500">
        No songs found.
      </div>
    );
  }

  return (
    <div className="divide-y divide-zinc-800/60">
      {grouped.map(group => {
        const isCollapsed = collapsedKeys.has(group.key);
        return (
          <div key={group.key}>
            <button
              onClick={() => {
                setCollapsedKeys(prev => {
                  const next = new Set(prev);
                  if (next.has(group.key)) next.delete(group.key);
                  else next.add(group.key);
                  return next;
                });
              }}
              className="flex w-full items-center gap-2 bg-zinc-900/60 px-5 py-3 text-left transition-colors hover:bg-zinc-800/50"
            >
              <span className={`text-xs font-bold uppercase tracking-[0.22em] ${group.isUnassigned ? 'text-zinc-500' : 'text-red-400'}`}>
                {group.title}
              </span>
              <span className="ml-auto text-xs text-zinc-600">{group.songs.length} tracks</span>
              <span className="ml-2 text-xs text-zinc-600">{isCollapsed ? '▶' : '▼'}</span>
            </button>

            {!isCollapsed && (
              <div className="divide-y divide-zinc-800/50">
                {group.songs.map((song, i) => (
                  <div
                    key={song.songId}
                    className="group flex items-center gap-4 px-5 py-3 transition-colors hover:bg-zinc-800/30"
                  >
                    <span className="inline-flex w-8 shrink-0 items-center justify-center rounded-md border border-zinc-700 bg-zinc-950/80 py-1 text-[11px] font-mono text-zinc-400">
                      {String(song.albumTrackNumber ?? i + 1).padStart(2, '0')}
                    </span>

                    <div className="min-w-0 flex-1">
                      <p className="truncate text-sm font-semibold text-zinc-100 transition group-hover:text-white">{song.title}</p>
                      <div className="mt-1 flex flex-wrap items-center gap-2 text-xs text-zinc-500">
                        {song.tuning ? <span className="rounded border border-zinc-700 px-1.5 py-0.5 text-[10px] uppercase tracking-[0.12em] text-zinc-400">{song.tuning}</span> : null}
                        {song.songKey ? <span>Key {song.songKey}</span> : null}
                      </div>
                    </div>

                    <div className="flex shrink-0 items-center gap-3 text-xs text-zinc-500">
                      {song.bpm != null && <span className="rounded border border-zinc-700 px-2 py-1 text-[10px] tracking-[0.16em]">{song.bpm} BPM</span>}
                      <span className="w-12 text-right font-mono tabular-nums text-zinc-300">{fmt(song.durationSeconds)}</span>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        );
      })}
    </div>
  );
}
