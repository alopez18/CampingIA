# Fase 14 — Dockerización

> Objetivo: `docker compose up` levanta toda la plataforma.

## Servicios del compose
- [ ] `campingai-api` — `CampingAI.WebApi` (Dockerfile existente, target Linux).
- [ ] `campingai-mobile` — build/serve de la app Ionic (PWA).
- [ ] `sqlserver` — imagen MSSQL con volumen persistente + init script (`0.CreateDatabase.sql`).
- [ ] `qdrant` — base vectorial con volumen persistente (para Fase 12).

## Configuración
- [ ] Red interna entre servicios.
- [ ] Variables de entorno / secrets: cadena SQL, JWT, claves OpenAI, endpoint Qdrant.
- [ ] Healthchecks (api, sqlserver, qdrant) y `depends_on`.
- [ ] Volúmenes para SQL Server y Qdrant.

## Verificación
- [ ] `docker compose up` arranca todos los servicios sin errores.
- [ ] API accesible; conexión a SQL y Qdrant operativa.

## Best practices
- [ ] Aplicar Azure/container best practices para despliegue.

## Criterio de aceptación
- Plataforma completa (api + mobile + sqlserver + qdrant) operativa con un único `docker compose up`.
