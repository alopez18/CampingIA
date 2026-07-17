# Fase 13 — Testing

> Objetivo: cobertura de Domain, Application, Infra y API con xUnit + FluentAssertions.

> **Nota:** el RAG se traslada a la **Fase Bonus** (`fase-bonus-rag.md`) y solo se implementará si da tiempo. Por tanto, esta fase **no** incluye tests de RAG (retrieval/embeddings/Qdrant); dichos tests quedan documentados en la fase bonus.

## Convenciones
- `Method_Should_ExpectedBehavior_WhenCondition`.
- Arrange / Act / Assert.
- `[Theory]` + `[InlineData]` para matrices; `[Fact]` para casos únicos.
- Los proyectos de test reflejan la capa; sin código de producción en tests.

## Domain Tests (`CampingAI.Domain.Tests`)
- [x] Value Objects: validación y `DomainException` (Email, CampingName, Latitude, Longitude, Price, ReservationDate).
- [x] Entidades: factory `CreateNew`, métodos de intención, borrado lógico.

## Application Tests (`CampingAI.Application.Tests`)
- [x] Handlers de Commands/Queries (mock de repos + `IUnitOfWork`).
- [x] Validators FluentValidation (SearchCampingsQueryValidator).
- [x] Búsqueda avanzada (combinaciones de filtros).

## Infrastructure Tests (`CampingAI.Infra.Tests`)
- [ ] Repositorios Dapper (integración contra SQL de test o contenedor).
- [x] Mappers Infra→Domain (Campings, Users, Reservations).

## API Tests
- [ ] (Opcional) Proyecto de tests de integración WebApi con `WebApplicationFactory`.
- [ ] Endpoints principales: auth, campings, search, favorites, reservations.

## Ejecución
- [x] `dotnet test` en verde.

## Criterio de aceptación
- [x] Suite de tests pasa; cobertura significativa de dominio y handlers.

## Registro de implementación

**Fecha:** 2025-07 — **Autor:** Copilot

### Ficheros creados
- `CampingAI.Domain.Tests/ValueObjects/CampingNameVOTests.cs` — tests de `CampingNameVO` (válido, null/vacío, Equals)
- `CampingAI.Domain.Tests/ValueObjects/LatitudeVOTests.cs` — tests de `LatitudeVO` (rango -90..90, casos borde, Equals, ToString)
- `CampingAI.Domain.Tests/ValueObjects/LongitudeVOTests.cs` — tests de `LongitudeVO` (rango -180..180, casos borde, Equals, ToString)
- `CampingAI.Domain.Tests/ValueObjects/PriceVOTests.cs` — tests de `PriceVO` (>=0, negativo lanza DomainException, cero válido, Equals)
- `CampingAI.Domain.Tests/Entities/CampingTests.cs` — tests de entidad `Camping` (CreateNew, UpdateDetails, UpdateLocation, facilidades, categorías, borrado lógico)
- `CampingAI.Domain.Tests/Entities/UserTests.cs` — tests de entidad `User` (CreateNew, UpdateProfile, UpdateEmail, UpdatePassword, UpdateRole, RequestManagerRole, ApproveManagerRole, RejectManagerRole, GrantManagerRoleInstantly, SetDeleted)
- `CampingAI.Domain.Tests/Entities/FavoriteTests.cs` — tests de entidad `Favorite` (CreateNew, constructor con id vacío, ids únicos)
- `CampingAI.Application.Tests/Commands/Camping/CreateCampingCommandHandlerTests.cs` — handler Crear Camping (happy path, facilidades+categorías, ValidationException)
- `CampingAI.Application.Tests/Commands/Camping/UpdateCampingCommandHandlerTests.cs` — handler Actualizar Camping (happy path, KeyNotFoundException)
- `CampingAI.Application.Tests/Commands/Camping/DeleteCampingCommandHandlerTests.cs` — handler Eliminar Camping (happy path, KeyNotFoundException)
- `CampingAI.Application.Tests/Commands/User/RegisterUserCommandHandlerTests.cs` — handler Registro (happy path, email duplicado, hash de contraseña)
- `CampingAI.Application.Tests/Commands/User/LoginUserCommandHandlerTests.cs` — handler Login (happy path, usuario no encontrado, contraseña incorrecta)
- `CampingAI.Application.Tests/Queries/Camping/GetCampingByIdQueryHandlerTests.cs` — query por id (happy path, KeyNotFoundException)
- `CampingAI.Application.Tests/Queries/Camping/GetCampingsByOwnerQueryHandlerTests.cs` — query por propietario (con resultados, vacío)
- `CampingAI.Application.Tests/Queries/Camping/GetCampingsQueryHandlerTests.cs` — query paginada (con resultados, vacío)
- `CampingAI.Infra.Tests/Campings/CampingsMapperTests.cs` — mapper T_CAMPINGS ↔ Camping (Map, ReverseMap, colección, campos opcionales)
- `CampingAI.Infra.Tests/Users/UsersMapperTests.cs` — mapper T_USERS ↔ User (Map, ReverseMap, DeletedOn, ManagerStatus)
- `CampingAI.Infra.Tests/Reservations/ReservationsMapperTests.cs` — mapper T_RESERVATIONS ↔ Reservation (Map, ReverseMap, colección, DeletedOn)

### Ficheros modificados
- `CampingAI.Domain/ValueObjects/DateFromPastVO.cs` — corregido bug en `CreateNow()`: usaba `DateTime.UtcNow` directamente, provocando race condition con la validación `>= UtcNow` del constructor; ahora usa `DateTime.UtcNow.AddTicks(-1)` para garantizar que el valor siempre sea estrictamente pasado.

### Resultado final
- `dotnet build` ✅
- **253/253 tests** ✅ (0 fallidos) — todos los proyectos de test de la solución
