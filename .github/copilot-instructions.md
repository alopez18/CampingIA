# CampingAI — Instrucciones para Copilot

## Visión general
CampingAI es una solución en .NET 8 que implementa una **Clean Architecture** con un enfoque **CQRS** (Commands/Queries personalizados + Mediator). La capa web (`CampingAI.WebApi`) expone tanto una API REST (`api/RedArbor`) como Vistas MVC (Razor / autenticación Cookie + Google), construida originalmente como una "RedArbor Technical Test". La persistencia usa **Dapper** para lecturas/escrituras contra SQL Server, además de paquetes EF Core para tooling y configuración del contexto.

## Estructura de la solución (dirección de dependencias: WebApi → Application → Infra → Domain)
- **CampingAI.Domain** — Entidades, Value Objects, excepciones de dominio, interfaces de repositorio. Sin dependencias externas.
- **CampingAI.Application** — Commands, Queries, Handlers, DTOs, Mappers, el `Mediator`, servicios de aplicación (p. ej. `PasswordHashingService`). Referencia a Domain (y a abstracciones de Infra).
- **CampingAI.Infra** — Implementaciones de repositorio (Dapper), `SqlConnectionFactory`, `UnitOfWork`, modelos de BD (`Models/REDARBOR_DB`), scripts SQL. Referencia a Domain.
- **CampingAI.WebApi** — Controladores (API + MVC), DTOs, mappers, `Startup`/`Program`, cableado de DI, Swagger, autenticación, Docker.
- **Proyectos `*.Tests`** — Tests unitarios con xUnit + FluentAssertions que reflejan cada capa.

Cada capa tiene su propio `Configuration/DI_Manager.cs` (o `Config/DI_Manager.cs` en WebApi) que registra sus servicios y encadena con el `DI_Manager.Configure(...)` de la siguiente capa interna. Registra nuevos handlers/mappers/servicios en el `DI_Manager` correspondiente.

## Patrones de arquitectura
- **CQRS mediante abstracciones propias** (sin MediatR):
  - Commands: `ICommand`, `ICommandHandler<TCommand>` / `ICommandHandler<TCommand, TResult>` con `Task HandleAsync(...)`.
  - Queries: `IQuery<TResult>`, `IQueryHandler<TQuery, TResult>` con `Task<TResult> HandleAsync(...)`.
  - Despacho a través de `IMediator` (`SendCommandAsync` / `SendQueryAsync`), que resuelve los handlers desde el contenedor de DI. Nota: actualmente los controladores inyectan los handlers directamente además de `IMediator`.
- **Carpeta por feature** para commands/queries: `Commands/Employee/CreateEmployee/{CreateEmployeeCommand.cs, CreateEmployeeCommandHandler.cs}`.
- **Repositorios** separados en interfaces de lectura/escritura en `Domain/Repositories/...` (p. ej. `IEmpoyeesReadRepository`, `IEmployeesWriteRepository`), implementados en `Infra`.
- **Unit of Work** (`Infra.Abstractions.IUnitOfWork`) — los handlers de escritura llaman a `_repo.SaveAsync(...)` y después a `_unitOfWork.SaveChangesAsync()`.
- **Mappers** derivan de `Domain.Abstractions.Mappers.SimpleMapper<TSource,TDestination>` / `CompleteMapper` y exponen un `Map(IEnumerable<>)` por lotes. Registra los mappers en DI e inyéctalos; no hagas mapeo manual inline salvo que sigas un patrón ya existente en un controlador.

## Convenciones de modelado de dominio
- Las **Entidades** tienen propiedades con `private set` y solo mutan mediante métodos de intención explícitos (`UpdateIdentity`, `UpdateIdentifiers`, `UpdatePassword`, `Created`, `Updated`). Usa la factory estática `CreateNew(...)` para crear una nueva instancia; el constructor recibe el estado completo (usado al rehidratar desde persistencia).
- Las entidades extienden `Abstractions.Entities.Entity` / `Deleteable` e implementan `IAuditableEntity` para el seguimiento de creación/actualización; borrado lógico mediante `DeletedOn`.
- Los **Value Objects** envuelven primitivos, validan en el constructor y lanzan `DomainException` ante entradas inválidas. Clases base: `SimpleStringValueObject<T>`, `SimpleStringRequiredValueObject<T>`. Ejemplos: `EmailVO`, `EmployeeUserNameVO`, `PasswordHashedVO`, `DateFromPastVO`. Prefiere nuevos VOs frente a strings sin validar para campos validados.
- Los errores de validación de dominio siempre usan `Exceptions.DomainException`.

## Convenciones de acceso a datos
- Las lecturas/escrituras usan **Dapper** a través de `ISqlConnectionFactory.CreateConnection()` (envuelto en `using`).
- Construye el SQL con un `StringBuilder` y referencia los nombres de columna mediante `nameof(Models.REDARBOR_DB.T_EMPLOYEES.<Campo>)`; usa `ModelExtractor<TModel>` (`GetFieldNamesForSql()`, `GetTableNameForSql()`) para evitar strings de columna/tabla hardcodeados.
- Los repositorios mapean los modelos de BD a entidades de dominio mediante un `EmployeesMapper` inyectado.
- Envuelve las llamadas a BD en try/catch, registra con el `ILogger<T>` inyectado y relanza la excepción.
- Los ids `Guid` se almacenan/consultan como strings (`id.ToString()`).

