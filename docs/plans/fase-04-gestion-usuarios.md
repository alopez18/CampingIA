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

# Fase 4.1 — Sistema de Roles

> Objetivo: formalizar los roles de usuario del sistema con validación en el dominio.
> No se crea tabla `T_ROLES`; `USR_RoleId` sigue siendo `int` en BD. La validación del
> conjunto permitido vive en el dominio mediante un Value Object.

## Estado: ✅ COMPLETADA

## Catálogo de roles
| Id  | Nombre         | Descripción                                             |
|-----|----------------|---------------------------------------------------------|
| 1   | Sistema        | Usuario técnico interno (propietario de campings import).|
| 2   | Gestor         | Gestor / propietario de camping.                        |
| 3   | Usuario común  | Usuario final de la app móvil (rol por defecto).        |
| 99  | Admin          | Administrador con acceso total.                         |

## Domain (`CampingAI.Domain`)
- [x] `Enums/UserRole.cs` — enum con los valores `Sistema = 1`, `Gestor = 2`, `Comun = 3`, `Admin = 99`.
- [x] `ValueObjects/RoleVO.cs` — VO que envuelve un `int Value`:
  - Constructor valida que el valor pertenezca al conjunto permitido (`Enum.IsDefined(typeof(UserRole), value)`); lanza `DomainException` si no.
  - Propiedad de conveniencia `Role` (cast a `UserRole`) y `Name` (nombre del rol).
  - `ToString`, `Equals`, `GetHashCode` siguiendo el patrón de `PriceVO`.
- [x] `Entities/User.cs` — reemplazar `int RoleId` por `RoleVO Role` (con `private set`):
  - Constructor rehidrata `Role = new RoleVO(roleId)`.
  - `CreateNew(...)` recibe `UserRole role` y construye el VO.
  - Añadido método de intención `UpdateRole(UserRole role)` para futuras iteraciones (uso Gestor/Admin).
  - Expuesto `RoleId => Role.Value` para compatibilidad con mappers/persistencia.

## Application (`CampingAI.Application`)
- [x] `RegisterUserCommandHandler` — el registro desde la app móvil **siempre** asigna
      `UserRole.Comun` (3), ignorando cualquier `RoleId` recibido en el request.
- [x] `RegisterUserCommand` / `RegisterRequest` — eliminado el campo `RoleId` del contrato público
      (registro móvil). *(El registro de Gestores vía API dedicada se abordará en una iteración posterior.)*
- [x] `RegisterUserCommandValidator` — quitada la regla `RoleId > 0` (ya no aplica).
- [x] `GoogleLoginUserCommandHandler` — cambiado `roleId: 2` por `UserRole.Comun` (3).
- [x] `LoginUserCommandHandler` / `GoogleLoginUserCommandHandler` — claim `ClaimTypes.Role`
      usa `user.RoleId` (compila sin cambios con el VO).

## WebApi (`CampingAI.WebApi`)
- [x] `Auth/DTO/RegisterRequest.cs` — eliminado `RoleId`.
- [x] `Auth/AuthController.cs` — ajustada la construcción de `RegisterUserCommand` (sin `RoleId`).
- [x] `Users/DTO/UserResponse.cs` — mantiene `RoleId`; añadido `RoleName`.
- [x] `Users/Mappers/UserResponseMapper.cs` — mapea `RoleId` y `RoleName`.

## Infra (`CampingAI.Infra`)
- [x] `Users/Mappers/UsersMapper.cs` — mapeo `USR_RoleId ↔ RoleId` verificado (compatibilidad vía `RoleId`).
- [x] `9.SeedSystemUser.sql` — el usuario Sistema ya usa `RoleId = 1` (sin cambios).
- [x] Comentario documentando el catálogo de roles añadido en `1.CreateCampingAITables.sql`.

## Tests
- [x] `RoleVO` — VO acepta 1/2/3/99 y lanza `DomainException` para valores inválidos (0, 4, -1).
- [ ] `User.CreateNew` — asigna correctamente el rol. *(Cubierto indirectamente; test dedicado pendiente — Fase 13)*
- [ ] `RegisterUserCommandHandler` — fuerza `UserRole.Comun` (3). *(Pendiente — Fase 13)*
- [ ] `GoogleLoginUserCommandHandler` — asigna `UserRole.Comun` (3). *(Pendiente — Fase 13)*

## Criterio de aceptación
- [x] `RoleVO` valida el conjunto permitido `{1, 2, 3, 99}`.
- [x] Registro móvil siempre crea usuarios con rol 3.
- [x] Login Google crea usuarios con rol 3.
- [x] `dotnet build` sin errores y tests en verde.

### 2025-07-12 — Fase 4.1 Sistema de Roles (Copilot)

#### Domain (`CampingAI.Domain`)
- `Enums/UserRole.cs` (nuevo) — enum `Sistema=1`, `Gestor=2`, `Comun=3`, `Admin=99`.
- `ValueObjects/RoleVO.cs` (nuevo) — VO `int Value`, valida vía `Enum.IsDefined`, propiedades `Role`/`Name`.
- `Entities/User.cs` (modificado) — `RoleVO Role` + `RoleId => Role.Value`; `CreateNew` recibe `UserRole`; añadido `UpdateRole(UserRole)`.

#### Application (`CampingAI.Application`)
- `Commands/User/RegisterUser/RegisterUserCommand.cs` (modificado) — eliminado `RoleId`.
- `Commands/User/RegisterUser/RegisterUserCommandHandler.cs` (modificado) — fuerza `UserRole.Comun`.
- `Commands/User/RegisterUser/RegisterUserCommandValidator.cs` (modificado) — eliminada regla `RoleId`.
- `Commands/User/GoogleLoginUser/GoogleLoginUserCommandHandler.cs` (modificado) — `UserRole.Comun` en lugar de `roleId: 2`.

#### WebApi (`CampingAI.WebApi`)
- `Controllers/api/Auth/DTO/RegisterRequest.cs` (modificado) — eliminado `RoleId`.
- `Controllers/api/Auth/AuthController.cs` (modificado) — comando sin `RoleId`.
- `Controllers/api/Users/DTO/UserResponse.cs` (modificado) — añadido `RoleName`.
- `Controllers/api/Users/Mappers/UserResponseMapper.cs` (modificado) — mapea `RoleName`.

#### Infra (`CampingAI.Infra`)
- `1.CreateCampingAITables.sql` (modificado) — comentario con el catálogo de roles sobre `T_USERS`.

#### Tests (`CampingAI.Domain.Tests`)
- `ValueObjects/RoleVOTests.cs` (nuevo) — 9 tests (1/2/3/99 válidos; 0/4/-1/100 inválidos; enum). ✅ 9/9.

**Resultado:** `dotnet build` ✅ · `RoleVOTests` 9/9 ✅

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

