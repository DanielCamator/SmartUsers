import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { toast } from 'sonner';
import { Loader2, Edit2, X } from 'lucide-react';
import { useAuthStore } from '../store/useAuthStore';
import { userService } from '../services/userService';

const profileSchema = z.object({
  firstName: z.string().min(2, 'Requerido'),
  lastName: z.string().min(2, 'Requerido'),
  phoneNumber: z.string().min(10, 'Mínimo 10 dígitos'),
  address: z.string().min(5, 'Requerido'),
});

type ProfileFormValues = z.infer<typeof profileSchema>;

export default function ProfilePage() {
  const { user, login, token } = useAuthStore();
  const [isEditing, setIsEditing] = useState(false);

  const { register, handleSubmit, formState: { errors, isSubmitting }, reset } = useForm<ProfileFormValues>({
    resolver: zodResolver(profileSchema),
    values: {
      firstName: user?.firstName || '',
      lastName: user?.lastName || '',
      phoneNumber: user?.phoneNumber || '',
      address: user?.address || '',
    }
  });

  const onSubmit = async (data: ProfileFormValues) => {
    if (!user || !token) return;
    try {
      await userService.updateProfile(data);
      
      const updatedUser = { ...user, ...data };
      login(updatedUser, token); 
      
      toast.success('Perfil actualizado correctamente');
      setIsEditing(false);
    } catch (error: any) {
      toast.error(error.response?.data?.detail || 'Error al actualizar el perfil');
    }
  };

  const handleCancel = () => {
    reset();
    setIsEditing(false);
  };

  const inputStyles = `w-full px-4 py-3 rounded-xl border outline-none transition-all disabled:bg-gray-50 disabled:border-transparent disabled:text-gray-700 disabled:shadow-none`;

  return (
    <div className="max-w-2xl mx-auto py-6">
      <div className="bg-white rounded-3xl shadow-sm border p-8">
        <div className="mb-8 flex justify-between items-start">
          <div>
            <h2 className="text-3xl font-bold text-gray-900">Mi Perfil</h2>
            <p className="text-gray-500 mt-2">Información personal y de contacto</p>
          </div>
          
          {!isEditing && (
            <button
              type="button"
              onClick={() => setIsEditing(true)}
              className="flex items-center gap-2 px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors font-medium text-sm"
            >
              <Edit2 className="w-4 h-4" />
              Editar
            </button>
          )}
        </div>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label className="block text-sm font-medium text-gray-500 mb-2">Nombre</label>
              <input
                {...register('firstName')}
                disabled={!isEditing}
                className={`${inputStyles} ${errors.firstName && isEditing ? 'border-red-500' : 'border-gray-200 focus:ring-2 focus:ring-blue-500'}`}
              />
              {errors.firstName && isEditing && <p className="text-red-500 text-xs mt-1">{errors.firstName.message}</p>}
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-500 mb-2">Apellido</label>
              <input
                {...register('lastName')}
                disabled={!isEditing}
                className={`${inputStyles} ${errors.lastName && isEditing ? 'border-red-500' : 'border-gray-200 focus:ring-2 focus:ring-blue-500'}`}
              />
              {errors.lastName && isEditing && <p className="text-red-500 text-xs mt-1">{errors.lastName.message}</p>}
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-500 mb-2">Teléfono</label>
            <input
              {...register('phoneNumber')}
              disabled={!isEditing}
              className={`${inputStyles} ${errors.phoneNumber && isEditing ? 'border-red-500' : 'border-gray-200 focus:ring-2 focus:ring-blue-500'}`}
            />
            {errors.phoneNumber && isEditing && <p className="text-red-500 text-xs mt-1">{errors.phoneNumber.message}</p>}
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-500 mb-2">Dirección</label>
            <input
              {...register('address')}
              disabled={!isEditing}
              className={`${inputStyles} ${errors.address && isEditing ? 'border-red-500' : 'border-gray-200 focus:ring-2 focus:ring-blue-500'}`}
            />
            {errors.address && isEditing && <p className="text-red-500 text-xs mt-1">{errors.address.message}</p>}
          </div>

          {isEditing && (
            <div className="pt-6 flex gap-3 border-t mt-8">
              <button
                type="button"
                onClick={handleCancel}
                disabled={isSubmitting}
                className="flex-1 px-4 py-3 rounded-xl border border-gray-200 text-gray-700 font-bold hover:bg-gray-50 transition-all flex items-center justify-center gap-2"
              >
                <X className="w-5 h-5" />
                Cancelar
              </button>
              <button
                type="submit"
                disabled={isSubmitting}
                className="flex-1 bg-blue-600 text-white py-3 rounded-xl font-bold hover:bg-blue-700 transition-all disabled:opacity-50 flex items-center justify-center gap-2"
              >
                {isSubmitting && <Loader2 className="animate-spin w-5 h-5" />}
                {isSubmitting ? 'Guardando...' : 'Guardar Cambios'}
              </button>
            </div>
          )}
        </form>
      </div>
    </div>
  );
}