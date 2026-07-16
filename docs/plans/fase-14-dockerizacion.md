# Fase 14 — Dockerización

> Objetivo: `docker compose up` levanta toda la plataforma.

> **Nota:** el servicio `qdrant` es **opcional** y solo se añadirá si se implementa la **Fase Bonus — RAG** (`fase-bonus-rag.md`). La plataforma obligatoria funciona sin él.

## Servicios del compose
- [ ] `campingai-api` — `CampingAI.WebApi` (Dockerfile existente, target Linux).
- [ ] `campingai-mobile` — build/serve de la app Ionic (PWA).
- [ ] `sqlserver` — imagen MSSQL con volumen persistente + init script (`0.CreateDatabase.sql`).
- [ ] *(Opcional / Bonus RAG)* `qdrant` — base vectorial con volumen persistente (solo si se aborda la Fase Bonus).

## Configuración
- [ ] Red interna entre servicios.
- [ ] Variables de entorno / secrets: cadena SQL, JWT, `AISettings:ApiKey` (Google Gemini) y *(solo Bonus RAG)* endpoint Qdrant.
- [ ] Healthchecks (api, sqlserver) y `depends_on`; incluir `qdrant` solo si se aborda la Fase Bonus.
- [ ] Volúmenes para SQL Server y *(solo Bonus RAG)* Qdrant.

## Verificación
- [ ] `docker compose up` arranca todos los servicios sin errores.
- [ ] API accesible; conexión a SQL operativa (y a Qdrant solo si se incluye la Fase Bonus).

## Best practices
- [ ] Aplicar Azure/container best practices para despliegue.

## Criterio de aceptación
- Plataforma completa (api + mobile + sqlserver) operativa con un único `docker compose up`.
- *(Opcional / Bonus RAG)* Si se aborda la Fase Bonus, `qdrant` se levanta y queda operativo en el mismo compose.
