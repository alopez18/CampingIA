# Fase 6 — Búsqueda Avanzada

> Objetivo: búsqueda de campings con filtros combinables.

## Application
### Queries (`Queries/Camping/Search/...`)
- [ ] `SearchCampingsQuery` + handler (filtros combinables + paginación).
- [ ] DTO de filtros: provincia, precio (min/max), servicios, categorías, mascotas, piscina.

### Validators
- [ ] `SearchCampingsQueryValidator` (rangos de precio válidos, límites de paginación).

## Infra
- [ ] Método de repositorio `Campings` (read) con SQL dinámico parametrizado (evitar inyección; `StringBuilder` + parámetros Dapper).
- [ ] Joins con `CampingCategories` / `CampingServices` según filtros.

## WebApi
### DTOs + mappers
- [ ] Request de filtros (query string) + Response paginada.

### Endpoints (`[Authorize]`)
- [ ] `GET /api/campings/search`

## DI
- [ ] Registrar handler, mappers y validator.

## Tests
- [ ] Filtros individuales y combinados; paginación; sin resultados.

## Criterio de aceptación
- La búsqueda devuelve campings filtrados por cualquier combinación de criterios, paginada y sin inyección SQL.
