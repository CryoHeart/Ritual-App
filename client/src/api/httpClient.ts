const BASE_URL = 'http://localhost:5000';

export async function apiFetch<T>(path: string, options?: RequestInit): Promise<T> {
  const response = await fetch(`${BASE_URL}${path}`, options);

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

  return response.json() as Promise<T>;
}
