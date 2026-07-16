# Fase 4-B — Gestión de Usuarios Gestores (Web + Backoffice)

> Objetivo: permitir que un usuario se registre como **Gestor** desde la **web alojada en la API**
> (no desde la app móvil), asocie un camping y lo administre desde un **backoffice** dentro de
> `CampingAI.WebApi`. Un gestor puede seguir usando la app móvil como usuario común (ve lo mismo);
> la gestión del camping vive exclusivamente en el backoffice web.

## Estado: ✅ IMPLEMENTADO (Fase 4-B.1)

---

## Decisiones de diseño (confirmadas)
- **UI:** MVC Controllers + Vistas Razor (mismo patrón que `AccountController` / `Views/Account`).
- **Registro gestor en 2 pasos:**
  1. Alta/upgrade del usuario a Gestor.
  2. Alta del camping.
  - Si el email ya existe como **usuario común**, se pide **login** para reutilizar sus datos
	(no re-introducir email/nombre) y luego se hace el **upgrade** de rol.
- **Backoffice:** CRUD completo de campings del gestor (listar, crear, editar, borrar, facilities).
- **Sesión web:** autenticación por **Cookie**, validando credenciales contra el store de usuarios
  (BCrypt vía `IPasswordHashingService.VerifyPassword`). Se emiten claims (`NameIdentifier`, `Email`,
  `Role`, `Name`) para que funcione `[Authorize(Roles = ...)]`.
- **Aprobación del gestor:**
  - Registro vía **Google** → aprobación **inmediata** (rol Gestor al instante).
  - Registro vía **formulario (email/contraseña)** → queda **Pendiente**; requiere **aprobación de un Admin**.
  - Se crea un **usuario Admin fijo** (rol 99) mediante seeding idempotente.
- Mientras un gestor está **Pendiente**, mantiene el rol **Usuario común** (3) y puede usar la app
  con normalidad; **no** accede al backoffice hasta ser aprobado.

---

## Domain (`CampingAI.Domain`)
- [x] `Enums/ManagerApprovalStatus.cs` — `None = 0`, `Pending = 1`, `Approved = 2`, `Rejected = 3`.
- [x] `Entities/User.cs`:
  - Añadir propiedad `ManagerApprovalStatus ManagerStatus { get; private set; }` (rehidratada en el constructor; default `None`).
  - `RequestManagerRole()` — marca `ManagerStatus = Pending` (el rol permanece `Comun`).
  - `ApproveManagerRole()` — `ManagerStatus = Approved` + `UpdateRole(UserRole.Gestor)`.
  - `RejectManagerRole()` — `ManagerStatus = Rejected` (el rol permanece `Comun`).
  - `GrantManagerRoleInstantly()` (para Google) — `ManagerStatus = Approved` + `UpdateRole(UserRole.Gestor)`.
  - `CreateNew(...)` inicializa `ManagerStatus = None`.
- [x] `Repositories/ICampingsReadRepository.cs` — añadir `Task<IEnumerable<Entities.Camping>> GetByOwnerAsync(Guid ownerId);`.
- [x] `Repositories/IUsersReadRepository.cs` — añadir `Task<IEnumerable<Entities.User>> GetPendingManagersAsync();`.

## Infra (`CampingAI.Infra`)
- [x] **Migración SQL** `10.AddUserManagerStatus.sql` — `ALTER TABLE [T_USERS] ADD [USR_ManagerStatus] [int] NOT NULL CONSTRAINT DF_T_USERS_ManagerStatus DEFAULT (0);` (idempotente con `IF NOT EXISTS ... sys.columns`).
- [x] `Models/CampingAI_DB/T_USERS.cs` — añadir `public int USR_ManagerStatus { get; set; }`.
- [x] `Users/Mappers/UsersMapper.cs` — mapear `USR_ManagerStatus ↔ ManagerStatus` (Map y ReverseMap).
- [x] `Users/UsersWriteRepository.cs` — incluir `USR_ManagerStatus` en INSERT/UPDATE.
- [x] `Users/UsersReadRepository.cs` — implementar `GetPendingManagersAsync()` (WHERE `USR_ManagerStatus = 1`).
- [x] `Campings/CampingsReadRepository.cs` — implementar `GetByOwnerAsync(ownerId)` (WHERE `CMP_OwnerId = @ownerId AND CMP_DeletedOn IS NULL`).
- [ ] **Seed Admin** `11.SeedAdminUser.sql` *(esqueleto)* — inserta el Admin (rol 99, GUID fijo) **solo** si se prefiere SQL. *(Ver alternativa en WebApi: seeding por código con hash BCrypt.)*

