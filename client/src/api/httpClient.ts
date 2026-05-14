const BASE_URL = 'http://localhost:5000';

function getStoredToken(): string | null {
  try {
    const raw = localStorage.getItem('ritual_user');
    if (!raw) return null;
    const user = JSON.parse(raw);
    return user?.token ?? null;
  } catch {
    return null;
  }
}

export async function apiFetch<T>(path: string, options?: RequestInit): Promise<T> {
  const token = getStoredToken();
  const authHeaders: Record<string, string> = token
    ? { Authorization: `Bearer ${token}` }
    : {};

  let response: Response;
  try {
    response = await fetch(`${BASE_URL}${path}`, {
      ...options,
      headers: {
        ...authHeaders,
        ...(options?.headers as Record<string, string> | undefined),
      },
    });
  } catch (err) {
    const message = err instanceof Error ? err.message : '';
    if (message.toLowerCase().includes('failed to fetch')) {
      throw new Error(`Cannot reach API at ${BASE_URL}. Make sure the backend is running.`);
    }
    throw err;
  }

  if (!response.ok) {
    let message = `HTTP ${response.status}`;
    try {
      const body = await response.json();
      if (body?.error) message = body.error;
    } catch {
      // ignore parse failure
    }
    throw new Error(message);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  const contentType = response.headers.get('content-type') ?? '';
  if (!contentType.includes('application/json')) {
    return undefined as T;
  }

  return response.json() as Promise<T>;
}
