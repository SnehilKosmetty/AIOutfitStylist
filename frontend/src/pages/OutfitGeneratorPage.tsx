import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Sparkles } from 'lucide-react';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import toast from 'react-hot-toast';
import { z } from 'zod';
import { apiClient, getApiErrorMessage } from '../lib/api';
import { currentUser } from '../lib/auth';
import { budgetOptions, styleOptions } from '../lib/profileOptions';
import type { Occasion, Outfit } from '../types';
import { OutfitCard } from '../ui/OutfitCard';

const schema = z.object({
  occasion: z.coerce.number(),
  budget: z.coerce.number().min(25),
  weather: z.string().min(1),
  stylePreference: z.string().min(1),
  shoppingDepartment: z.enum(['mens', 'womens', 'unisex'])
});
type FormValues = z.infer<typeof schema>;

const occasions: { label: string; value: Occasion }[] = [
  { label: 'Interview', value: 1 }, { label: 'Office', value: 2 }, { label: 'Casual', value: 3 }, { label: 'Party', value: 4 },
  { label: 'Wedding', value: 5 }, { label: 'Date Night', value: 6 }, { label: 'Travel', value: 7 }, { label: 'Gym', value: 8 }
];

const departmentOptions = [
  { label: "Men's", value: 'mens' },
  { label: "Women's", value: 'womens' },
  { label: 'Unisex', value: 'unisex' }
];

const weatherOptions = [
  'Hot',
  'Warm',
  'Mild',
  'Cool',
  'Cold',
  'Rainy',
  'Snowy',
  'Windy',
  'Humid'
];

export function OutfitGeneratorPage() {
  const [outfits, setOutfits] = useState<Outfit[]>([]);
  const latestAnalysisId = localStorage.getItem('aios_latest_analysis');
  const [useUploadedPhoto, setUseUploadedPhoto] = useState(Boolean(latestAnalysisId));
  const queryClient = useQueryClient();
  const user = currentUser();
  const form = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      occasion: 3,
      budget: user?.budgetMax ?? 100,
      weather: 'Mild',
      stylePreference: user?.preferredStyle ?? 'Modern casual',
      shoppingDepartment: user?.gender === 1 ? 'womens' : user?.gender === 2 ? 'mens' : 'unisex'
    }
  });
  const generate = useMutation({
    mutationFn: (values: FormValues) => apiClient.generateOutfits({
      ...values,
      photoAnalysisId: useUploadedPhoto ? latestAnalysisId : null
    }),
    onSuccess: (data) => { setOutfits(data); queryClient.invalidateQueries({ queryKey: ['outfits'] }); toast.success('Outfits generated'); },
    onError: (error) => toast.error(getApiErrorMessage(error))
  });
  const save = useMutation({
    mutationFn: apiClient.saveOutfit,
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['outfits'] }); toast.success('Outfit saved'); },
    onError: () => toast.error('Save failed')
  });

  return (
    <div className="space-y-6 pb-20 lg:pb-0">
      <form className="rounded-md border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900" onSubmit={form.handleSubmit((values) => generate.mutate(values))}>
        <h2 className="text-xl font-semibold">Outfit Generator</h2>
        <div className="mt-5 grid gap-3 md:grid-cols-5">
          <select className="input" {...form.register('occasion')}>
            {occasions.map((item) => <option key={item.value} value={item.value}>{item.label}</option>)}
          </select>
          <select className="input" {...form.register('shoppingDepartment')}>
            {departmentOptions.map((item) => <option key={item.value} value={item.value}>{item.label}</option>)}
          </select>
          <select className="input" {...form.register('budget')}>
            {budgetOptions.map((budget) => <option key={budget} value={budget}>${budget}</option>)}
          </select>
          <select className="input" {...form.register('weather')}>
            {weatherOptions.map((weather) => <option key={weather} value={weather}>{weather}</option>)}
          </select>
          <select className="input" {...form.register('stylePreference')}>
            {styleOptions.map((style) => <option key={style} value={style}>{style}</option>)}
          </select>
        </div>
        <div className="mt-4 flex flex-wrap items-center gap-3">
          <label className={`flex items-center gap-2 rounded-md border px-3 py-2 text-sm ${latestAnalysisId ? 'border-slate-300 dark:border-slate-700' : 'border-slate-200 text-slate-400 dark:border-slate-800'}`}>
            <input
              type="checkbox"
              checked={useUploadedPhoto}
              disabled={!latestAnalysisId}
              onChange={(event) => setUseUploadedPhoto(event.target.checked)}
            />
            Use uploaded photo
          </label>
          <button className="button" disabled={generate.isPending}><Sparkles size={16} /> {generate.isPending ? 'Generating...' : 'Generate 3 Outfits'}</button>
        </div>
      </form>
      <section className="grid gap-5 lg:grid-cols-3">
        {outfits.map((outfit) => <OutfitCard key={outfit.id} outfit={outfit} onSave={(id) => save.mutate(id)} />)}
      </section>
    </div>
  );
}
