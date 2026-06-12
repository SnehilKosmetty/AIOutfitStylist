import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { apiClient } from '../lib/api';
import { OutfitCard } from '../ui/OutfitCard';

export function SavedOutfitsPage() {
  const queryClient = useQueryClient();
  const history = useQuery({ queryKey: ['outfits'], queryFn: apiClient.history });
  const remove = useMutation({
    mutationFn: apiClient.deleteOutfit,
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['outfits'] }); toast.success('Outfit deleted'); },
    onError: () => toast.error('Delete failed')
  });
  const saved = history.data?.filter((x) => x.isSaved) ?? [];

  return (
    <div className="space-y-5 pb-20 lg:pb-0">
      <h2 className="text-xl font-semibold">Saved Outfits</h2>
      <section className="grid gap-5 lg:grid-cols-3">
        {saved.map((outfit) => <OutfitCard key={outfit.id} outfit={outfit} onDelete={(id) => remove.mutate(id)} />)}
      </section>
      {!history.isLoading && saved.length === 0 && <div className="rounded-md border border-dashed border-slate-300 p-8 text-center text-slate-500 dark:border-slate-700">Saved outfits will appear here.</div>}
    </div>
  );
}
