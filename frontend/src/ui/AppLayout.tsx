import { Link, NavLink, Outlet, useNavigate } from 'react-router-dom';
import { Camera, Heart, LayoutDashboard, LogOut, Moon, Shirt, Sparkles, Sun, User } from 'lucide-react';
import { useEffect, useState } from 'react';
import { clearAuth, currentUser } from '../lib/auth';

const navItems = [
  { to: '/', label: 'Dashboard', icon: LayoutDashboard },
  { to: '/upload', label: 'Upload Photo', icon: Camera },
  { to: '/generator', label: 'Generator', icon: Sparkles },
  { to: '/saved', label: 'Saved', icon: Heart },
  { to: '/profile', label: 'Profile', icon: User }
];

export function AppLayout() {
  const navigate = useNavigate();
  const [dark, setDark] = useState(() => localStorage.getItem('aios_theme') === 'dark');
  const user = currentUser();

  useEffect(() => {
    document.documentElement.classList.toggle('dark', dark);
    localStorage.setItem('aios_theme', dark ? 'dark' : 'light');
  }, [dark]);

  return (
    <div className="min-h-screen bg-mist text-slate-900 dark:bg-slate-950 dark:text-slate-100">
      <aside className="fixed inset-y-0 left-0 hidden w-64 border-r border-slate-200 bg-white px-4 py-5 dark:border-slate-800 dark:bg-slate-900 lg:block">
        <Link to="/" className="flex items-center gap-3 font-semibold">
          <span className="grid h-10 w-10 place-items-center rounded-md bg-teal-700 text-white"><Shirt size={22} /></span>
          <span>AI Outfit Stylist</span>
        </Link>
        <nav className="mt-8 space-y-1">
          {navItems.map((item) => (
            <NavLink key={item.to} to={item.to} end={item.to === '/'} className={({ isActive }) => `flex items-center gap-3 rounded-md px-3 py-2 text-sm ${isActive ? 'bg-teal-700 text-white' : 'text-slate-600 hover:bg-slate-100 dark:text-slate-300 dark:hover:bg-slate-800'}`}>
              <item.icon size={18} /> {item.label}
            </NavLink>
          ))}
        </nav>
      </aside>
      <main className="lg:pl-64">
        <header className="sticky top-0 z-10 flex h-16 items-center justify-between border-b border-slate-200 bg-white/90 px-4 backdrop-blur dark:border-slate-800 dark:bg-slate-900/90 sm:px-6">
          <div>
            <p className="text-xs uppercase tracking-wider text-teal-700 dark:text-teal-300">Styled for the moment</p>
            <h1 className="text-lg font-semibold">Welcome, {user?.firstName ?? 'there'}</h1>
          </div>
          <div className="flex items-center gap-2">
            <button className="button-secondary px-3" onClick={() => setDark((value) => !value)} aria-label="Toggle theme">
              {dark ? <Sun size={18} /> : <Moon size={18} />}
            </button>
            <button className="button-secondary px-3" onClick={() => { clearAuth(); navigate('/login'); }} aria-label="Sign out">
              <LogOut size={18} />
            </button>
          </div>
        </header>
        <div className="mx-auto max-w-7xl px-4 py-6 sm:px-6">
          <Outlet />
        </div>
        <nav className="fixed inset-x-0 bottom-0 grid grid-cols-5 border-t border-slate-200 bg-white dark:border-slate-800 dark:bg-slate-900 lg:hidden">
          {navItems.map((item) => (
            <NavLink key={item.to} to={item.to} end={item.to === '/'} className={({ isActive }) => `flex flex-col items-center gap-1 px-2 py-2 text-xs ${isActive ? 'text-teal-700 dark:text-teal-300' : 'text-slate-500'}`}>
              <item.icon size={18} /> {item.label}
            </NavLink>
          ))}
        </nav>
      </main>
    </div>
  );
}
