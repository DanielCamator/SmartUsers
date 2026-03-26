# Estrategia de IA y Prompt Engineering

Este documento detalla la implementación del Agente de IA para el sistema **SmartUsers**, utilizando el patrón RAG (Retrieval-Augmented Generation) para garantizar respuestas precisas, contextuales y libres de alucinaciones.

## 1. Arquitectura del Agente (RAG)

El agente está construido sobre **.NET 10** utilizando **Semantic Kernel**. Se utiliza un pipeline de Retrieval-Augmented Generation (RAG) estructurado bajo los siguientes componentes:

- **Modelo de Orquestación:** Semantic Kernel (.NET 10).
- **LLM (Razonamiento):** Llama-3.1-8b-instant (vía Groq) para optimizar latencia.
- **Vector Database:** Pinecone (Indexación semántica de manuales y políticas).
- **Embeddings:** text-embedding-3-small (OpenAI).

El proceso de ingesta segmenta las reglas de negocio de SmartUsers (roles, políticas de contraseñas, auditorías) y las almacena como vectores. En tiempo de ejecución, la consulta del usuario se vectoriza para recuperar el top 3 de fragmentos más relevantes, los cuales se inyectan en el prompt.

## 2. Estructura del System Prompt

Se implementó un System Message diseñado bajo el principio de delimitación de contexto. Se utilizan etiquetas XML para ayudar al modelo a distinguir entre las instrucciones de comportamiento y los datos recuperados de la base de vectores.

**Componentes del Prompt:**

1. **Definición de Rol:** Definido explícitamente como "asistente técnico avanzado del sistema SmartUsers".
2. **Inyección de Contexto (RAG):** Los datos recuperados de Pinecone se inyectan dentro de la etiqueta `<contexto_sistema>` para que el LLM separe claramente la instrucción de los datos crudos.
3. **Reglas de Restricción (Guardrails):** Se instruye al modelo a responder "No sé" o denegar cortésmente cualquier solicitud que no pueda responderse utilizando única y exclusivamente el contexto inyectado.
4. **Tono:** Profesional, técnico y conciso.
5. **Filtro de Dominio:** El agente tiene prohibido responder sobre temas ajenos a la administración de usuarios (ej. clima, temas personales).

## 3. Técnicas de Optimización Aplicadas

Para calibrar el tono y asegurar el cumplimiento de las restricciones, inyectamos ejemplos de interacción (Few-Shot) directamente en el `ChatHistory` antes de procesar la consulta real del usuario:

- **Ejemplo In-Domain:** Se le enseña a responder de forma concisa sobre políticas del sistema.
- **Ejemplo Out-of-Domain:** Se le enseña a rechazar preguntas genéricas (ej. _"¿Cómo arreglo mi lavadora?"_) reconduciendo la conversación hacia la administración de usuarios.

## 4. Hiperparámetros

- **Temperature = 0.0:** Al tratarse de un asistente técnico corporativo, se prioriza la exactitud y el determinismo sobre la creatividad. Una temperatura de 0 minimiza drásticamente las alucinaciones.

## 5. Observabilidad y Evaluación (Métricas)

Para cumplir con los requisitos de optimización y control de costos, el microservicio extrae métricas vitales en cada respuesta:

- **Latencia:** Tracking mediante Stopwatch para medir la eficiencia del pipeline RAG.
- **Uso de Tokens:** Extracción dinámica de metadatos del LLM para registrar Prompt, Completion y Total Tokens.
- **Estimación de Costos:** Algoritmo de cálculo basado en los precios vigentes del modelo por cada millón de tokens (Input/Output).
