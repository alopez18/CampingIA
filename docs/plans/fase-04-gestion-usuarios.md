# Fase 4 — Gestión de Usuarios

> Objetivo: registro, login (JWT) y consulta de usuario.

## Estado: ✅ COMPLETADA

## Application (`CampingAI.Application`)

### Commands (carpeta por feature `Commands/User/...`)
- [x] `RegisterUserCommand` + `RegisterUserCommandHandler`
  - Hashea contraseña (`PasswordHashingService`), crea `User` con `CreateNew`, persiste vía repo + `IUnitOfWork`.
- [x] `LoginUserCommand` + `LoginUserCommandHandler`
  - Valida credenciales, genera token JWT.
- [x] `UpdateUserCommand` + `UpdateUserCommandHandler`.

### Queries (`Queries/User/...`)
- [x] `GetCurrentUserQuery` + handler.
- [x] `GetUserByIdQuery` + handler.

### Validators (FluentValidation)
- [x] Validators para Register/Login/Update.

### Servicios
- [x] `IJwtTokenService` (generación de token) en Application/Infra.
  → Preexistente desde Fase 1 (`Application/Services/JwtTokenService/Interfaces/IJwtTokenService.cs` + impl. en `WebApi/Services/JwtTokenService.cs`).

## WebApi
### DTOs (`Controllers/api/Auth/DTO`, `Users/DTO`)
- [x] Request/Response DTOs + mappers.

### Endpoints
- [x] `POST /api/auth/register`
- [x] `POST /api/auth/login`
- [x] `GET  /api/users/me` (`[Authorize(JwtBearer)]`)
- [x] `PUT  /api/users/{id}` (`[Authorize(JwtBearer)]`)
- [x] `[ProducesResponseType(...)]` + `Shared.ErrorResponse`.
- [x] Errores vía `GlobalExceptionMiddleware` (añadido catch `ValidationException` → HTTP 422).

## DI
- [x] Registrar handlers, mappers, validators y `IJwtTokenService`.

## Tests
- [ ] Handlers y validators (Application.Tests); repos (Infra.Tests). *(Pendiente — Fase 13)*

## Criterio de aceptación
- [x] Registro + login devuelven token válido; `me` funciona con `[Authorize]`.
- [x] `dotnet build` sin errores ✅ (2025-07-12)

---

## Registro de implementación

### 2025-07-12 — Implementación completa (Copilot)

#### Fix previo aplicado
- `GlobalExceptionMiddleware` — añadido catch `FluentValidation.ValidationException` → HTTP 422
  con lista de errores (`exception.Errors.Select(e => e.ErrorMessage)`).

#### Application — Commands (`CampingAI.Application/Commands/User/`)
- `RegisterUser/RegisterUserCommand.cs` — record: `Email`, `Password`, `Name?`, `RoleId`
- `RegisterUser/RegisterUserCommandHandler.cs` — valida → `ExistsAsync` (DomainException si duplicado) → `HashPassword` → `User.CreateNew` → repo + UoW
- `RegisterUser/RegisterUserCommandValidator.cs` — email válido, password ≥ 8 chars, RoleId > 0
- `LoginUser/LoginUserCommand.cs` — record: `Email`, `Password`
- `LoginUser/LoginUserCommandHandler.cs` — `GetByEmailAsync` → `VerifyPassword` → claims (`sub`, `email`, `role`, `name`) → `IJwtTokenService.GenerateToken` → devuelve `string`
- `LoginUser/LoginUserCommandValidator.cs` — email y password no vacíos
- `UpdateUser/UpdateUserCommand.cs` — record: `UserId`, `Name?`, `Email?`, `Password?`
- `UpdateUser/UpdateUserCommandHandler.cs` — `GetByIdAsync` (KeyNotFoundException si no existe) → `UpdateProfile/UpdateEmail/UpdatePassword` selectivos → `Updated()` → repo + UoW
- `UpdateUser/UpdateUserCommandValidator.cs` — UserId no vacío; Email/Password opcionales con reglas cuando presentes

#### Application — Queries (`CampingAI.Application/Queries/User/`)
- `GetCurrentUser/GetCurrentUserQuery.cs` — record: `UserId`
- `GetCurrentUser/GetCurrentUserQueryHandler.cs` — `GetByIdAsync` → `KeyNotFoundException` si null
- `GetUserById/GetUserByIdQuery.cs` — record: `UserId`
- `GetUserById/GetUserByIdQueryHandler.cs` — ídem

#### Application — DI (`Configuration/DI_Manager.cs`)
- Registrados 3 `ICommandHandler` User, 2 `IQueryHandler` User y 3 `IValidator` User.

#### WebApi — DTOs y Mappers
- `Controllers/api/Auth/DTO/RegisterRequest.cs` — record: `Email`, `Password`, `Name?`, `RoleId`
- `Controllers/api/Auth/DTO/LoginRequest.cs` — record: `Email`, `Password`
- `Controllers/api/Auth/DTO/AuthResponse.cs` — `Token`, `Email`, `Name?`
- `Controllers/api/Auth/Mappers/AuthResponseMapper.cs` — `(User, string token)` → `AuthResponse`
- `Controllers/api/Users/DTO/UserResponse.cs` — `Id`, `Email`, `Name?`, `RoleId`, `CreatedOn`
- `Controllers/api/Users/DTO/UpdateUserRequest.cs` — record: `Name?`, `Email?`, `Password?`
- `Controllers/api/Users/Mappers/UserResponseMapper.cs` — `User` → `UserResponse`

#### WebApi — Controllers
- `Controllers/api/Auth/AuthController.cs`
  - `POST /api/auth/register` `[AllowAnonymous]` → `RegisterUserCommand` → `LoginUserCommand` → `AuthResponse` (201)
  - `POST /api/auth/login` `[AllowAnonymous]` → `LoginUserCommand` → `AuthResponse` (200)
- `Controllers/api/Users/UsersController.cs` `[Authorize(JwtBearer)]`
  - `GET /api/users/me` → extrae `UserId` del claim `NameIdentifier` → `GetCurrentUserQuery` → `UserResponse` (200)
  - `PUT /api/users/{id}` → `UpdateUserCommand` → `UserResponse` (200)

#### WebApi — DI (`Config/DI_Manager.cs`)
- Registrados `AuthResponseMapper` y `UserResponseMapper`.

**Resultado:** `dotnet build` ✅

