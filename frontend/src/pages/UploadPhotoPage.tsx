import { useMutation } from '@tanstack/react-query';
import { Camera, Sparkles, Upload } from 'lucide-react';
import { useState } from 'react';
import toast from 'react-hot-toast';
import { apiClient, getApiErrorMessage, toAbsoluteUrl } from '../lib/api';
import type { PhotoAnalysis, PhotoUploadResponse } from '../types';

export function UploadPhotoPage() {
  const [photo, setPhoto] = useState<PhotoUploadResponse | null>(null);
  const [analysis, setAnalysis] = useState<PhotoAnalysis | null>(null);
  const upload = useMutation({
    mutationFn: apiClient.uploadPhoto,
    onSuccess: (data) => {
      setPhoto(data);
      localStorage.setItem('aios_latest_photo_url', toAbsoluteUrl(data.blobUrl));
      toast.success('Photo uploaded');
    },
    onError: (error) => toast.error(getApiErrorMessage(error))
  });
  const analyze = useMutation({
    mutationFn: apiClient.analyzePhoto,
    onSuccess: (data) => { setAnalysis(data); localStorage.setItem('aios_latest_analysis', data.id); toast.success('Analysis complete'); },
    onError: (error) => toast.error(getApiErrorMessage(error))
  });

  return (
    <div className="grid gap-6 pb-20 lg:grid-cols-[0.9fr_1.1fr] lg:pb-0">
      <section className="rounded-md border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900">
        <h2 className="text-xl font-semibold">Upload Photo</h2>
        <label className="mt-5 grid cursor-pointer place-items-center rounded-md border border-dashed border-slate-300 p-10 text-center dark:border-slate-700">
          <Upload className="mb-3 text-teal-700" />
          <span className="text-sm text-slate-500">JPG, PNG, or WEBP up to 10MB</span>
          <input className="sr-only" type="file" accept="image/jpeg,image/png,image/webp" onChange={(event) => {
            const file = event.target.files?.[0];
            if (file) upload.mutate(file);
          }} />
        </label>
        {photo && (
          <div className="mt-4 grid h-96 place-items-center overflow-hidden rounded-md border border-slate-200 bg-slate-100 dark:border-slate-800 dark:bg-slate-950">
            <img src={toAbsoluteUrl(photo.blobUrl)} alt="Uploaded outfit reference" className="h-full w-full object-contain" />
          </div>
        )}
        {photo && <button className="button mt-4" onClick={() => analyze.mutate(photo.photoId)} disabled={analyze.isPending}><Sparkles size={16} /> {analyze.isPending ? 'Analyzing...' : 'Analyze Photo'}</button>}
      </section>
      <section className="rounded-md border border-slate-200 bg-white p-5 dark:border-slate-800 dark:bg-slate-900">
        <h2 className="text-xl font-semibold">AI Analysis</h2>
        {!analysis && <div className="mt-6 grid place-items-center rounded-md bg-slate-50 p-12 text-slate-500 dark:bg-slate-950"><Camera /></div>}
        {analysis && (
          <div className="mt-5 space-y-4">
            <div className="grid gap-3 sm:grid-cols-3">
              <Info label="Body Type" value={analysis.bodyType} />
              <Info label="Skin Tone" value={analysis.skinTone} />
              <Info label="Style" value={analysis.style} />
            </div>
            <ChipList title="Recommended Colors" items={analysis.recommendedColors} />
            <ChipList title="Recommended Styles" items={analysis.recommendations} />
          </div>
        )}
      </section>
    </div>
  );
}

function Info({ label, value }: { label: string; value: string }) {
  return <div className="rounded-md bg-slate-50 p-3 dark:bg-slate-950"><p className="text-xs text-slate-500">{label}</p><p className="font-semibold">{value}</p></div>;
}

function ChipList({ title, items }: { title: string; items: string[] }) {
  return <div><h3 className="mb-2 font-semibold">{title}</h3><div className="flex flex-wrap gap-2">{items.map((item) => <span key={item} className="rounded-md bg-teal-700/10 px-3 py-1 text-sm text-teal-800 dark:text-teal-200">{item}</span>)}</div></div>;
}
