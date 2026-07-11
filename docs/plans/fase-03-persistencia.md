# Fase 3 — Persistencia

> Objetivo del MasterPlan: implementar SqlConnectionFactory, Dapper, Unit Of Work y repositorios.

## Referencia
`EmployeesReadRepository` / `EmployeesWriteRepository`, `SqlConnectionFactory`,
`UnitOfWork`, `Models/REDARBOR_DB`, `ModelExtractor<T>` y `EmployeesMapper`.

> **Nota:** Los `Guid` se almacenan como `uniqueidentifier` en SQL Server y como `Guid` en los POCOs.
> Dapper convierte de forma nativa; no se usa `id.ToString()` ni `Guid.Parse()`.

## Tareas

### 1. Modelo de base de datos (SQL)
- [x] Crear/actualizar scripts para las tablas:
  `T_USERS`, `T_CAMPINGS`, `T_RESERVATIONS`, `T_FACILITIES`, `T_CAMPING_FACILITIES`, `T_FAVORITES`
  → `CampingAI.Infra/1.CreateCampingAITables.sql`
- [x] Definir modelos POCO de BD en `CampingAI.Infra/Models/CampingAI_DB` (uno por tabla).

### 2. Infraestructura base
- [x] Confirmar `ISqlConnectionFactory` / `SqlConnectionFactory` — ya existían ✅
- [x] Confirmar `IUnitOfWork` / `UnitOfWork` — ya existían ✅

### 3. Mappers (Infra → Domain)
- [x] `UsersMapper` (`CompleteMapper`) → `CampingAI.Infra/Users/Mappers/`
- [x] `CampingsMapper` (`CompleteMapper`) → `CampingAI.Infra/Campings/Mappers/`
- [x] `ReservationsMapper` (`CompleteMapper`) → `CampingAI.Infra/Reservations/Mappers/`
- [x] `FacilitiesMapper` (`CompleteMapper`) → `CampingAI.Infra/Facilities/Mappers/`
- [x] `FavoritesMapper` (`SimpleMapper`) → `CampingAI.Infra/Favorites/Mappers/`

### 4. Repositorios Dapper
- [x] `UsersReadRepository` / `UsersWriteRepository`
- [x] `CampingsReadRepository` / `CampingsWriteRepository` — incluye carga de `FacilityIds` desde `T_CAMPING_FACILITIES`
- [x] `ReservationsReadRepository` / `ReservationsWriteRepository`
- [x] `FacilitiesReadRepository` / `FacilitiesWriteRepository`
- [x] `CampingFacilitiesWriteRepository` — incluye `DeleteByCampingIdAsync`
- [x] `FavoritesReadRepository` / `FavoritesWriteRepository`

### 5. DI
- [x] Registrar repos, mappers y extractors en `CampingAI.Infra/Configuration/DI_Manager.cs`

## Criterio de aceptación
- [x] Repos compilan y resuelven en DI — `dotnet build` ✅ (2025-07-11)
- [ ] Consulta/escritura básica contra SQL Server funciona (validar en Fase 13 con Infra.Tests).

---

## Registro de implementación

### 2025-07-11 — Implementación completa (Copilot)

**POCOs creados (`CampingAI.Infra/Models/CampingAI_DB/`):**
- `T_USERS.cs`, `T_CAMPINGS.cs`, `T_RESERVATIONS.cs`
- `T_FACILITIES.cs`, `T_CAMPING_FACILITIES.cs`, `T_FAVORITES.cs`

**Script SQL:** `CampingAI.Infra/1.CreateCampingAITables.sql`

**Mappers (`CompleteMapper` / `SimpleMapper`):**
- `Users/Mappers/UsersMapper.cs`
- `Campings/Mappers/CampingsMapper.cs`
- `Reservations/Mappers/ReservationsMapper.cs`
- `Facilities/Mappers/FacilitiesMapper.cs`
- `Favorites/Mappers/FavoritesMapper.cs` (solo `Map`, sin `ReverseMap`)

**Repositorios Dapper:**
- `Users/UsersReadRepository.cs` / `UsersWriteRepository.cs`
- `Campings/CampingsReadRepository.cs` / `CampingsWriteRepository.cs`
- `Reservations/ReservationsReadRepository.cs` / `ReservationsWriteRepository.cs`
- `Facilities/FacilitiesReadRepository.cs` / `FacilitiesWriteRepository.cs`
- `Facilities/CampingFacilitiesWriteRepository.cs`
- `Favorites/FavoritesReadRepository.cs` / `FavoritesWriteRepository.cs`

**DI_Manager.cs** actualizado con todos los extractors, mappers y repos.

**Resultado:** `dotnet build` ✅
