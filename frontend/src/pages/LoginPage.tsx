import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation } from '@tanstack/react-query';
import { Eye, EyeOff, Sparkles } from 'lucide-react';
import type React from 'react';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import toast from 'react-hot-toast';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { z } from 'zod';
import { apiClient } from '../lib/api';
import { persistAuth } from '../lib/auth';
import { isAdminUser } from '../lib/admin';
import { GoogleSignInButton } from '../ui/GoogleSignInButton';

const schema = z.object({ email: z.string().email(), password: z.string().min(1) });
type FormValues = z.infer<typeof schema>;

type LoginPageProps = {
  mode?: 'user' | 'admin';
};

export function LoginPage({ mode = 'user' }: LoginPageProps) {
  const navigate = useNavigate();
  const location = useLocation();
  const redirectTo = (location.state as { from?: string } | null)?.from ?? (mode === 'admin' ? '/admin' : '/');
  const [showPassword, setShowPassword] = useState(false);
  const form = useForm<FormValues>({ resolver: zodResolver(schema) });
  const mutation = useMutation({
    mutationFn: apiClient.login,
    onSuccess: (data) => {
      persistAuth(data);
      if (mode === 'admin' && !isAdminUser(data.user)) {
        toast.error('This account is not allowed to access Admin.');
        navigate('/login', { replace: true });
        return;
      }

      navigate(redirectTo, { replace: true });
    },
    onError: () => toast.error('Login failed')
  });
  const title = mode === 'admin' ? 'Admin Dashboard' : 'AI Outfit Stylist';

  return (
    <AuthShell mode={mode}>
      <form className="space-y-4" onSubmit={form.handleSubmit((values) => mutation.mutate(values))}>
        <div className="flex items-center gap-3"><Sparkles className="text-teal-700" /><h1 className="text-2xl font-semibold">{title}</h1></div>
        <input className="input" placeholder="Email" {...form.register('email')} />
        <div className="relative">
          <input className="input pr-12" placeholder="Password" type={showPassword ? 'text' : 'password'} {...form.register('password')} />
          <button type="button" className="absolute right-2 top-1/2 -translate-y-1/2 rounded-md p-2 text-slate-500 hover:bg-slate-100 dark:hover:bg-slate-800" onClick={() => setShowPassword((value) => !value)} aria-label={showPassword ? 'Hide password' : 'Show password'}>
            {showPassword ? <EyeOff size={18} /> : <Eye size={18} />}
          </button>
        </div>
        <button className="button w-full" disabled={mutation.isPending}>{mutation.isPending ? 'Signing in...' : 'Sign in'}</button>
        <div className="flex items-center gap-3 text-xs uppercase tracking-wider text-slate-400"><span className="h-px flex-1 bg-slate-200 dark:bg-slate-800" /> Or <span className="h-px flex-1 bg-slate-200 dark:bg-slate-800" /></div>
        <GoogleSignInButton text="signin_with" redirectTo={redirectTo} requireAdmin={mode === 'admin'} />
        {mode === 'admin' ? (
          <p className="text-sm text-slate-500">Need the user app? <Link className="font-semibold text-teal-700" to="/login">User sign in</Link></p>
        ) : (
          <p className="text-sm text-slate-500">New here? <Link className="font-semibold text-teal-700" to="/register">Create an account</Link></p>
        )}
      </form>
    </AuthShell>
  );
}

export function AuthShell({ children, mode = 'user' }: { children: React.ReactNode; mode?: 'user' | 'admin' }) {
  const isAdmin = mode === 'admin';
  return (
    <main className={`grid min-h-screen place-items-center px-4 ${isAdmin ? 'dark bg-slate-950 text-slate-100' : 'bg-mist dark:bg-slate-950'}`}>
      <section className={`grid w-full max-w-5xl overflow-hidden rounded-md border shadow-sm md:grid-cols-[1fr_1.1fr] ${isAdmin ? 'border-slate-800 bg-slate-900' : 'border-slate-200 bg-white dark:border-slate-800 dark:bg-slate-900'}`}>
        <div className="p-6 sm:p-8">{children}</div>
        <div className={`hidden bg-cover bg-center md:block ${isAdmin ? "bg-[url('https://images.unsplash.com/photo-1551434678-e076c223a692?auto=format&fit=crop&w=1200&q=80')]" : "bg-[url('https://images.unsplash.com/photo-1496747611176-843222e1e57c?auto=format&fit=crop&w=1200&q=80')]"}`} />
      </section>
    </main>
  );
}
