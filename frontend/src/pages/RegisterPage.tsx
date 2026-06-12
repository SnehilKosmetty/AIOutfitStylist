import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation } from '@tanstack/react-query';
import { Eye, EyeOff, MailCheck } from 'lucide-react';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import toast from 'react-hot-toast';
import { Link, useNavigate } from 'react-router-dom';
import { z } from 'zod';
import { apiClient, getApiErrorMessage } from '../lib/api';
import { persistAuth } from '../lib/auth';
import { ageOptions, budgetOptions, clothingSizeOptions, genderOptions, heightOptions, passwordScore, passwordStrengthLabel, styleOptions, weightOptions } from '../lib/profileOptions';
import { GoogleSignInButton } from '../ui/GoogleSignInButton';
import { AuthShell } from './LoginPage';

const schema = z.object({
  firstName: z.string().min(1),
  lastName: z.string().min(1),
  email: z.string().email(),
  password: z.string()
    .min(8)
    .regex(/[A-Z]/)
    .regex(/[a-z]/)
    .regex(/[0-9]/)
    .regex(/[^a-zA-Z0-9]/),
  otpCode: z.string().length(6),
  gender: z.coerce.number().default(0),
  age: z.preprocess(optionalNumber, z.number().optional()),
  height: z.preprocess(optionalNumber, z.number().optional()),
  weight: z.preprocess(optionalNumber, z.number().optional()),
  preferredStyle: z.string().optional(),
  clothingSize: z.string().optional(),
  budgetMin: z.preprocess(optionalNumber, z.number().optional()),
  budgetMax: z.preprocess(optionalNumber, z.number().optional())
});
type FormValues = z.input<typeof schema>;

export function RegisterPage() {
  const navigate = useNavigate();
  const form = useForm<FormValues>({ resolver: zodResolver(schema), defaultValues: { gender: 0 } });
  const [showPassword, setShowPassword] = useState(false);
  const watchedPassword = form.watch('password') ?? '';
  const strength = passwordScore(String(watchedPassword));
  const sendOtp = useMutation({
    mutationFn: apiClient.sendRegistrationOtp,
    onSuccess: () => {
      toast.success('OTP sent. It is valid for 5 minutes.');
    },
    onError: (error) => toast.error(getApiErrorMessage(error))
  });
  const mutation = useMutation({
    mutationFn: apiClient.register,
    onSuccess: (data) => { persistAuth(data); navigate('/'); },
    onError: (error) => toast.error(getApiErrorMessage(error))
  });

  return (
    <AuthShell>
      <form className="space-y-4" onSubmit={form.handleSubmit((values) => mutation.mutate(values))}>
        <h1 className="text-2xl font-semibold">Create your profile</h1>
        <div className="grid gap-3 sm:grid-cols-2">
          <input className="input" placeholder="First name" {...form.register('firstName')} />
          <input className="input" placeholder="Last name" {...form.register('lastName')} />
        </div>
        <div className="flex gap-2">
          <input className="input" placeholder="Email" {...form.register('email')} />
          <button type="button" className="button-secondary whitespace-nowrap" disabled={sendOtp.isPending} onClick={() => sendOtp.mutate({ email: form.getValues('email') })}>
            <MailCheck size={16} /> {sendOtp.isPending ? 'Sending...' : 'Send OTP'}
          </button>
        </div>
        <input className="input" placeholder="6 digit OTP" maxLength={6} {...form.register('otpCode')} />
        <div className="relative">
          <input className="input pr-12" placeholder="Password" type={showPassword ? 'text' : 'password'} {...form.register('password')} />
          <button type="button" className="absolute right-2 top-1/2 -translate-y-1/2 rounded-md p-2 text-slate-500 hover:bg-slate-100 dark:hover:bg-slate-800" onClick={() => setShowPassword((value) => !value)} aria-label={showPassword ? 'Hide password' : 'Show password'}>
            {showPassword ? <EyeOff size={18} /> : <Eye size={18} />}
          </button>
        </div>
        <div className="space-y-1">
          <div className="h-2 overflow-hidden rounded-full bg-slate-200 dark:bg-slate-800">
            <span className="block h-full bg-teal-700 transition-all" style={{ width: `${strength * 20}%` }} />
          </div>
          <p className="text-xs text-slate-500">Password strength: {passwordStrengthLabel(strength)}</p>
        </div>
        <div className="grid gap-3 sm:grid-cols-2">
          <select className="input" {...form.register('gender')}>
            {genderOptions.map((option) => <option key={option.value} value={option.value}>{option.label}</option>)}
          </select>
          <select className="input" {...form.register('age')}>
            <option value="">Age</option>
            {ageOptions.map((age) => <option key={age} value={age}>{age}</option>)}
          </select>
          <select className="input" {...form.register('height')}>
            <option value="">Height</option>
            {heightOptions.map((height) => <option key={height} value={height}>{height.toFixed(1)}</option>)}
          </select>
          <select className="input" {...form.register('weight')}>
            <option value="">Weight</option>
            {weightOptions.map((weight) => <option key={weight} value={weight}>{weight} lb</option>)}
          </select>
          <select className="input" {...form.register('preferredStyle')}>
            <option value="">Preferred style</option>
            {styleOptions.map((style) => <option key={style} value={style}>{style}</option>)}
          </select>
          <select className="input" {...form.register('clothingSize')}>
            <option value="">Clothing size</option>
            {clothingSizeOptions.map((size) => <option key={size} value={size}>{size}</option>)}
          </select>
          <select className="input" {...form.register('budgetMin')}>
            <option value="">Budget min</option>
            {budgetOptions.map((budget) => <option key={budget} value={budget}>${budget}</option>)}
          </select>
          <select className="input" {...form.register('budgetMax')}>
            <option value="">Budget max</option>
            {budgetOptions.map((budget) => <option key={budget} value={budget}>${budget}</option>)}
          </select>
        </div>
        <button className="button w-full" disabled={mutation.isPending}>{mutation.isPending ? 'Creating...' : 'Create account'}</button>
        <div className="flex items-center gap-3 text-xs uppercase tracking-wider text-slate-400"><span className="h-px flex-1 bg-slate-200 dark:bg-slate-800" /> Or <span className="h-px flex-1 bg-slate-200 dark:bg-slate-800" /></div>
        <GoogleSignInButton text="signup_with" />
        <p className="text-sm text-slate-500">Already have an account? <Link className="font-semibold text-teal-700" to="/login">Sign in</Link></p>
      </form>
    </AuthShell>
  );
}

function optionalNumber(value: unknown) {
  return value === '' || value === undefined || Number.isNaN(value) ? undefined : Number(value);
}
