import * as signalR from '@microsoft/signalr';
import type { LiveSession } from '../types/liveSession';

const HUB_URL = 'http://localhost:5000/hubs/live-ritual';

function getToken(): string {
  try {
    const raw = localStorage.getItem('ritual_user');
    if (!raw) return '';
    return JSON.parse(raw)?.token ?? '';
  } catch {
    return '';
  }
}

export function createLiveRitualConnection(): signalR.HubConnection {
  return new signalR.HubConnectionBuilder()
    .withUrl(HUB_URL, {
      accessTokenFactory: () => getToken(),
    })
    .withAutomaticReconnect()
    .build();
}

export interface LiveRitualEvents {
  onRitualStarted?: (session: LiveSession) => void;
  onRitualUpdated?: (session: LiveSession) => void;
  onRitualEnded?: (session: LiveSession) => void;
}

export function registerLiveRitualEvents(
  connection: signalR.HubConnection,
  events: LiveRitualEvents
) {
  if (events.onRitualStarted) connection.on('RitualStarted', events.onRitualStarted);
  if (events.onRitualUpdated) connection.on('RitualUpdated', events.onRitualUpdated);
  if (events.onRitualEnded) connection.on('RitualEnded', events.onRitualEnded);
}

export function unregisterLiveRitualEvents(connection: signalR.HubConnection) {
  connection.off('RitualStarted');
  connection.off('RitualUpdated');
  connection.off('RitualEnded');
}

export async function joinBandGroup(connection: signalR.HubConnection, bandId: string) {
  await connection.invoke('JoinBand', bandId);
}

export async function leaveBandGroup(connection: signalR.HubConnection, bandId: string) {
  await connection.invoke('LeaveBand', bandId);
}

export async function joinLiveSessionGroup(connection: signalR.HubConnection, liveSessionId: string) {
  await connection.invoke('JoinLiveSession', liveSessionId);
}

export async function leaveLiveSessionGroup(connection: signalR.HubConnection, liveSessionId: string) {
  await connection.invoke('LeaveLiveSession', liveSessionId);
}
