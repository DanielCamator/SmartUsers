# SmartUsers

# Solución Prueba Técnica - Senior Full-Stack Engineer (IA)

Este repositorio contiene la resolución completa de la prueba técnica para la vacante de **Senior Full-Stack Engineer (IA)**, integrando una arquitectura de microservicios, desarrollo frontend y agentes de Inteligencia Artificial.

---

## 🚀 Inicio Rápido (Docker)

### 🛠️ Configuración de Variables de Entorno

Antes de levantar el proyecto, es necesario configurar las llaves de API para el Agente de IA:

1. Localice el archivo `.env.example` en la raíz.
2. Cree una copia y renómbrela a `.env`.
3. Edite el archivo `.env` e ingrese sus credenciales de **Groq**, **OpenAI** y **Pinecone**.

El proyecto está 100% dockerizado. Para levantar todo el ecosistema (Bases de Datos, Bus de Mensajería, Backend y Frontend), asegúrese de tener Docker instalado y ejecute en la raíz del proyecto:

docker-compose up -d --build

Una vez que los contenedores estén en ejecución, podrá acceder a:

- **UI Frontend:** http://localhost:3000
- **Auth API:** Puerto 5002
- **User API:** Puerto 5000
- **Audit API:** Puerto 5001
- **AI API:** Puerto 5003

---

## 📂 Contenido de la Entrega

La solución se divide en 5 ejercicios clave según el requerimiento:

### 🏛️ 1. Arquitectura y Diseño

Diseño detallado de microservicios, estrategia multi-DB y patrones aplicados (DDD/Clean Architecture).

- [Ver Documentación de Arquitectura](./docs/arquitectura.md)

### ⚙️ 2. Microservicios (Backend)

Implementación de una ecosistema de servicios independientes construidos sobre **.NET 10**, diseñados bajo principios de alta disponibilidad y bajo acoplamiento.

#### 🛠️ Guía de Ejecución del Sistema

El proyecto utiliza **Docker Compose** para orquestar tanto los microservicios como la infraestructura necesaria. Siga estos pasos para iniciar el sistema:

1.  **Prerrequisitos:** Asegúrese de tener instalado **Docker Desktop** y que el motor de Docker esté en ejecución.
2.  **Despliegue de Infraestructura y Servicios:** Desde la raíz del proyecto, ejecute:
    ```bash
    docker-compose up -d --build
    ```
3.  **Verificación de Salud:** Una vez finalizado el proceso, los servicios estarán disponibles en:
    - **Gateway/Proxy:** El frontend redirige las peticiones a los puertos 5000-5003 según el servicio.
    - **Persistence:** PostgreSQL (Relacional), MongoDB (NoSQL) y RabbitMQ (Bus) estarán operativos internamente.

#### 🏛️ Arquitectura: DDD y Clean Architecture

La lógica de negocio se ha organizado siguiendo el patrón de **Clean Architecture** (Arquitectura Cebolla), asegurando que el núcleo del sistema sea independiente de frameworks, bases de datos o agentes externos:

- **Capa de Dominio (Domain):** Contiene las entidades de negocio, interfaces base, Value Objects y excepciones core. Es la capa de mayor nivel y no posee dependencias externas.
- **Capa de Aplicación (Application):** Implementa el patrón **CQRS** mediante la librería **MediatR**. Aquí se definen los _Commands_ (escritura) y _Queries_ (lectura), permitiendo una escalabilidad independiente y un código más mantenible.
- **Capa de Infraestructura (Infrastructure):** Contiene las implementaciones técnicas. Incluye el acceso a datos mediante **Entity Framework Core**, la integración con **RabbitMQ (MassTransit)** para eventos asíncronos y los clientes de **Pinecone** y **OpenAI**.
- **Capa de Presentación (Web API):** Controladores delgados encargados de la validación de entrada, manejo de CORS, Rate Limiting y la exposición de endpoints REST.

