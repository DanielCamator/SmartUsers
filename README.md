# SmartUsers

# Solución Prueba Técnica - Senior Full-Stack Engineer (IA)

Este repositorio contiene la resolución completa de la prueba técnica para la vacante de **Senior Full-Stack Engineer (IA)**, integrando una arquitectura de microservicios, desarrollo frontend y agentes de Inteligencia Artificial.

---

## 🚀 Inicio Rápido (Docker)

Para levantar todo el ecosistema (Bases de Datos, Bus de Mensajería, Backend y Frontend), asegúrese de tener Docker instalado y ejecute:

```bash
docker-compose up --build
```

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

Interfaz de usuario en React/TypeScript con gestión de estado y consumo de APIs de microservicios.

### 🔍 4. Análisis y Diagnóstico

Resolución teórica de fallas en producción, análisis de latencia y problemas en agentes de IA.

- [Ver Documento de Diagnóstico](./docs/diagnostico_falla.md)

### 🤖 5. Integración de IA y Agentes

Implementación de agente de IA con pipeline de RAG (Retrieval-Augmented Generation) y evaluación de respuestas.

- [Ver Estrategia de IA y Prompt Engineering](./docs/prompt_engineering.md)

---

## 🛠️ Stack Tecnológico

- **Backend:** .NET 10 (C#), Entity Framework Core, MediatR (CQRS).
- **Frontend:** React + TypeScript + Redux Toolkit.
- **Mensajería:** RabbitMQ.
- **Persistencia:** PostgreSQL, MongoDB, Redis Stack (Vector DB).
- **IA:** Semantic Kernel / Azure OpenAI API.

---

### 👨‍💻 Autor

**Daniel Camacho**