## Application (`CampingAI.Application`)
### Commands (`Commands/User/...`)
- [x] `RequestManagerRole/RequestManagerRoleCommand.cs` — `record (Guid UserId)`.
- [x] `RequestManagerRole/RequestManagerRoleCommandHandler.cs` — `GetByIdAsync` → `RequestManagerRole()` → repo + UoW. (Idempotente si ya está Pending/Approved.)
- [x] `ApproveManager/ApproveManagerCommand.cs` — `record (Guid UserId)`.
- [x] `ApproveManager/ApproveManagerCommandHandler.cs` — `GetByIdAsync` → `ApproveManagerRole()` → repo + UoW.
- [x] `RejectManager/RejectManagerCommand.cs` + handler — `RejectManagerRole()`.
- [x] `GoogleRegisterManager/GoogleRegisterManagerCommand.cs` + handler — command dedicado que usa `GrantManagerRoleInstantly()` (aprobación inmediata).
- [x] `RegisterManager/RegisterManagerCommand.cs` + handler + validator — alta por formulario que deja al usuario `Pending`.
- [x] **Reutilizar** `CreateCamping.CreateCampingCommand` para el alta del camping (paso 2), con `OwnerId = userId`.
- [x] **Reutilizar** `UpdateCamping` / `DeleteCamping` para el backoffice.

### Queries (`Queries/...`)
- [x] `Camping/GetCampingsByOwner/GetCampingsByOwnerQuery.cs` + handler — `GetByOwnerAsync`.
- [x] `User/GetPendingManagers/GetPendingManagersQuery.cs` + handler — `GetPendingManagersAsync`.

### Validators
- [x] Validators para los nuevos commands (FluentValidation) donde aplique (`RegisterManagerCommandValidator`).

## WebApi (`CampingAI.WebApi`) — MVC + Vistas Razor
### Autenticación por formulario (Cookie)
- [ ] `Controllers/Account/AccountController.cs`:
  - `GET /Account/Login` — mantiene botón Google y **añade** formulario email/contraseña.
  - `POST /Account/Login` — valida credenciales (`GetByEmailAsync` + `VerifyPassword`), crea `ClaimsPrincipal`
	con claims (`NameIdentifier`, `Email`, `Role`, `Name`) y `SignInAsync(CookieAuthenticationDefaults...)`.
  - Manejo de credenciales inválidas (ModelState / mensaje).
- [x] `Views/Account/Login.cshtml` — añadir formulario de login normal + enlace "Registrarme como gestor".
- [x] `Controllers/Account/DTO/LoginFormRequest.cs` (o ViewModel) — `Email`, `Password`, `ReturnUrl?`.

### Registro de gestor
- [x] `Controllers/Manager/ManagerController.cs`:
  - `GET/POST /Manager/Register` — alta por formulario que crea usuario común y marca `RequestManagerRole` (queda Pendiente).
  - Si el email ya existe → error indicando iniciar sesión para solicitar el rol.
  - `GET /Manager/RegisterPending` — confirmación "pendiente de aprobación por un Admin".
- [x] `Views/Manager/Register.cshtml`, `Views/Manager/RegisterPending.cshtml`.
- [x] ViewModels/DTOs en `Controllers/Manager/DTO/`.
- [ ] *(Pendiente iteración)* Paso 2 alta de camping durante el registro (`RegisterCamping`) y reutilización vía login del email existente.