#### 📡 Patrones y Comunicación

- **Event-Driven Architecture:** Se utiliza un bus de mensajes con **RabbitMQ** para la creación de logs de auditoría. Cuando un usuario es creado o modificado, se publica un evento que el `AuditService` consume de forma asíncrona, evitando latencia en la operación principal.
- **Persistencia Políglota:** - **PostgreSQL:** Utilizado para datos transaccionales y consistencia fuerte (Usuarios y Autenticación).
  - **MongoDB:** Utilizado para el almacenamiento de auditorías, aprovechando su flexibilidad para esquemas de logs variables.
  - **Redis Stack:** Utilizado para la gestión de estados y almacenamiento vectorial temporal.

### 💻 3. Desarrollo Front-end (React)

Interfaz de usuario moderna y responsiva construida con **React 18** y **Vite**, enfocada en la velocidad de carga y una experiencia de usuario fluida.

#### 🛠️ Guía de Ejecución

Existen dos formas de ejecutar el frontend, dependiendo de la necesidad:

**A. Vía Docker (Recomendado para Evaluación)**
Si ya ejecutó el `docker-compose` de la raíz, el frontend ya está disponible en:

- **URL:** `http://localhost:3000`
- **Nota:** Esta versión corre sobre un servidor optimizado y ya está vinculada a la red interna de microservicios.

**B. Modo Desarrollo (Local)**
Si desea realizar cambios en el código y ver los resultados en tiempo real (HMR), siga estos pasos:

1. Entre a la carpeta: `cd src/frontend`
2. Instale dependencias: `npm install`
3. Inicie el servidor de desarrollo: `npm run dev`
4. Acceda a: `http://localhost:5173` (o el puerto indicado por Vite).

#### 🧪 Testing y Calidad

Para asegurar la estabilidad de la UI, se implementaron pruebas unitarias y de integración:

- **Ejecutar Tests:** `npm run test`
- **Herramientas:** Vitest + React Testing Library. Se validaron los flujos de autenticación, el estado global de Zustand y el renderizado del Chatbot Widget.

#### 🎨 Decisiones Técnicas

- **Gestión de Estado:** Uso de **Zustand** para un manejo de estado ligero, evitando el boilerplate innecesario de Redux.
- **Estilos:** **Tailwind CSS** para un diseño consistente y rápido de iterar.
- **Iconografía:** **Lucide React** para una interfaz limpia y profesional.
- **Comunicación:** Integración asíncrona con el Agente de IA mediante `fetch` con manejo estricto de estados de carga (`loading`) y errores de CORS.

### 🔍 4. Análisis y Diagnóstico

Resolución teórica de fallas en producción, análisis de latencia y problemas en agentes de IA.

- [Ver Documento de Diagnóstico](./docs/diagnostico_falla.md)

### 🤖 5. Integración de IA y Agentes

Implementación de agente de IA con pipeline de RAG (Retrieval-Augmented Generation) y evaluación de respuestas.

- [Ver Estrategia de IA y Prompt Engineering](./docs/prompt_engineering.md)

---

## 🛠️ Stack Tecnológico

- **Backend:** .NET 10 (C#), Entity Framework Core, MediatR (CQRS), MassTransit.
- **Frontend:** React 18, TypeScript, Vite, Zustand, Tailwind CSS.
- **Mensajería:** RabbitMQ.
- **Persistencia:** PostgreSQL, MongoDB, Redis Stack (Vector DB).
- **IA:** Semantic Kernel, OpenAI API, Pinecone, Groq.

---

### 👨‍💻 Autor

**Daniel Camacho** - Senior Software Engineer | Backend, Cloud & AI

- 🌐 **Portafolio Web:** [daniel-camacho.net](https://daniel-camacho.net/)
- 💼 **LinkedIn:** [linkedin.com/in/daniel-ct](https://linkedin.com/in/daniel-ct)
