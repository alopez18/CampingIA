# Fase 7 — Favoritos

> Objetivo: gestión de favoritos por usuario.

## Application
### Commands (`Commands/Favorite/...`)
- [ ] `AddFavoriteCommand` + handler (evita duplicados usuario+camping).
- [ ] `RemoveFavoriteCommand` + handler.

### Queries (`Queries/Favorite/...`)
- [ ] `GetFavoritesQuery` + handler (favoritos del usuario autenticado).

### Validators
- [ ] Add/Remove validators.

## Infra
- [ ] Repositorios `Favorites` (read/write) con Dapper.
- [ ] Restricción de unicidad usuario+camping en BD.

## WebApi
### Endpoints (`[Authorize]`)
- [ ] `POST   /api/favorites`
- [ ] `DELETE /api/favorites/{campingId}`
- [ ] `GET    /api/favorites`
- [ ] DTOs + mappers.

## DI
- [ ] Registrar handlers, mappers, validators.

## Tests
- [ ] Añadir/eliminar/duplicados; listado por usuario.

## Criterio de aceptación
- Un usuario puede añadir, listar y eliminar favoritos sin duplicados.
