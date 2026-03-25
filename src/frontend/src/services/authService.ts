import { apiClient } from '../api/client';
import { userService } from './userService';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  user: any; 
}

const AUTH_URL = import.meta.env.VITE_AUTH_API_URL || 'http://localhost:5002/api';
const USER_URL = import.meta.env.VITE_USER_API_URL || 'http://localhost:5000/api';

export const authService = {
  login: async (credentials: LoginRequest): Promise<AuthResponse> => {
    const response = await apiClient.post(`${AUTH_URL}/auth/login`, credentials);
    const token = response.data.Token || response.data.token;

    localStorage.setItem('jwt_token', token);

    const user = await userService.getProfile();

    return { token, user };
  },
  
  register: async (userData: any): Promise<void> => {
    await apiClient.post(`${USER_URL}/users/register`, userData);
  }
};