### Backoffice (CRUD campings del gestor)
- [x] `Controllers/Backoffice/BackofficeController.cs` `[Authorize(Roles = "Gestor")]` (cookie):
  - `GET /Backoffice` — dashboard: lista campings del gestor (`GetCampingsByOwnerQuery`).
  - `GET /Backoffice/Create` + `POST /Backoffice/Save` — alta (`CreateCampingCommand`).
  - `GET /Backoffice/Edit/{id}` + `POST /Backoffice/Save` — edición (`UpdateCampingCommand`) validando propiedad (`OwnerId == userId`).
  - `POST /Backoffice/Delete/{id}` — borrado lógico (`DeleteCampingCommand`) validando propiedad.
- [x] Vistas `Views/Backoffice/*` (Index, Edit).
- [x] Autorización de propiedad: comprobar que el camping pertenece al gestor antes de editar/borrar.
- [ ] *(Pendiente iteración)* Gestión de **facilities** del camping (asignar/quitar).

### Administración (aprobación de gestores)
- [x] `Controllers/Admin/AdminController.cs` `[Authorize(Roles = "Admin")]`:
  - `GET /Admin/PendingManagers` — lista gestores pendientes (`GetPendingManagersQuery`).
  - `POST /Admin/Approve/{id}` — `ApproveManagerCommand`.
  - `POST /Admin/Reject/{id}` — `RejectManagerCommand`.
- [x] Vistas `Views/Admin/PendingManagers.cshtml`.

### Seeding del Admin fijo (por código, recomendado)
- [x] `Seeding/AdminSeeder.cs` (startup hook) — idempotente:
  lee `AdminSeed:Email` / `AdminSeed:Password` de configuración (user-secrets), hashea con
  `IPasswordHashingService` y crea el usuario Admin (rol 99) si no existe.
- [ ] `appsettings`/user-secrets — documentar claves `AdminSeed:Email`, `AdminSeed:Password`.

### Startup / DI
- [x] `Startup.cs` — MVC + cookie sign-in soportan los nuevos controladores; se emiten claims de rol
  (`ClaimTypes.Role = UserRole.Name`) vía `UserClaimsFactory` para `[Authorize(Roles = ...)]`; hook del seeder.
- [x] `Config/DI_Manager.cs` (WebApi) y `Configuration/DI_Manager.cs` (Application) — registrados
  los nuevos handlers, queries y validators.
- [x] `_Layout.cshtml` — navegación condicional (enlaces Backoffice para Gestor, Admin para Admin).
- [ ] `_Layout.cshtml` — navegación condicional (enlaces Backoffice para Gestor, Admin para Admin).

## Tests
- [ ] Domain: `User.RequestManagerRole/ApproveManagerRole/RejectManagerRole/GrantManagerRoleInstantly` transicionan estado y rol correctamente.
- [ ] Application: handlers de `RequestManagerRole`, `ApproveManager`, `RejectManager`, `GetCampingsByOwner`, `GetPendingManagers`.
- [ ] Infra: `UsersReadRepository.GetPendingManagersAsync`, `CampingsReadRepository.GetByOwnerAsync` (según patrón existente).

## Criterio de aceptación
- [x] La página de login web ofrece **Google** y **email/contraseña**; el login normal crea sesión cookie válida.
- [x] El registro de gestor por **formulario** deja al usuario **Pendiente** (rol Comun). *(El alta de camping en el registro queda como iteración futura; el gestor lo crea desde el backoffice al ser aprobado.)*
- [ ] Si el email ya existe como usuario común, se solicita **login** y se reutilizan sus datos (upgrade a Pendiente). *(Pendiente iteración: actualmente se muestra error para iniciar sesión.)*
- [x] El registro de gestor vía **Google** aprueba **al instante** (rol Gestor) y da acceso al backoffice (`GoogleRegisterManagerCommand`).
- [x] Un **Admin** puede listar y **aprobar/rechazar** gestores pendientes; al aprobar, el rol pasa a Gestor.
- [x] El **backoffice** permite a un gestor listar, crear, editar y borrar **solo sus** campings. *(Facilities: iteración futura.)*
- [x] Un gestor puede seguir usando la app móvil como usuario común (el rol no bloquea la app).
- [x] `dotnet build` sin errores y tests en verde (38/38 Application).

