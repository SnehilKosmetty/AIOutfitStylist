import { Navigate, createBrowserRouter } from 'react-router-dom';
import type React from 'react';
import { AppLayout } from './ui/AppLayout';
import { AdminLayout } from './ui/AdminLayout';
import { isAuthenticated } from './lib/auth';
import { DashboardPage } from './pages/DashboardPage';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { UploadPhotoPage } from './pages/UploadPhotoPage';
import { OutfitGeneratorPage } from './pages/OutfitGeneratorPage';
import { SavedOutfitsPage } from './pages/SavedOutfitsPage';
import { ProfilePage } from './pages/ProfilePage';
import { AdminDashboardPage } from './pages/AdminDashboardPage';

function PrivateRoute({ children }: { children: React.ReactNode }) {
  return isAuthenticated() ? children : <Navigate to="/login" replace />;
}

export const router = createBrowserRouter([
  { path: '/login', element: <LoginPage /> },
  { path: '/register', element: <RegisterPage /> },
  {
    path: '/',
    element: (
      <PrivateRoute>
        <AppLayout />
      </PrivateRoute>
    ),
    children: [
      { index: true, element: <DashboardPage /> },
      { path: 'upload', element: <UploadPhotoPage /> },
      { path: 'generator', element: <OutfitGeneratorPage /> },
      { path: 'saved', element: <SavedOutfitsPage /> },
      { path: 'profile', element: <ProfilePage /> }
    ]
  },
  {
    path: '/admin',
    element: (
      <PrivateRoute>
        <AdminLayout />
      </PrivateRoute>
    ),
    children: [
      { index: true, element: <AdminDashboardPage /> },
      { path: 'dashboard', element: <AdminDashboardPage /> }
    ]
  }
]);
