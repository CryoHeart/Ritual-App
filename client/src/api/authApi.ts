import { apiFetch } from './httpClient';
import type { AuthUser } from '../types/auth';

export async function loginApi(email: string, password: string): Promise<AuthUser> {
  return apiFetch<AuthUser>('/api/auth/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password }),
  });
}

export async function registerApi(displayName: string, email: string, password: string): Promise<AuthUser> {
  return apiFetch<AuthUser>('/api/auth/register', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ displayName, email, password }),
  });
}

export async function updateEmailApi(userId: string, newEmail: string, currentPassword: string): Promise<AuthUser> {
  return apiFetch<AuthUser>('/api/auth/email', {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ userId, newEmail, currentPassword }),
  });
}

export async function updatePasswordApi(userId: string, currentPassword: string, newPassword: string): Promise<void> {
  await apiFetch<void>('/api/auth/password', {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ userId, currentPassword, newPassword }),
  });
}
