# Fase 4 — Gestión de Usuarios

> Objetivo: registro, login (JWT) y consulta de usuario.

## Application (`CampingAI.Application`)

### Commands (carpeta por feature `Commands/User/...`)
- [ ] `RegisterUserCommand` + `RegisterUserCommandHandler`
  - Hashea contraseña (`PasswordHashingService`), crea `User` con `CreateNew`, persiste vía repo + `IUnitOfWork`.
- [ ] `LoginUserCommand` + `LoginUserCommandHandler`
  - Valida credenciales, genera token JWT.
- [ ] `UpdateUserCommand` + `UpdateUserCommandHandler`.

### Queries (`Queries/User/...`)
- [ ] `GetCurrentUserQuery` + handler.
- [ ] `GetUserByIdQuery` + handler.

### Validators (FluentValidation)
- [ ] Validators para Register/Login/Update.

### Servicios
- [ ] `IJwtTokenService` (generación de token) en Application/Infra.

## WebApi
### DTOs (`Controllers/api/Auth/DTO`, `Users/DTO`)
- [ ] Request/Response DTOs + mappers.

### Endpoints
- [ ] `POST /api/auth/register`
- [ ] `POST /api/auth/login`
- [ ] `GET  /api/users/me` (`[Authorize]`)
- [ ] `[ProducesResponseType(...)]` + `Shared.ErrorResponse`.
- [ ] Errores vía `GlobalExceptionMiddleware`.

## DI
- [ ] Registrar handlers, mappers, validators y `IJwtTokenService`.

## Tests
- [ ] Handlers y validators (Application.Tests); repos (Infra.Tests).

## Criterio de aceptación
- Registro + login devuelven token válido; `me` funciona con `[Authorize]`.
