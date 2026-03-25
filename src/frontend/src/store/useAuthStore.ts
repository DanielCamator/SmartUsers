import { create } from 'zustand';

interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  roleId: string;
  phoneNumber?: string;
  address?: string;
}

interface AuthState {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  login: (user: User, token: string) => void;
  logout: () => void;
}

const getUserFromStorage = (): User | null => {
  try {
    const item = localStorage.getItem('user');
    if (!item || item === 'undefined') return null;
    return JSON.parse(item);
  } catch {
    return null;
  }
};

export const useAuthStore = create<AuthState>((set) => ({
  user: getUserFromStorage(),
  token: localStorage.getItem('jwt_token') !== 'undefined' ? localStorage.getItem('jwt_token') : null,
  isAuthenticated: !!localStorage.getItem('jwt_token') && localStorage.getItem('jwt_token') !== 'undefined',
  
  login: (user, token) => {
    localStorage.setItem('jwt_token', token);
    localStorage.setItem('user', JSON.stringify(user));
    set({ user, token, isAuthenticated: true });
  },
  
  logout: () => {
    localStorage.removeItem('jwt_token');
    localStorage.removeItem('user');
    set({ user: null, token: null, isAuthenticated: false });
  },
}));