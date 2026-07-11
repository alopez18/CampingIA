# Fase 5 — Gestión de Campings

> Objetivo: CRUD completo de campings.

## Application
### Commands (`Commands/Camping/...`)
- [ ] `CreateCampingCommand` + handler (crea `Camping` con VOs `CampingNameVO`, `LatitudeVO`, `LongitudeVO`, `PriceVO`).
- [ ] `UpdateCampingCommand` + handler.
- [ ] `DeleteCampingCommand` + handler (borrado lógico vía `Deleteable`/`DeletedOn`).

### Queries (`Queries/Camping/...`)
- [ ] `GetCampingsQuery` + handler (paginado).
- [ ] `GetCampingByIdQuery` + handler (incluye categorías/servicios).

### Validators
- [ ] Create/Update/Delete validators.

## WebApi
### DTOs + mappers
- [ ] Request/Response por endpoint.

### Endpoints
- [ ] `GET    /api/campings`
- [ ] `GET    /api/campings/{id}`
- [ ] `POST   /api/campings`
- [ ] `PUT    /api/campings/{id}`
- [ ] `DELETE /api/campings/{id}`

## DI
- [ ] Registrar handlers, mappers y validators.

## Tests
- [ ] Handlers + repos (relaciones camping-categoría / camping-servicio).

## Criterio de aceptación
- CRUD funcional con validación de dominio y borrado lógico.
