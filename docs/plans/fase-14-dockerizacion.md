# Fase 14 — Dockerización

> Objetivo: `docker compose up` levanta toda la plataforma.

> **Nota:** el servicio `qdrant` es **opcional** y solo se añadirá si se implementa la **Fase Bonus — RAG** (`fase-bonus-rag.md`). La plataforma obligatoria funciona sin él.

## Servicios del compose
- [x] `campingai-api` — `CampingAI.WebApi` (Dockerfile existente, target Linux).
- [x] `campingai-mobile` — build/serve de la app Ionic (PWA).
- [x] `sqlserver` — imagen MSSQL con volumen persistente + init script (`0.CreateDatabase.sql`).
- [ ] *(Opcional / Bonus RAG)* `qdrant` — base vectorial con volumen persistente (solo si se aborda la Fase Bonus).

## Configuración
- [x] Red interna entre servicios.
- [x] Variables de entorno / secrets: cadena SQL, JWT, `AISettings:ApiKey` (Google Gemini) y *(solo Bonus RAG)* endpoint Qdrant.
- [x] Healthchecks (api, sqlserver) y `depends_on`; incluir `qdrant` solo si se aborda la Fase Bonus.
- [x] Volúmenes para SQL Server y *(solo Bonus RAG)* Qdrant.

## Verificación
- [ ] `docker compose up` arranca todos los servicios sin errores.
- [ ] API accesible; conexión a SQL operativa (y a Qdrant solo si se incluye la Fase Bonus).

## Best practices
- [x] Aplicar Azure/container best practices para despliegue.

## Criterio de aceptación
- Plataforma completa (api + mobile + sqlserver) operativa con un único `docker compose up`.
- *(Opcional / Bonus RAG)* Si se aborda la Fase Bonus, `qdrant` se levanta y queda operativo en el mismo compose.

## Registro de implementación

**Fecha:** 2025-07-16 | **Autor:** Copilot

### Ficheros creados
- `CampingAI.Mobile/src/environments/environment.docker.ts` — Environment para Docker con `apiUrl: '/api'` (proxy nginx, sin CORS).
- `CampingAI.Mobile/nginx.conf` — Config nginx: sirve SPA + proxy `/api` → `campingai-webapi:8080`.
- `CampingAI.Mobile/Dockerfile` — Multistage: Node 22 build (`npm run build:docker`) + nginx alpine serve.

### Ficheros modificados
- `CampingAI.Mobile/angular.json` — Añadida configuración de build `docker` con `fileReplacements` hacia `environment.docker.ts` y `serviceWorker`.
- `CampingAI.Mobile/package.json` — Añadido script `build:docker`.
- `CampingAI.WebApi/docker-compose.yml` — Añadido servicio `campingai-mobile`; scripts `db-init` completados (9-13); healthcheck en `webapi`.

### Paquetes NuGet / npm
Ninguno.

### Comandos de infraestructura
```bash
# Arrancar la plataforma completa desde CampingAI.WebApi/
docker compose up --build
# Servicios levantados:
#   http://localhost:32777  → API (CampingAI.WebApi)
#   http://localhost:8102   → Mobile PWA (CampingAI.Mobile, nginx proxy /api)
#   sql-campingai:1433      → SQL Server (interno) / localhost:14333 (externo)
```

### Resultado final
- Build .NET: no aplica (solo ficheros Docker/config).
- Verificación manual pendiente: `docker compose up` desde entorno con Docker Desktop.