## Convenciones de WebApi
- Los controladores de API viven bajo `Controllers/api/...`, usan `[ApiController]`, `[Route("api/[controller]")]`, `[Authorize]` y declaran `[ProducesResponseType(...)]` para el éxito + `Shared.ErrorResponse` para los errores.
- Los contratos de petición/respuesta son DTOs dedicados en una carpeta `DTO/` por controlador; envuelve las respuestas en el `ResponseWrapper` / DTOs de respuesta tipados existentes.
- El manejo global de errores está centralizado en `Handlers/GlobalExceptionMiddleware`; no añadas try/catch ad-hoc en los controladores para respuestas de error transversales.
- Autenticación: Cookie (esquema por defecto, `/Account/Login`) + Google OAuth; JWT Bearer configurado para Swagger. El `ClientId`/`ClientSecret` de Google provienen de la configuración (`GoogleAuth:*`) — son obligatorios, no hardcodees secretos; usa user-secrets.
- La lógica de arranque vive en `Startup.cs` (`ConfigureServices` / `ConfigureApp`); `Program.cs` solo llama a `Startup.Init`.
- Las Vistas Razor MVC viven bajo `Views/` (`_Layout`, `_ViewImports`, `_ViewStart`). Esto es Razor Pages/MVC — prefiérelo frente a Blazor.

## Estilo de código
- Lenguaje: C# 12 / .NET 8, `Nullable` habilitado, `ImplicitUsings` habilitado.
- **Llaves estilo K&R** (llave de apertura en la misma línea, incluyendo tipos y métodos): `public class Foo {`.
- Prefiere el **acceso a namespaces totalmente cualificado** a través de namespaces padre (p. ej. `Domain.Entities.Employee`, `Application.Abstractions.Command.ICommandHandler<...>`) como se usa en todo el proyecto, en lugar de añadir muchos `using`.
- Los campos de dependencia privados usan `readonly` (a menudo sin estilo solo-underscore — sigue los campos `_camelCase` ya presentes) y se agrupan en un bloque `#region Dependencias`/`#region Dependencies`.
- Usa `var` para variables locales cuando el tipo sea evidente.
- Respeta los ficheros `.editorconfig` de cada proyecto.

## Testing
- Framework: **xUnit** con **FluentAssertions** (`.Should()...`).
- Usa `[Theory]` + `[InlineData]` para matrices de entrada; `[Fact]` para casos únicos.
- Estructura los tests con comentarios Arrange/Act/Assert (ver `EmailVOTests`).
- Nomenclatura de métodos de test: `Method_Should_ExpectedBehavior_WhenCondition`.
- Los proyectos de test reflejan la capa bajo prueba (`CampingAI.Domain.Tests`, `CampingAI.Application.Tests`, `CampingAI.Infra.Tests`). No añadas código de producción a los proyectos de test.

## Compilación y ejecución
- Compilar: `dotnet build CampingAI.sln`.
- Test: `dotnet test`.
- Ejecutar la API: `dotnet run --project CampingAI.WebApi` (Swagger disponible en Development).
- Contenedor: `Dockerfile` + `docker-compose.yml` en `CampingAI.WebApi` (target Linux).
- Bootstrap de BD: `CampingAI.Infra/0.CreateDatabase.sql`.

## Al añadir una feature
1. Modela/valida en **Domain** (métodos de entidad + value objects, `DomainException` para estado inválido).
2. Añade métodos de repositorio a las interfaces de lectura/escritura (Domain) e impleméntalos en **Infra** (Dapper + mapper + logging).
3. Añade un Command/Query + Handler en **Application** dentro de una carpeta de feature; los handlers de escritura usan el repo + `IUnitOfWork`.
4. Añade DTOs + mapper y un endpoint en **WebApi**; registra todo en el `DI_Manager` correspondiente.
5. Añade tests unitarios en el proyecto `*.Tests` correspondiente.

## Al completar la implementación de una fase del MasterPlan
Una vez terminada la implementación de una fase (fichero `docs/plans/fase-XX-*.md`), actualiza **siempre** ese fichero:
1. Marca cada tarea completada con `[x]` (las no realizadas permanecen `[ ]`).
2. Actualiza los criterios de aceptación con `[x]` / `[ ]` según corresponda.
3. Añade al final del fichero una sección `## Registro de implementación` con:
   - Fecha y autor (Copilot o desarrollador).
   - Lista de **ficheros creados** y **ficheros modificados** con una línea de descripción cada uno.
   - Paquetes NuGet añadidos/modificados (nombre + versión).
   - Comandos de infraestructura ejecutados (user-secrets, migraciones, etc.).
   - Resultado final: estado del build y tests (`dotnet build` ✅/❌, `N/N tests` ✅/❌).
