import type { UserProfile } from '../types';

export function isAdminUser(user: UserProfile | null) {
  const allowedEmails = (import.meta.env.VITE_ADMIN_EMAILS as string | undefined)
    ?.split(',')
    .map((email) => email.trim().toLowerCase())
    .filter(Boolean) ?? [];

  return Boolean(user?.email && allowedEmails.includes(user.email.toLowerCase()));
}
