import type { AuthResponse, UserProfile } from '../types';

export function persistAuth(response: AuthResponse) {
  localStorage.setItem('aios_token', response.token);
  localStorage.setItem('aios_user', JSON.stringify(response.user));
}

export function clearAuth() {
  localStorage.removeItem('aios_token');
  localStorage.removeItem('aios_user');
}

export function currentUser(): UserProfile | null {
  const raw = localStorage.getItem('aios_user');
  return raw ? (JSON.parse(raw) as UserProfile) : null;
}

export function isAuthenticated() {
  return Boolean(localStorage.getItem('aios_token'));
}
