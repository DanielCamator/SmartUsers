import { useAuthStore } from '../store/useAuthStore';
import { UserCircle, Mail, BadgeCheck, Loader2 } from 'lucide-react';

export default function DashboardPage() {
  const { user } = useAuthStore();

  if (!user) {
    return (
      <div className="flex justify-center items-center h-64">
        <Loader2 className="w-10 h-10 animate-spin text-blue-600" />
      </div>
    );
  }

  return (
    <div className="max-w-4xl mx-auto py-6">
      <div className="bg-white rounded-3xl shadow-sm border p-8">
        <div className="flex items-start gap-6">
          <div className="bg-blue-100 p-4 rounded-full text-blue-600 flex-shrink-0">
            <UserCircle className="w-16 h-16" />
          </div>
          <div className="flex-1 min-w-0">
            <div className="flex items-center gap-3 mb-2">
              <h1 className="text-3xl font-bold text-gray-900 truncate">
                {user.firstName || 'Usuario'} {user.lastName || ''}
              </h1>
              <BadgeCheck className="w-6 h-6 text-green-500 flex-shrink-0" />
            </div>
            <p className="text-gray-500 text-lg flex items-center gap-2 truncate">
              <Mail className="w-5 h-5 flex-shrink-0" />
              {user.email || 'Sin correo registrado'}
            </p>
          </div>
        </div>

        <div className="mt-10 grid grid-cols-1 md:grid-cols-2 gap-6">
          <div className="p-6 bg-gray-50 rounded-2xl border">
            <p className="text-sm text-gray-500 mb-1 font-medium">ID de Usuario</p>
            <p className="font-mono text-gray-900 break-all">{user.id || 'N/A'}</p>
          </div>
          <div className="p-6 bg-gray-50 rounded-2xl border">
            <p className="text-sm text-gray-500 mb-1 font-medium">Rol Asignado (ID)</p>
            <p className="font-mono text-gray-900 break-all">{user.roleId || 'No asignado'}</p>
          </div>
        </div>
      </div>
    </div>
  );
}