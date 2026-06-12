import { Navigate, createBrowserRouter, useLocation } from 'react-router-dom';
import type React from 'react';
import { AppLayout } from './ui/AppLayout';
import { AdminLayout } from './ui/AdminLayout';
import { currentUser, isAuthenticated } from './lib/auth';
import { isAdminUser } from './lib/admin';
import { DashboardPage } from './pages/DashboardPage';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { UploadPhotoPage } from './pages/UploadPhotoPage';
import { OutfitGeneratorPage } from './pages/OutfitGeneratorPage';
import { SavedOutfitsPage } from './pages/SavedOutfitsPage';
import { ProfilePage } from './pages/ProfilePage';
import { AdminDashboardPage } from './pages/AdminDashboardPage';

function PrivateRoute({ children }: { children: React.ReactNode }) {
  const location = useLocation();
  return isAuthenticated() ? children : <Navigate to="/login" replace state={{ from: location.pathname }} />;
}

function AdminPrivateRoute({ children }: { children: React.ReactNode }) {
  const location = useLocation();
  if (!isAuthenticated()) {
    return <Navigate to="/admin/login" replace state={{ from: location.pathname }} />;
  }

  return isAdminUser(currentUser()) ? children : <Navigate to="/" replace />;
}

export const router = createBrowserRouter([
  { path: '/login', element: <LoginPage /> },
  { path: '/admin/login', element: <LoginPage mode="admin" /> },
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
      <AdminPrivateRoute>
        <AdminLayout />
      </AdminPrivateRoute>
    ),
    children: [
      { index: true, element: <AdminDashboardPage /> },
      { path: 'dashboard', element: <AdminDashboardPage /> }
    ]
  }
]);
