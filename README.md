# SmartUsers

# Solución Prueba Técnica - Senior Full-Stack Engineer (IA)

Este repositorio contiene la resolución completa de la prueba técnica para la vacante de **Senior Full-Stack Engineer (IA)**, integrando una arquitectura de microservicios, desarrollo frontend y agentes de Inteligencia Artificial.

---

## 🚀 Inicio Rápido (Docker)

El proyecto está 100% dockerizado. Para levantar todo el ecosistema (Bases de Datos, Bus de Mensajería, Backend y Frontend), asegúrese de tener Docker instalado y ejecute en la raíz del proyecto:

docker-compose up -d --build

Una vez que los contenedores estén en ejecución, podrá acceder a:

- **UI Frontend:** http://localhost:3000
- **Auth API:** Puerto 5002
- **User API:** Puerto 5000
- **Audit API:** Puerto 5001

---

## 📂 Contenido de la Entrega

La solución se divide en 5 ejercicios clave según el requerimiento:

### 🏛️ 1. Arquitectura y Diseño

Diseño detallado de microservicios, estrategia multi-DB y patrones aplicados (DDD/Clean Architecture).

- [Ver Documentación de Arquitectura](./docs/arquitectura.md)

### ⚙️ 2. Microservicios (Backend)

Implementación de servicios independientes con .NET 10, comunicación síncrona/asíncrona y persistencia distribuida.

- **Servicios:** Auth, User, Role, Audit y AI Agent.

### 💻 3. Desarrollo Front-end

Interfaz de usuario robusta en React/TypeScript.

- Gestión de estado global con **Zustand**.
- Validación de esquemas con **React Hook Form + Zod**.
- Testing de componentes y lógica con **Vitest + React Testing Library**.

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
