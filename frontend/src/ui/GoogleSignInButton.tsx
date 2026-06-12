import { useMutation } from '@tanstack/react-query';
import { useCallback, useEffect, useRef } from 'react';
import toast from 'react-hot-toast';
import { useNavigate } from 'react-router-dom';
import { apiClient, getApiErrorMessage } from '../lib/api';
import { persistAuth } from '../lib/auth';
import { isAdminUser } from '../lib/admin';

type GoogleCredentialResponse = {
  credential?: string;
};

declare global {
  interface Window {
    google?: {
      accounts: {
        id: {
          initialize: (options: { client_id: string; callback: (response: GoogleCredentialResponse) => void }) => void;
          renderButton: (element: HTMLElement, options: { theme: string; size: string; width: string; text: string }) => void;
        };
      };
    };
  }
}

type Props = {
  text: 'signin_with' | 'signup_with' | 'continue_with';
  redirectTo?: string;
  requireAdmin?: boolean;
};

export function GoogleSignInButton({ text, redirectTo = '/', requireAdmin = false }: Props) {
  const navigate = useNavigate();
  const containerRef = useRef<HTMLDivElement>(null);
  const clientId = import.meta.env.VITE_GOOGLE_CLIENT_ID as string | undefined;
  const { mutate: googleLogin } = useMutation({
    mutationFn: apiClient.googleLogin,
    onSuccess: (data) => {
      persistAuth(data);
      if (requireAdmin && !isAdminUser(data.user)) {
        toast.error('This account is not allowed to access Admin.');
        navigate('/login', { replace: true });
        return;
      }

      navigate(redirectTo, { replace: true });
    },
    onError: (error) => toast.error(getApiErrorMessage(error))
  });
  const handleCredential = useCallback((response: GoogleCredentialResponse) => {
    if (!response.credential) {
      toast.error('Google did not return a login token.');
      return;
    }

    googleLogin({ idToken: response.credential });
  }, [googleLogin]);

  useEffect(() => {
    if (!clientId || !containerRef.current) {
      return;
    }

    const render = () => {
      if (!window.google || !containerRef.current) {
        return;
      }

      window.google.accounts.id.initialize({
        client_id: clientId,
        callback: handleCredential
      });
      window.google.accounts.id.renderButton(containerRef.current, {
        theme: 'outline',
        size: 'large',
        width: '320',
        text
      });
    };

    if (window.google) {
      render();
      return;
    }

    const script = document.createElement('script');
    script.src = 'https://accounts.google.com/gsi/client';
    script.async = true;
    script.defer = true;
    script.onload = render;
    document.head.appendChild(script);
  }, [clientId, handleCredential, text]);

  if (!clientId) {
    return (
      <button type="button" className="button-secondary w-full justify-center" onClick={() => toast.error('Add VITE_GOOGLE_CLIENT_ID in the frontend and GoogleAuth:ClientId in the API.')}>
        Continue with Google
      </button>
    );
  }

  return <div className="flex justify-center" ref={containerRef} />;
}
