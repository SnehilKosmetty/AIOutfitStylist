import { useMutation } from '@tanstack/react-query';
import { useEffect, useRef } from 'react';
import toast from 'react-hot-toast';
import { useNavigate } from 'react-router-dom';
import { apiClient, getApiErrorMessage } from '../lib/api';
import { persistAuth } from '../lib/auth';

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
};

export function GoogleSignInButton({ text }: Props) {
  const navigate = useNavigate();
  const containerRef = useRef<HTMLDivElement>(null);
  const clientId = import.meta.env.VITE_GOOGLE_CLIENT_ID as string | undefined;
  const login = useMutation({
    mutationFn: apiClient.googleLogin,
    onSuccess: (data) => {
      persistAuth(data);
      navigate('/');
    },
    onError: (error) => toast.error(getApiErrorMessage(error))
  });

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
        callback: (response) => {
          if (!response.credential) {
            toast.error('Google did not return a login token.');
            return;
          }

          login.mutate({ idToken: response.credential });
        }
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
  }, [clientId, login, text]);

  if (!clientId) {
    return (
      <button type="button" className="button-secondary w-full justify-center" onClick={() => toast.error('Add VITE_GOOGLE_CLIENT_ID in the frontend and GoogleAuth:ClientId in the API.')}>
        Continue with Google
      </button>
    );
  }

  return <div className="flex justify-center" ref={containerRef} />;
}
