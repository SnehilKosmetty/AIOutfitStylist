import { Link, Outlet, useNavigate } from 'react-router-dom';
import { LayoutDashboard, LogOut, Shirt } from 'lucide-react';
import { useEffect } from 'react';
import { clearAuth } from '../lib/auth';

export function AdminLayout() {
  const navigate = useNavigate();

  useEffect(() => {
    const hadDarkClass = document.documentElement.classList.contains('dark');
    document.documentElement.classList.add('dark');
    return () => {
      document.documentElement.classList.toggle('dark', hadDarkClass);
    };
  }, []);

  return (
    <div className="min-h-screen bg-slate-950 text-slate-100">
      <header className="border-b border-slate-800 bg-slate-900">
        <div className="mx-auto flex h-16 max-w-7xl items-center justify-between px-4 sm:px-6">
          <Link to="/admin" className="flex items-center gap-3 font-semibold">
            <span className="grid h-10 w-10 place-items-center rounded-md bg-teal-700 text-white"><LayoutDashboard size={22} /></span>
            <span>Admin Dashboard</span>
          </Link>
          <div className="flex items-center gap-2">
            <Link className="inline-flex items-center justify-center gap-2 rounded-md border border-slate-700 bg-slate-900 px-4 py-2 text-sm font-semibold text-slate-100 transition hover:bg-slate-800" to="/">
              <Shirt size={16} /> User App
            </Link>
            <button className="inline-flex items-center justify-center gap-2 rounded-md border border-slate-700 bg-slate-900 px-3 py-2 text-sm font-semibold text-slate-100 transition hover:bg-slate-800" onClick={() => { clearAuth(); navigate('/admin/login'); }} aria-label="Sign out">
              <LogOut size={18} />
            </button>
          </div>
        </div>
      </header>
      <main className="mx-auto max-w-7xl px-4 py-6 sm:px-6">
        <Outlet />
      </main>
    </div>
  );
}
