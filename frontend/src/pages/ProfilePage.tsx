import { useMutation, useQuery } from '@tanstack/react-query';
import { Save } from 'lucide-react';
import { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import toast from 'react-hot-toast';
import { apiClient } from '../lib/api';
import { ageOptions, budgetOptions, clothingSizeOptions, genderOptions, heightOptions, styleOptions, weightOptions } from '../lib/profileOptions';

type FormValues = {
  firstName: string;
  lastName: string;
  gender: number;
  age?: number;
  height?: number;
  weight?: number;
  clothingSize?: string;
  preferredStyle?: string;
  budgetMin?: number;
  budgetMax?: number;
};

export function ProfilePage() {
  const profile = useQuery({ queryKey: ['profile'], queryFn: apiClient.profile });
  const form = useForm<FormValues>();
  useEffect(() => { if (profile.data) form.reset(profile.data); }, [profile.data, form]);
  const save = useMutation({
    mutationFn: apiClient.updateProfile,
    onSuccess: () => toast.success('Profile updated'),
    onError: () => toast.error('Update failed')
  });

  return (
    <form className="max-w-3xl rounded-md border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900" onSubmit={form.handleSubmit((values) => save.mutate(values))}>
      <h2 className="text-xl font-semibold">User Profile</h2>
      <div className="mt-5 grid gap-3 sm:grid-cols-2">
        <input className="input" placeholder="First name" {...form.register('firstName')} />
        <input className="input" placeholder="Last name" {...form.register('lastName')} />
        <select className="input" {...form.register('gender', { valueAsNumber: true })}>
          {genderOptions.map((option) => <option key={option.value} value={option.value}>{option.label}</option>)}
        </select>
        <select className="input" {...form.register('age', { setValueAs: optionalNumber })}>
          <option value="">Age</option>
          {ageOptions.map((age) => <option key={age} value={age}>{age}</option>)}
        </select>
        <select className="input" {...form.register('height', { setValueAs: optionalNumber })}>
          <option value="">Height</option>
          {heightOptions.map((height) => <option key={height} value={height}>{height.toFixed(1)}</option>)}
        </select>
        <select className="input" {...form.register('weight', { setValueAs: optionalNumber })}>
          <option value="">Weight</option>
          {weightOptions.map((weight) => <option key={weight} value={weight}>{weight} lb</option>)}
        </select>
        <select className="input" {...form.register('clothingSize')}>
          <option value="">Clothing size</option>
          {clothingSizeOptions.map((size) => <option key={size} value={size}>{size}</option>)}
        </select>
        <select className="input" {...form.register('preferredStyle')}>
          <option value="">Preferred style</option>
          {styleOptions.map((style) => <option key={style} value={style}>{style}</option>)}
        </select>
        <select className="input" {...form.register('budgetMin', { setValueAs: optionalNumber })}>
          <option value="">Budget min</option>
          {budgetOptions.map((budget) => <option key={budget} value={budget}>${budget}</option>)}
        </select>
        <select className="input" {...form.register('budgetMax', { setValueAs: optionalNumber })}>
          <option value="">Budget max</option>
          {budgetOptions.map((budget) => <option key={budget} value={budget}>${budget}</option>)}
        </select>
      </div>
      <button className="button mt-4" disabled={save.isPending}><Save size={16} /> {save.isPending ? 'Saving...' : 'Save Profile'}</button>
    </form>
  );
}

function optionalNumber(value: string) {
  return value === '' ? undefined : Number(value);
}
