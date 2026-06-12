import { useQuery } from '@tanstack/react-query';
import { Camera, Heart, Palette, Sparkles } from 'lucide-react';
import { Link } from 'react-router-dom';
import { apiClient } from '../lib/api';
import { OutfitCard } from '../ui/OutfitCard';

export function DashboardPage() {
  const history = useQuery({ queryKey: ['outfits'], queryFn: apiClient.history });
  const latest = history.data?.[0];
  const latestPhotoUrl = localStorage.getItem('aios_latest_photo_url');

  return (
    <div className="space-y-6 pb-20 lg:pb-0">
      <section className="grid gap-4 md:grid-cols-4">
        <Metric icon={Camera} label="Photos" value="Ready" />
        <Metric icon={Palette} label="AI Analysis" value="Vision" />
        <Metric icon={Sparkles} label="Generated" value={String(history.data?.length ?? 0)} />
        <Metric icon={Heart} label="Saved" value={String(history.data?.filter((x) => x.isSaved).length ?? 0)} />
      </section>
      <section className="grid gap-6 lg:grid-cols-[0.9fr_1.1fr]">
        <div className="overflow-hidden rounded-md border border-slate-200 bg-white dark:border-slate-800 dark:bg-slate-900">
          {latestPhotoUrl && (
            <div className="grid h-80 place-items-center bg-slate-100 dark:bg-slate-950">
              <img src={latestPhotoUrl} alt="Latest uploaded style reference" className="h-full w-full object-contain" />
            </div>
          )}
          <div className="p-5">
          <h2 className="text-xl font-semibold">Create a complete look</h2>
          <p className="mt-2 text-sm leading-6 text-slate-600 dark:text-slate-300">Upload a photo, let AI build your color and style profile, then generate three shoppable outfits within budget.</p>
          <div className="mt-5 flex flex-wrap gap-3">
            <Link className="button" to="/upload"><Camera size={16} /> Upload Photo</Link>
            <Link className="button-secondary" to="/generator"><Sparkles size={16} /> Generate Outfit</Link>
          </div>
          </div>
        </div>
        {latest ? <OutfitCard outfit={latest} /> : <div className="rounded-md border border-dashed border-slate-300 p-8 text-center text-slate-500 dark:border-slate-700">Your newest outfit will appear here.</div>}
      </section>
    </div>
  );
}

function Metric({ icon: Icon, label, value }: { icon: typeof Camera; label: string; value: string }) {
  return (
    <div className="rounded-md border border-slate-200 bg-white p-4 dark:border-slate-800 dark:bg-slate-900">
      <Icon className="text-teal-700 dark:text-teal-300" size={20} />
      <p className="mt-3 text-sm text-slate-500">{label}</p>
      <p className="text-2xl font-semibold">{value}</p>
    </div>
  );
}
