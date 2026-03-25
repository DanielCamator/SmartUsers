import { apiClient } from '../api/client';

export interface UpdateProfileRequest {
  firstName: string;
  lastName: string;
  phoneNumber: string;
  address: string;
}

const USER_URL = import.meta.env.VITE_USER_API_URL || 'http://localhost:5002/api';

export const userService = {
  getProfile: async () => {
    const response = await apiClient.get(`${USER_URL}/users/me`);
    return response.data;
  },
  
  updateProfile: async (data: UpdateProfileRequest): Promise<void> => {
    await apiClient.put(`${USER_URL}/users/me`, data);
  }
};