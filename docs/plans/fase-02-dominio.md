# Fase 2 — Dominio

> Objetivo del MasterPlan: crear Entidades, Value Objects, interfaces de repositorio y excepciones de dominio.

## Referencia
La entidad `Employee` y los VOs existentes (`EmailVO`, `EmployeeUserNameVO`,
`PasswordHashedVO`, `DateFromPastVO`) son la plantilla a seguir.

## Entidades a crear (`CampingAI.Domain/Entities`)
- [ ] `User` — registro, autenticación, favoritos.
- [ ] `Camping` — información principal, ubicación, categorías, servicios.
- [ ] `Reservation` — gestión de reservas.
- [ ] `Category` — clasificación de campings.
- [ ] `Service` — servicios disponibles.
- [ ] `Favorite` — relación usuario ↔ camping.

Cada entidad debe:
- Heredar de `Entity` / `Deleteable` e implementar `IAuditableEntity`.
- Tener propiedades con `private set`.
- Exponer factory estática `CreateNew(...)` + constructor de rehidratación.
- Mutar solo mediante métodos de intención (`UpdateXxx`, `Created`, `Updated`).

## Value Objects a crear (`CampingAI.Domain/ValueObjects`)
- [ ] `EmailVO` (reutilizable del existente).
- [ ] `CampingNameVO` — `SimpleStringRequiredValueObject`.
- [ ] `CampingDescriptionVO`.
- [ ] `LatitudeVO` — validar rango [-90, 90].
- [ ] `LongitudeVO` — validar rango [-180, 180].
- [ ] `PriceVO` — validar no negativo.
- [ ] `ReservationDateVO` — validar coherencia de fechas.

Todos: validar en constructor, lanzar `DomainException` ante entrada inválida.

## Interfaces de repositorio (`CampingAI.Domain/Repositories`)
Lectura / Escritura separadas, siguiendo `IEmpoyeesReadRepository` / `IEmployeesWriteRepository`:
- [ ] `IUsersReadRepository` / `IUsersWriteRepository`
- [ ] `ICampingsReadRepository` / `ICampingsWriteRepository`
- [ ] `IReservationsReadRepository` / `IReservationsWriteRepository`
- [ ] `IFavoritesReadRepository` / `IFavoritesWriteRepository`

## Excepciones
- [ ] Reutilizar `DomainException` para toda validación de estado inválido.

## Criterio de aceptación
- `CampingAI.Domain` compila sin dependencias externas.
- Cada VO lanza `DomainException` con entradas inválidas (cubierto en Fase 13).
