# Fase 6 — Búsqueda Avanzada

> Objetivo: búsqueda de campings con filtros combinables.

## Application
### Queries (`Queries/Camping/Search/...`)
- [x] `SearchCampingsQuery` + handler (filtros combinables + paginación).
- [x] DTO de filtros: provincia, precio (min/max), servicios, categorías, mascotas, piscina.

### Validators
- [x] `SearchCampingsQueryValidator` (rangos de precio válidos, límites de paginación).

## Infra
- [x] Método de repositorio `Campings` (read) con SQL dinámico parametrizado (evitar inyección; `StringBuilder` + parámetros Dapper).
- [x] Joins con `CampingCategories` / `CampingServices` según filtros.

## WebApi
### DTOs + mappers
- [x] Request de filtros (query string) + Response paginada.

### Endpoints (`[Authorize]`)
- [x] `GET /api/campings/search`

## DI
- [x] Registrar handler, mappers y validator.

## Tests
- [x] Filtros individuales y combinados; paginación; sin resultados.

## Criterio de aceptación
- [x] La búsqueda devuelve campings filtrados por cualquier combinación de criterios, paginada y sin inyección SQL.

---

## Registro de implementación

**Fecha:** 2025-07-15  
**Autor:** GitHub Copilot

### Ficheros creados
| Fichero | Descripción |
|---|---|
| `CampingAI.Infra/2.AlterCampingsAddFilters.sql` | Script SQL: ALTER TABLE para añadir `CMP_Provincia` + seed "Toboganes en piscina" |
| `CampingAI.Domain/Repositories/CampingSearchFilters.cs` | Record con los filtros de búsqueda de campings |
| `CampingAI.Application/Queries/Camping/SearchCampings/SearchCampingsQuery.cs` | Query record con todos los filtros |
| `CampingAI.Application/Queries/Camping/SearchCampings/SearchCampingsResult.cs` | Result record con items paginados |
| `CampingAI.Application/Queries/Camping/SearchCampings/SearchCampingsQueryHandler.cs` | Handler que delega en `ICampingsReadRepository.SearchAsync` |
| `CampingAI.Application/Queries/Camping/SearchCampings/SearchCampingsQueryValidator.cs` | FluentValidation: rangos de precio, página, pageSize |
| `CampingAI.WebApi/Controllers/api/Campings/DTO/SearchCampingsRequest.cs` | DTO de request con `[FromQuery]` |
| `CampingAI.Application.Tests/Queries/Camping/SearchCampingsQueryHandlerTests.cs` | 8 tests unitarios del handler |
| `CampingAI.Application.Tests/Queries/Camping/SearchCampingsQueryValidatorTests.cs` | 10 tests unitarios del validator |

### Ficheros modificados
| Fichero | Descripción |
|---|---|
| `CampingAI.Domain/Entities/Camping.cs` | Añadidas propiedad `Provincia`; actualizado constructor, `CreateNew` y `UpdateDetails` |
| `CampingAI.Domain/Repositories/ICampingsReadRepository.cs` | Añadido método `SearchAsync(CampingSearchFilters)` |
| `CampingAI.Infra/Models/CampingAI_DB/T_CAMPINGS.cs` | Añadidas la columna nueva al modelo POCO |
| `CampingAI.Infra/Campings/Mappers/CampingsMapper.cs` | `Map` y `ReverseMap` actualizados con el nuevo campo |
| `CampingAI.Infra/Campings/CampingsReadRepository.cs` | Implementado `SearchAsync` con SQL dinámico parametrizado Dapper |
| `CampingAI.WebApi/Controllers/api/Campings/CampingsController.cs` | Inyectado nuevo handler; añadido endpoint `GET /api/campings/search` |
| `CampingAI.Application/Configuration/DI_Manager.cs` | Registrados `SearchCampingsQueryHandler` y `SearchCampingsQueryValidator` |

### Paquetes NuGet añadidos
Ninguno. Se reutilizan los ya existentes (Dapper, FluentValidation, xUnit, FluentAssertions, Moq).

### Comandos de infraestructura
```sql
-- Ejecutar manualmente contra CAMPING_AI_DB:
-- CampingAI.Infra/2.AlterCampingsAddFilters.sql
```

### Resultado final
- `dotnet build` ✅
- `23/23 tests` ✅

