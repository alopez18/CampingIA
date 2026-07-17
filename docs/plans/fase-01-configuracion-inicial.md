# Fase 1 — Configuración Inicial

> Objetivo del MasterPlan: dejar la solución arrancando con `docker compose up`,
> con SQL Server, Swagger, JWT y FluentValidation operativos y la arquitectura verificada.

## Estado actual

La solución ya existe con las 4 capas + proyectos de test y wiring de DI por capa.

## Tareas

### 1. Verificación de arquitectura y dependencias
- [x] Confirmar dirección de dependencias: `WebApi → Application → Infra → Domain`.
- [x] Verificar que `Domain` no tiene dependencias externas.
- [x] Revisar los `Configuration/DI_Manager.cs` de cada capa y su encadenamiento.

### 2. Configuración de base de datos
- [x] Revisar/actualizar `CampingAI.Infra/0.CreateDatabase.sql` como script de bootstrap CampingAI.
- [x] Definir cadena de conexión en `appsettings.json` / user-secrets (no hardcodear).
- [x] Verificar `SqlConnectionFactory` y `ISqlConnectionFactory`.

### 3. Swagger
- [x] Confirmar Swagger activo en Development (`Startup.ConfigureApp`).
- [x] Documentar esquema JWT Bearer en Swagger.

### 4. Autenticación JWT
- [x] Añadir/confirmar esquema JWT Bearer junto a Cookie + Google.
- [x] Configurar `JwtSettings` (Issuer, Audience, Key) desde configuración/user-secrets.
- [x] Registrar servicio de generación/validación de tokens en `Application`/`WebApi`.

### 5. FluentValidation
- [x] Añadir paquete `FluentValidation` a `CampingAI.Application` (usar herramienta NuGet).
- [x] Definir estrategia de validación (validators por Command/Query) y punto de integración (pipeline del Mediator o filtro en WebApi).

### 6. Docker
- [x] Revisar `Dockerfile` + `docker-compose.yml` en `CampingAI.WebApi`.
- [x] Añadir servicio `sqlserver` al compose.
- [ ] Verificar arranque end-to-end con `docker compose up`.

## Criterio de aceptación
- [x] `dotnet build CampingAI.sln` sin errores.
- [ ] `docker compose up` levanta API + SQL Server. *(pendiente de verificar en entorno con Docker)*
- [x] Swagger accesible con soporte JWT.
- [x] Un validator de FluentValidation de ejemplo se ejecuta correctamente.

---

## Registro de implementación

### 2025-07-11 — Implementación completa (Copilot)

**Ficheros creados:**
- `CampingAI.WebApi/Settings/JwtSettings.cs` — POCO para configuración JWT.
- `CampingAI.WebApi/Services/JwtTokenService.cs` — Implementación `IJwtTokenService` con HmacSha256, token 8h.
- `CampingAI.Application/Services/JwtTokenService/Interfaces/IJwtTokenService.cs` — Contrato de generación de tokens.

**Ficheros modificados:**
- `CampingAI.WebApi/appsettings.json` — Eliminados secretos hardcodeados; placeholders para Google, Jwt:Key y connection string renombrada a `CAMPING_AI_SqlServer`.
- `CampingAI.WebApi/Startup.cs` — Título Swagger → `CampingAI API`; añadido `AddJwtBearer` con `TokenValidationParameters`; usings para JWT.
- `CampingAI.WebApi/Config/DI_Manager.cs` — Registro de `JwtSettings` (IOptions) y `JwtTokenService`.
- `CampingAI.WebApi/Dockerfile` — Todas las referencias `RedArbor.TT.*` → `CampingAI.*`; usa `CampingAI.sln`.
- `CampingAI.WebApi/docker-compose.yml` — Container `sql-campingai`, network `campingai-network`, env var `CAMPING_AI_SqlServer`, path script SQL correcto, entrypoint `dotnet CampingAI.WebApi.dll`.

**User-secrets configurados** (proyecto `CampingAI.WebApi`, `UserSecretsId: aa4013aa-…`):
- `GoogleAuth:ClientId`, `GoogleAuth:ClientSecret`
- `Jwt:Key`
- `ConnectionStrings:CAMPING_AI_SqlServer`

**Resultado:** `dotnet build` ✅ — 64/64 tests ✅