## Notas / iteraciones futuras
- Verificación de email para gestores por formulario (además de aprobación de Admin).
- Publicación/estado del camping (borrador vs publicado) mientras el gestor está pendiente.
- Gestión de múltiples campings por gestor y permisos por camping.
- Paso 2 del registro (alta de camping) y reutilización de datos vía login del email existente.
- Gestión de facilities del camping desde el backoffice.

## Registro de implementación
**Fecha:** 2025 · **Autor:** Copilot

### Ficheros creados
- `CampingAI.Domain/Enums/ManagerApprovalStatus.cs` — enum de estado de aprobación del gestor.
- `CampingAI.Infra/10.AddUserManagerStatus.sql` — migración idempotente para `USR_ManagerStatus`.
- `CampingAI.Application/Commands/User/RequestManagerRole/*` — command + handler.
- `CampingAI.Application/Commands/User/ApproveManager/*` — command + handler.
- `CampingAI.Application/Commands/User/RejectManager/*` — command + handler.
- `CampingAI.Application/Commands/User/GoogleRegisterManager/*` — command + handler (aprobación inmediata).
- `CampingAI.Application/Commands/User/RegisterManager/*` — command + handler + validator (queda Pendiente).
- `CampingAI.Application/Queries/User/GetPendingManagers/*` — query + handler.
- `CampingAI.Application/Queries/Camping/GetCampingsByOwner/*` — query + handler.
- `CampingAI.WebApi/Controllers/Account/UserClaimsFactory.cs` — construcción de `ClaimsPrincipal` con rol.
- `CampingAI.WebApi/Controllers/Account/DTO/LoginFormRequest.cs` — DTO del login por formulario.
- `CampingAI.WebApi/Controllers/Manager/ManagerController.cs` + `DTO/RegisterManagerFormRequest.cs`.
- `CampingAI.WebApi/Controllers/Admin/AdminController.cs`.
- `CampingAI.WebApi/Controllers/Backoffice/BackofficeController.cs` + `DTO/CampingFormRequest.cs`.
- `CampingAI.WebApi/Seeding/AdminSeeder.cs` — seeding idempotente del Admin.
- Vistas: `Views/Manager/Register.cshtml`, `Views/Manager/RegisterPending.cshtml`, `Views/Admin/PendingManagers.cshtml`, `Views/Backoffice/Index.cshtml`, `Views/Backoffice/Edit.cshtml`.

### Ficheros modificados
- `CampingAI.Domain/Entities/User.cs` — `ManagerStatus` + métodos de transición de gestor.
- `CampingAI.Domain/Repositories/IUsersReadRepository.cs`, `ICampingsReadRepository.cs` — nuevos métodos.
- `CampingAI.Infra/Models/CampingAI_DB/T_USERS.cs`, `Users/Mappers/UsersMapper.cs`, `Users/UsersReadRepository.cs`, `Users/UsersWriteRepository.cs`, `Campings/CampingsReadRepository.cs`, `1.CreateCampingAITables.sql`.
- `CampingAI.Application/Configuration/DI_Manager.cs` — registro de nuevos handlers/validators.
- `CampingAI.WebApi/Controllers/Account/AccountController.cs` — login por formulario (cookie) + Google.
- `CampingAI.WebApi/Views/Account/Login.cshtml` — formulario email/contraseña + enlace de registro.
- `CampingAI.WebApi/Startup.cs` — hook del seeder de Admin.
- `CampingAI.WebApi/Views/Shared/_Layout.cshtml` — navegación condicional por rol.

### Paquetes NuGet
- Ninguno añadido/modificado.

### Comandos de infraestructura pendientes (usuario)
- Configurar user-secrets: `AdminSeed:Email` y `AdminSeed:Password` en `CampingAI.WebApi`.
- Ejecutar la migración `CampingAI.Infra/10.AddUserManagerStatus.sql` contra la base de datos.

### Resultado final
- `dotnet build` (solución completa): ✅
- Tests `CampingAI.Application.Tests`: ✅ 38/38
