import { useQuery } from '@tanstack/react-query';
import { BarChart3, Camera, Heart, Shirt, Sparkles, Users } from 'lucide-react';
import { apiClient } from '../lib/api';

export function AdminDashboardPage() {
  const dashboard = useQuery({ queryKey: ['admin-dashboard'], queryFn: apiClient.adminDashboard });
  const data = dashboard.data;

  if (dashboard.isLoading) {
    return <div className="rounded-md border border-slate-800 bg-slate-900 p-5 text-slate-100">Loading admin dashboard...</div>;
  }

  if (!data) {
    return <div className="rounded-md border border-red-900 bg-red-950 p-5 text-red-200">Unable to load admin dashboard.</div>;
  }

  const stats = [
    { label: 'Users', value: data.totalUsers, icon: Users },
    { label: 'Photos', value: data.totalPhotos, icon: Camera },
    { label: 'AI analyses', value: data.totalAnalyses, icon: Sparkles },
    { label: 'Outfits', value: data.totalOutfits, icon: Shirt },
    { label: 'Saved outfits', value: data.savedOutfits, icon: Heart },
    { label: 'Generated today', value: data.generatedToday, icon: BarChart3 }
  ];

  return (
    <div className="space-y-6">
      <div>
        <p className="text-xs uppercase tracking-wider text-teal-300">Admin</p>
        <h2 className="text-2xl font-semibold">Admin Dashboard</h2>
      </div>
      <section className="grid gap-4 md:grid-cols-3">
        {stats.map((stat) => (
          <article key={stat.label} className="rounded-md border border-slate-800 bg-slate-900 p-4 text-slate-100">
            <stat.icon className="text-teal-300" size={20} />
            <p className="mt-3 text-sm text-slate-400">{stat.label}</p>
            <p className="text-3xl font-semibold">{stat.value}</p>
          </article>
        ))}
      </section>
      <section className="overflow-hidden rounded-md border border-slate-800 bg-slate-900 text-slate-100">
        <div className="border-b border-slate-800 p-4">
          <h3 className="font-semibold">Recent User Activity</h3>
        </div>
        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm">
            <thead className="bg-slate-950 text-slate-400">
              <tr>
                <th className="px-4 py-3">User</th>
                <th className="px-4 py-3">Email</th>
                <th className="px-4 py-3">Gender</th>
                <th className="px-4 py-3">Age</th>
                <th className="px-4 py-3">Height</th>
                <th className="px-4 py-3">Weight</th>
                <th className="px-4 py-3">Size</th>
                <th className="px-4 py-3">Style</th>
                <th className="px-4 py-3">Budget</th>
                <th className="px-4 py-3">Joined</th>
                <th className="px-4 py-3">Photos</th>
                <th className="px-4 py-3">Outfits</th>
                <th className="px-4 py-3">Saved</th>
              </tr>
            </thead>
            <tbody>
              {data.recentUsers.map((user) => (
                <tr key={user.userId} className="border-t border-slate-800">
                  <td className="px-4 py-3 font-medium">{user.name}</td>
                  <td className="px-4 py-3 text-slate-400">{user.email}</td>
                  <td className="px-4 py-3 text-slate-400">{user.gender}</td>
                  <td className="px-4 py-3 text-slate-400">{user.age ?? '-'}</td>
                  <td className="px-4 py-3 text-slate-400">{user.height ?? '-'}</td>
                  <td className="px-4 py-3 text-slate-400">{user.weight ? `${user.weight} lb` : '-'}</td>
                  <td className="px-4 py-3 text-slate-400">{user.clothingSize ?? '-'}</td>
                  <td className="px-4 py-3 text-slate-400">{user.preferredStyle ?? '-'}</td>
                  <td className="px-4 py-3 text-slate-400">{formatBudget(user.budgetMin, user.budgetMax)}</td>
                  <td className="px-4 py-3 text-slate-400">{new Date(user.createdAtUtc).toLocaleDateString()}</td>
                  <td className="px-4 py-3">{user.photos}</td>
                  <td className="px-4 py-3">{user.outfits}</td>
                  <td className="px-4 py-3">{user.savedOutfits}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </section>
    </div>
  );
}

function formatBudget(min?: number, max?: number) {
  if (min && max) return `$${min} - $${max}`;
  if (min) return `From $${min}`;
  if (max) return `Up to $${max}`;
  return '-';
}
