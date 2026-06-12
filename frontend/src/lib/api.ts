import axios from 'axios';
import type { AdminDashboard, AuthResponse, OtpResponse, Outfit, PhotoAnalysis, PhotoUploadResponse, UserProfile } from '../types';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? 'https://localhost:7132'
});

export const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? 'https://localhost:7132';

export function toAbsoluteUrl(url?: string) {
  if (!url) return '';
  if (url.startsWith('http')) return url;
  return `${apiBaseUrl}${url}`;
}

export function getApiErrorMessage(error: unknown) {
  if (axios.isAxiosError(error)) {
    if (error.response?.status === 401) {
      return 'Your session expired. Please sign in again.';
    }

    const data = error.response?.data as { message?: string; detail?: string; title?: string } | undefined;
    return data?.message ?? data?.detail ?? data?.title ?? error.message;
  }

  return 'Something went wrong.';
}

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('aios_token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

export const apiClient = {
  async sendRegistrationOtp(payload: unknown) {
    const { data } = await api.post<OtpResponse>('/api/auth/send-registration-otp', payload);
    return data;
  },
  async register(payload: unknown) {
    const { data } = await api.post<AuthResponse>('/api/auth/register', payload);
    return data;
  },
  async login(payload: unknown) {
    const { data } = await api.post<AuthResponse>('/api/auth/login', payload);
    return data;
  },
  async googleLogin(payload: unknown) {
    const { data } = await api.post<AuthResponse>('/api/auth/google', payload);
    return data;
  },
  async profile() {
    const { data } = await api.get<UserProfile>('/api/auth/profile');
    return data;
  },
  async updateProfile(payload: unknown) {
    const { data } = await api.put<UserProfile>('/api/auth/profile', payload);
    return data;
  },
  async uploadPhoto(file: File) {
    const formData = new FormData();
    formData.append('image', file);
    const { data } = await api.post<PhotoUploadResponse>('/api/photos/upload', formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });
    return data;
  },
  async analyzePhoto(photoId: string) {
    const { data } = await api.post<PhotoAnalysis>('/api/analysis/analyze-photo', { photoId });
    return data;
  },
  async generateOutfits(payload: unknown) {
    const { data } = await api.post<Outfit[]>('/api/outfits/generate', payload);
    return data;
  },
  async history() {
    const { data } = await api.get<Outfit[]>('/api/outfits/history');
    return data;
  },
  async saveOutfit(outfitId: string) {
    const { data } = await api.post<Outfit>('/api/outfits/save', { outfitId });
    return data;
  },
  async deleteOutfit(id: string) {
    await api.delete(`/api/outfits/${id}`);
  },
  async adminDashboard() {
    const { data } = await api.get<AdminDashboard>('/api/admin/dashboard');
    return data;
  }
};
