# Desempeño bajo presión – Resolución de problema

# Análisis y Diagnóstico de Fallas

**Contexto del incidente:** Se realizan reportes de fallas al guardar registros en la aplicación, se observan errores HTTP 500 y alta latencia en las respuestas del agente de IA.

---

## 1. Formulación de Hipótesis

Cuando nos enfrentamos a un escenario donde fallan las escrituras y la IA se vuelve lenta al mismo tiempo, mi experiencia trabajando en el desarrollo y mantenimiento de sistemas complejos me dice que rara vez estamos ante bugs aislados. Lo más probable es que estemos viendo una falla provocada por un cuello de botella en los servicios.

Tengo dos hipótesis principales y una correlación:

**A. Saturacion en los servicios de bases de datos y mensajería:**
Al ver errores internos al intentar guardar me dice que están surgiendo excepciones no controladas en el codigo, esto podría ser que el `User Service` está agotando el _connection pool_ de PostgreSQL por una alta cantidad de conexiones abiertas, lo que provoca _timeouts_ en nuevas peticiones. Otra causa podría ser un bloqueo en RabbitMQ: si las colas se llenan podría hacer que nuestro codigo se quede en una espera indefinida. Esto termina bloqueando a los servicios publicadores.

**B. Latencia en los servicos de IA:**
Sé que cuando el usuario reporta que el agente está lento, muchas veces no es un problema de procesamiento del modelo. Es muy probable que estemos golpeando el _Rate Limit_ de la API de nuestro servicio de IA (OpenAI) y nuestro microservicio esté atrapado en un ciclo de reintentos internos.
También podemos mencionar la posibilidad de que el problema sea Redis Stack: si el cálculo de similitud de cosenos para los _embeddings_ se queda sin RAM, nuestro RAG se congela antes de siquiera hacer la petición al LLM.

**C. Correlación de Fallos**
Cuando el agente de IA se vuelve lento ya sea por límites en la API del LLM o búsquedas vectoriales pesadas en Redis, los hilos de ejecución de .NET se quedan atrapados esperando una respuesta. Y por otro lado si la infraestructura falla, un PostgreSQL saturado o un RabbitMQ bloqueado que no acepta más mensajes, el servicio de IA se ve obligado a esperar o reintentar sus operaciones de escritura y auditoría. Este bloqueo infla artificialmente el tiempo de respuesta del agente. Al final esto crea una percepción de lentitud cuando el problema real podría ser un cuello de botella en los datos.

---

## 2. Plan de Acción y Diagnóstico

La prioridad absoluta es restablecer el flujo de escritura de registros:

1. **Revisión de la Infraestructura:**
   Revisaría el consumo de CPU y memoria de los contenedores de Postgres y RabbitMQ. Si se ve una gran cantidad de mensajes acumulados en una cola, sabría que falló.
2. **Trazabilidad del Error:**
   Usaría nuestra plataforma de logs centralizados filtrando por los errores 500 y tomaré el Trace ID de los _headers_ HTTP. Esto me permitirá seguir el camino exacto de una petición fallida desde el API Gateway hasta el punto donde surgio el problema.

---

## 3. Aislamiento del problema en la IA

En el caso del agente de IA, se requiere una estrategia de monitoreo distinta debido a sus costos y arquitectura. Para confirmar qué parte del RAG está fallando, revisaría las métricas dividiendo la latencia en tres fases exactas:

1. Tiempo en generar el _Embedding_.
2. Tiempo de búsqueda vectorial.
3. Tiempo de respuesta de generación del LLM.

A la par, auditaría los logs del orquestador buscando picos anómalos en el consumo de `prompt_tokens`. Un pico súbito podría indicar que caímos en un bucle infinito o que estamos sufriendo un intento de inyección de prompts.

**Mitigación rápida:** Si confirmo que la API del LLM nos está limitando, activaré de inmediato el patrón **Circuit Breaker**. Es mejor fallar rápido y mostrarle un mensaje claro al usuario que dejar conexiones HTTP abiertas consumiendo recursos vitales del servidor.

---

## 4. Comunicación con Stakeholders

En un incidente de producción, el manejo de la comunicación es crítico. Este sería mi protocolo:

- **Minuto 0:** Mensaje rápido al equipo de producto: _"Estamos investigando una falla generalizada en guardado y lentitud en la IA. Ingeniería ya está trazando los logs."_
- **Actualización de status:** Información clara y al grano: _"Identificamos un bloqueo en la base de datos que está rechazando las escrituras. Se requiere escalado de la instancia para mitigar el impacto."_
- **Resolución:** Una vez estabilizado el sistema, documentaré la causa raíz, la solución aplicada y los tickets técnicos que debemos agregar para garantizar que esto no se repita.

# Autor: Daniel Camacho
