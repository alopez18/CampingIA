# Fase 7 — Favoritos

> Objetivo: gestión de favoritos por usuario.

## Application
### Commands (`Commands/Favorite/...`)
- [x] `AddFavoriteCommand` + handler (evita duplicados usuario+camping).
- [x] `RemoveFavoriteCommand` + handler.

### Queries (`Queries/Favorite/...`)
- [x] `GetFavoritesQuery` + handler (favoritos del usuario autenticado).

### Validators
- [x] Add/Remove validators.

## Infra
- [x] Repositorios `Favorites` (read/write) con Dapper.
- [x] Restricción de unicidad usuario+camping en BD.

## WebApi
### Endpoints (`[Authorize]`)
- [x] `POST   /api/favorites`
- [x] `DELETE /api/favorites/{campingId}`
- [x] `GET    /api/favorites`
- [x] DTOs + mappers.

## DI
- [x] Registrar handlers, mappers, validators.

## Tests
- [x] Añadir/eliminar/duplicados; listado por usuario.

## Criterio de aceptación
- Un usuario puede añadir, listar y eliminar favoritos sin duplicados.

## Registro de implementación

**Fecha:** 2025-07-15 — Autor: Copilot

### Ficheros creados
- `CampingAI.Infra/8.AddFavoritesUniqueConstraint.sql` — Script SQL con restricción UNIQUE (UserId, CampingId).
- `CampingAI.Application/Commands/Favorite/AddFavorite/AddFavoriteCommand.cs` — Record command.
- `CampingAI.Application/Commands/Favorite/AddFavorite/AddFavoriteCommandHandler.cs` — Handler con guard de duplicado vía `ExistsAsync`.
- `CampingAI.Application/Commands/Favorite/AddFavorite/AddFavoriteCommandValidator.cs` — Validator FluentValidation.
- `CampingAI.Application/Commands/Favorite/RemoveFavorite/RemoveFavoriteCommand.cs` — Record command.
- `CampingAI.Application/Commands/Favorite/RemoveFavorite/RemoveFavoriteCommandHandler.cs` — Handler de borrado.
- `CampingAI.Application/Commands/Favorite/RemoveFavorite/RemoveFavoriteCommandValidator.cs` — Validator FluentValidation.
- `CampingAI.Application/Queries/Favorite/GetFavorites/GetFavoritesQuery.cs` — Record query.
- `CampingAI.Application/Queries/Favorite/GetFavorites/GetFavoritesQueryHandler.cs` — Handler de listado por usuario.
- `CampingAI.WebApi/Controllers/api/Favorites/DTO/AddFavoriteRequest.cs` — DTO de entrada.
- `CampingAI.WebApi/Controllers/api/Favorites/DTO/FavoriteResponse.cs` — DTO de respuesta.
- `CampingAI.WebApi/Controllers/api/Favorites/Mappers/FavoriteResponseMapper.cs` — Mapper entidad → DTO.
- `CampingAI.WebApi/Controllers/api/Favorites/FavoritesController.cs` — Controller con POST / DELETE / GET.
- `CampingAI.Application.Tests/Commands/Favorite/AddFavoriteCommandHandlerTests.cs` — 2 tests (happy path + duplicado).
- `CampingAI.Application.Tests/Commands/Favorite/RemoveFavoriteCommandHandlerTests.cs` — 1 test.
- `CampingAI.Application.Tests/Queries/Favorite/GetFavoritesQueryHandlerTests.cs` — 2 tests.

### Ficheros modificados
- `CampingAI.Application/Configuration/DI_Manager.cs` — Registrados commands, query y validators de Favorite.
- `CampingAI.WebApi/Config/DI_Manager.cs` — Registrado `FavoriteResponseMapper`.

### Paquetes NuGet añadidos
Ninguno.

### Comandos de infraestructura
- Ejecutar `8.AddFavoritesUniqueConstraint.sql` contra la BD para añadir la restricción de unicidad.

### Resultado final
- `dotnet build` ✅
- `5/5 tests` ✅

