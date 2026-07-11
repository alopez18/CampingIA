# Fase 5 — Gestión de Campings

> Objetivo: CRUD completo de campings.

## Estado: ✅ COMPLETADA

## Application
### Commands (`Commands/Camping/...`)
- [x] `CreateCampingCommand` + handler (crea `Camping` con VOs `CampingNameVO`, `LatitudeVO`, `LongitudeVO`, `PriceVO`).
- [x] `UpdateCampingCommand` + handler.
- [x] `DeleteCampingCommand` + handler (borrado lógico vía `Deleteable`/`DeletedOn`).

### Queries (`Queries/Camping/...`)
- [x] `GetCampingsQuery` + handler (paginado — `GetPagedAsync` con `OFFSET/FETCH`).
- [x] `GetCampingByIdQuery` + handler (incluye `FacilityIds`).

### Validators
- [x] `CreateCampingCommandValidator` — Name, Description, Lat/Lon rangos, Price > 0, OwnerId, CategoryId.
- [x] `UpdateCampingCommandValidator` — CampingId + mismas reglas.
- [x] `DeleteCampingCommandValidator` — CampingId != Guid.Empty.

## Domain
- [x] `ICampingsReadRepository` extendida con `GetPagedAsync(int page, int pageSize)`.

## Infra
- [x] `CampingsReadRepository.GetPagedAsync` — `COUNT(*)` + `OFFSET/FETCH` + carga de `FacilityIds`.

## WebApi
### DTOs + mappers
- [x] `CreateCampingRequest` — Name, Description, Lat, Lon, PricePerNight, OwnerId, CategoryId, FacilityIds?.
- [x] `UpdateCampingRequest` — Name, Description, Lat, Lon, PricePerNight, CategoryId, FacilityIds?.
- [x] `CampingResponse` — Id, Name, Description, Lat, Lon, PricePerNight, OwnerId, CategoryId, FacilityIds, CreatedOn, UpdatedOn.
- [x] `PagedCampingsResponse` — Items, TotalCount, Page, PageSize.
- [x] `CampingResponseMapper` (`Camping → CampingResponse`).

### Endpoints
- [x] `GET    /api/campings`       → `GetCampingsQuery(page, pageSize)` → `PagedCampingsResponse` (200)
- [x] `GET    /api/campings/{id}`  → `GetCampingByIdQuery` → `CampingResponse` (200) / 404
- [x] `POST   /api/campings`       → `CreateCampingCommand` → `CampingResponse` (201) / 422
- [x] `PUT    /api/campings/{id}`  → `UpdateCampingCommand` → `CampingResponse` (200) / 422 / 404
- [x] `DELETE /api/campings/{id}`  → `DeleteCampingCommand` → 204 / 404

Todos con `[Authorize(JwtBearer)]` y `[ProducesResponseType]` completos.

## DI
- [x] `Application/Configuration/DI_Manager.cs` — 3 `ICommandHandler` + 2 `IQueryHandler` + 3 `IValidator` Camping.
- [x] `WebApi/Config/DI_Manager.cs` — `CampingResponseMapper`.

## Tests
- [ ] Handlers + repos (relaciones camping-categoría / camping-servicio). *(Pendiente — Fase 13)*

## Criterio de aceptación
- [x] CRUD funcional con validación de dominio y borrado lógico.
- [x] `dotnet build` sin errores ✅

---

## Registro de implementación

### 2025-07-13 — Implementación completa (Copilot)

#### Domain (`CampingAI.Domain`)
- `Repositories/ICampingsReadRepository.cs` — añadido `GetPagedAsync(int page, int pageSize)`.

#### Infra (`CampingAI.Infra`)
- `Campings/CampingsReadRepository.cs` — implementación de `GetPagedAsync`: `COUNT(*)` para total + `OFFSET/FETCH` para página + `LoadFacilitiesForCampingsAsync`.

#### Application — Commands (`CampingAI.Application/Commands/Camping/`)
- `CreateCamping/CreateCampingCommand.cs` — record: Name, Description, Latitude, Longitude, PricePerNight, OwnerId, CategoryId, FacilityIds?
- `CreateCamping/CreateCampingCommandHandler.cs` — validator → `Camping.CreateNew` → `SetFacilities` → repo + UoW
- `CreateCamping/CreateCampingCommandValidator.cs` — Name, Description, Lat/Lon rangos, Price > 0, OwnerId != Empty, CategoryId > 0
- `UpdateCamping/UpdateCampingCommand.cs` — record: CampingId, Name, Description, Latitude, Longitude, PricePerNight, CategoryId, FacilityIds?
- `UpdateCamping/UpdateCampingCommandHandler.cs` — validator → `GetByIdAsync` (KeyNotFoundException) → `UpdateDetails` + `UpdateLocation` + `SetFacilities` → `Updated()` → repo + UoW
- `UpdateCamping/UpdateCampingCommandValidator.cs`
- `DeleteCamping/DeleteCampingCommand.cs` — record: CampingId
- `DeleteCamping/DeleteCampingCommandHandler.cs` — validator → `GetByIdAsync` (KeyNotFoundException) → `DeleteAsync` + UoW
- `DeleteCamping/DeleteCampingCommandValidator.cs` — CampingId != Guid.Empty

#### Application — Queries (`CampingAI.Application/Queries/Camping/`)
- `GetCampings/GetCampingsQuery.cs` — record: Page, PageSize; implementa `IQuery<GetCampingsResult>`
- `GetCampings/GetCampingsResult.cs` — record: Items, TotalCount
- `GetCampings/GetCampingsQueryHandler.cs` — `GetPagedAsync` → `GetCampingsResult`
- `GetCampingById/GetCampingByIdQuery.cs` — record: CampingId; implementa `IQuery<Camping>`
- `GetCampingById/GetCampingByIdQueryHandler.cs` — `GetByIdAsync` → KeyNotFoundException si null

#### Application — DI (`Configuration/DI_Manager.cs`)
- Registrados 3 `ICommandHandler` Camping, 2 `IQueryHandler` Camping, 3 `IValidator` Camping.

#### WebApi — DTOs y Mappers (`Controllers/api/Campings/`)
- `DTO/CreateCampingRequest.cs`
- `DTO/UpdateCampingRequest.cs`
- `DTO/CampingResponse.cs`
- `DTO/PagedCampingsResponse.cs`
- `Mappers/CampingResponseMapper.cs` — `Camping → CampingResponse` (VOs via `.Value` / `.ToString()`)

#### WebApi — Controller
- `CampingsController.cs` — 5 endpoints, `[Authorize(JwtBearer)]`, `ProducesResponseType` completos, `CreatedAtAction` en POST.

#### WebApi — DI (`Config/DI_Manager.cs`)
- Registrado `CampingResponseMapper`.

**Resultado:** `dotnet build` ✅
