import { describe, it, expect, beforeEach } from 'vitest';
import { useAuthStore } from './useAuthStore';

describe('useAuthStore', () => {
  beforeEach(() => {
    useAuthStore.getState().logout();
  });

  it('debe inicializar con usuario nulo y no autenticado', () => {
    const state = useAuthStore.getState();
    expect(state.user).toBeNull();
    expect(state.token).toBeNull();
    expect(state.isAuthenticated).toBe(false);
  });

  it('debe actualizar el estado al hacer login', () => {
    const mockUser = {
      id: '1',
      email: 'test@ejemplo.com',
      firstName: 'Test',
      lastName: 'User',
      roleId: '00000000-0000-0000-0000-000000000000'
    };
    const mockToken = 'fake-jwt-token';

    useAuthStore.getState().login(mockUser, mockToken);

    const state = useAuthStore.getState();
    expect(state.user).toEqual(mockUser);
    expect(state.token).toBe(mockToken);
    expect(state.isAuthenticated).toBe(true);
  });
});