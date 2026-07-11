# Fase 13 — Testing

> Objetivo: cobertura de Domain, Application, Infra y API con xUnit + FluentAssertions.

## Convenciones
- `Method_Should_ExpectedBehavior_WhenCondition`.
- Arrange / Act / Assert.
- `[Theory]` + `[InlineData]` para matrices; `[Fact]` para casos únicos.
- Los proyectos de test reflejan la capa; sin código de producción en tests.

## Domain Tests (`CampingAI.Domain.Tests`)
- [ ] Value Objects: validación y `DomainException` (Email, CampingName, Latitude, Longitude, Price, ReservationDate).
- [ ] Entidades: factory `CreateNew`, métodos de intención, borrado lógico.

## Application Tests (`CampingAI.Application.Tests`)
- [ ] Handlers de Commands/Queries (mock de repos + `IUnitOfWork`).
- [ ] Validators FluentValidation.
- [ ] Búsqueda avanzada (combinaciones de filtros).

## Infrastructure Tests (`CampingAI.Infra.Tests`)
- [ ] Repositorios Dapper (integración contra SQL de test o contenedor).
- [ ] Mappers Infra→Domain.

## API Tests
- [ ] (Opcional) Proyecto de tests de integración WebApi con `WebApplicationFactory`.
- [ ] Endpoints principales: auth, campings, search, favorites, reservations.

## Ejecución
- [ ] `dotnet test` en verde.

## Criterio de aceptación
- Suite de tests pasa; cobertura significativa de dominio y handlers.
