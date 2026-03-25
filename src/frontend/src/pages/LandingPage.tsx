import { Link } from 'react-router-dom';
import { Users, ShieldCheck, Zap, ArrowRight } from 'lucide-react';

export default function LandingPage() {
  return (
    <div className="min-h-screen bg-white">
      <nav className="flex items-center justify-between p-6 border-b">
        <div className="flex items-center gap-2">
          <div className="bg-blue-600 p-2 rounded-lg">
            <Users className="text-white w-6 h-6" />
          </div>
          <span className="text-xl font-bold tracking-tight">SmartUsers</span>
        </div>
        <div className="flex items-center gap-6">
          <Link 
            to="/login" 
            className="text-gray-600 font-medium hover:text-gray-900 transition-colors"
          >
            Iniciar Sesión
          </Link>
          <Link 
            to="/register" 
            className="bg-blue-600 text-white px-5 py-2 rounded-full font-medium hover:bg-blue-700 transition-colors shadow-sm"
          >
            Registrarse
          </Link>
        </div>
      </nav>

      <main className="max-w-6xl mx-auto px-6 py-20 text-center">
        <h1 className="text-5xl md:text-7xl font-extrabold text-gray-900 mb-6 tracking-tight">
          Gestiona tus usuarios <br />
          <span className="text-transparent bg-clip-text bg-gradient-to-r from-blue-600 to-indigo-600">
            con inteligencia.
          </span>
        </h1>
        <p className="text-xl text-gray-600 mb-12 max-w-2xl mx-auto">
          Una plataforma robusta, segura y escalable construida con arquitectura de microservicios.
        </p>

        <div className="flex flex-col sm:flex-row items-center justify-center gap-4">
          <Link 
            to="/register" 
            className="flex items-center gap-2 bg-blue-600 text-white px-8 py-4 rounded-full text-lg font-bold hover:bg-blue-700 transition-all shadow-lg hover:shadow-xl hover:-translate-y-0.5"
          >
            Comenzar ahora
            <ArrowRight className="w-5 h-5" />
          </Link>
          <Link 
            to="/login" 
            className="flex items-center gap-2 bg-white text-gray-700 border-2 border-gray-200 px-8 py-4 rounded-full text-lg font-bold hover:border-gray-300 hover:bg-gray-50 transition-all"
          >
            Ingresar a mi cuenta
          </Link>
        </div>

        <div className="grid md:grid-cols-3 gap-8 mt-24">
          <div className="p-8 border rounded-3xl bg-gray-50/50 hover:bg-white hover:shadow-lg transition-all text-left">
            <div className="bg-blue-100 w-16 h-16 rounded-2xl flex items-center justify-center mb-6">
              <ShieldCheck className="w-8 h-8 text-blue-600" />
            </div>
            <h3 className="text-xl font-bold mb-3 text-gray-900">Seguridad Total</h3>
            <p className="text-gray-600 leading-relaxed">
              Autenticación basada en JWT y encriptación de grado bancario para proteger cada dato.
            </p>
          </div>
          
          <div className="p-8 border rounded-3xl bg-gray-50/50 hover:bg-white hover:shadow-lg transition-all text-left">
            <div className="bg-blue-100 w-16 h-16 rounded-2xl flex items-center justify-center mb-6">
              <Zap className="w-8 h-8 text-blue-600" />
            </div>
            <h3 className="text-xl font-bold mb-3 text-gray-900">Alta Velocidad</h3>
            <p className="text-gray-600 leading-relaxed">
              Optimizado para una experiencia de usuario fluida, sin demoras y con state management global.
            </p>
          </div>
          
          <div className="p-8 border rounded-3xl bg-gray-50/50 hover:bg-white hover:shadow-lg transition-all text-left">
            <div className="bg-blue-100 w-16 h-16 rounded-2xl flex items-center justify-center mb-6">
              <Users className="w-8 h-8 text-blue-600" />
            </div>
            <h3 className="text-xl font-bold mb-3 text-gray-900">Auditoría Real</h3>
            <p className="text-gray-600 leading-relaxed">
              Seguimiento detallado asíncrono de cada evento y acción dentro de la plataforma.
            </p>
          </div>
        </div>
      </main>
    </div>
  );
}