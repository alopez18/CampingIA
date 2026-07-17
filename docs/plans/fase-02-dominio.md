# Fase 2 — Dominio

> Objetivo del MasterPlan: crear Entidades, Value Objects, interfaces de repositorio y excepciones de dominio.

## Estado: ✅ COMPLETADA

---

## Entidades (`CampingAI.Domain/Entities`)

Cada entidad:
- Hereda de `Entity` / `Deleteable` e implementa `IAuditableEntity` (cuando aplica).
- Propiedades con `private set`.
- Factory estática `CreateNew(...)` + constructor de rehidratación.
- Mutación solo mediante métodos de intención (`UpdateXxx`, `Created`, `Updated`).

| Entidad | Archivo | Base | `IAuditable` | Estado |
|---|---|---|---|---|
| `User` | `Entities/User.cs` | `Deleteable` | ✅ | ✅ |
| `Camping` | `Entities/Camping.cs` | `Deleteable` | ✅ | ✅ |
| `Reservation` | `Entities/Reservation.cs` | `Deleteable` | ✅ | ✅ |
| `Category` | `Entities/Category.cs` | `Entity` | — | ✅ |
| `Service` | `Entities/Service.cs` | `Entity` | — | ✅ |
| `Facility` | `Entities/Facility.cs` | `Entity` | — | ✅ |
| `CampingFacility` | `Entities/CampingFacility.cs` | `Entity` | — | ✅ |
| `Favorite` | `Entities/Favorite.cs` | `Entity` | — | ✅ |

### Notas de diseño

- `Camping` expone `IReadOnlyList<Guid> FacilityIds` con métodos `AddFacility`,
  `RemoveFacility` y `SetFacilities` (rehidratación desde repositorio).
- `CampingFacility` es la entidad de unión 1:N entre `Camping` y `Facility`.
- `Category`, `Service`, `Facility` y `Favorite` no requieren borrado lógico
  ni auditoría → heredan `Entity` directamente.

---

## Value Objects (`CampingAI.Domain/ValueObjects`)

Todos validan en constructor y lanzan `DomainException` ante entrada inválida.

| VO | Archivo | Validación | Estado |
|---|---|---|---|
| `EmailVO` | `ValueObjects/EmailVO.cs` | Formato email válido | ✅ (preexistente) |
| `CampingNameVO` | `ValueObjects/CampingNameVO.cs` | No nulo/vacío | ✅ |
| `CampingDescriptionVO` | `ValueObjects/CampingDescriptionVO.cs` | No nulo/vacío | ✅ |
| `LatitudeVO` | `ValueObjects/LatitudeVO.cs` | Rango `[-90, 90]` | ✅ |
| `LongitudeVO` | `ValueObjects/LongitudeVO.cs` | Rango `[-180, 180]` | ✅ |
| `PriceVO` | `ValueObjects/PriceVO.cs` | `>= 0` | ✅ |
| `ReservationDateVO` | `ValueObjects/ReservationDateVO.cs` | `CheckIn < CheckOut`, `CheckIn >= Today` | ✅ |

---

## Interfaces de repositorio (`CampingAI.Domain/Repositories`)

Lectura / Escritura separadas.

| Interfaz | Archivo | Estado |
|---|---|---|
| `IUsersReadRepository` | `Repositories/IUsersReadRepository.cs` | ✅ |
| `IUsersWriteRepository` | `Repositories/IUsersWriteRepository.cs` | ✅ |
| `ICampingsReadRepository` | `Repositories/ICampingsReadRepository.cs` | ✅ |
| `ICampingsWriteRepository` | `Repositories/ICampingsWriteRepository.cs` | ✅ |
| `IReservationsReadRepository` | `Repositories/IReservationsReadRepository.cs` | ✅ |
| `IReservationsWriteRepository` | `Repositories/IReservationsWriteRepository.cs` | ✅ |
| `IFacilitiesReadRepository` | `Repositories/IFacilitiesReadRepository.cs` | ✅ |
| `IFacilitiesWriteRepository` | `Repositories/IFacilitiesWriteRepository.cs` | ✅ |
| `ICampingFacilitiesWriteRepository` | `Repositories/ICampingFacilitiesWriteRepository.cs` | ✅ |
| `IFavoritesReadRepository` | `Repositories/IFavoritesReadRepository.cs` | ✅ |
| `IFavoritesWriteRepository` | `Repositories/IFavoritesWriteRepository.cs` | ✅ |

---

## Excepciones

- ✅ `DomainException` (preexistente en `Exceptions/DomainException.cs`) reutilizada
  en todos los VOs y en `CampingFacility`.

---

## Criterio de aceptación

- ✅ `CampingAI.Domain` compila sin dependencias externas.
- ✅ Cada VO lanza `DomainException` con entradas inválidas (tests cubiertos en Fase 13).

---

## Registro de implementación

### 2025-07-11 — Implementación completa (Copilot)

**Value Objects creados:**
- `CampingNameVO.cs` — `SimpleStringRequiredValueObject<CampingNameVO>`
- `CampingDescriptionVO.cs` — `SimpleStringRequiredValueObject<CampingDescriptionVO>`
- `LatitudeVO.cs` — rango `[-90, 90]`
- `LongitudeVO.cs` — rango `[-180, 180]`
- `PriceVO.cs` — `>= 0`
- `ReservationDateVO.cs` — `CheckIn < CheckOut`, `CheckIn >= Today`, propiedad `Nights`

**Entidades creadas:**
- `User.cs` — `Deleteable` + `IAuditableEntity`; `UpdateProfile`, `UpdateEmail`, `UpdatePassword`
- `Camping.cs` — `Deleteable` + `IAuditableEntity`; `UpdateDetails`, `UpdateLocation`, gestión de instalaciones (`AddFacility`, `RemoveFacility`, `SetFacilities`)
- `Reservation.cs` — `Deleteable` + `IAuditableEntity`; `UpdateDates`, `UpdateStatus`
- `Category.cs` — `Entity`; `UpdateName`
- `Service.cs` — `Entity`; `UpdateName`
- `Facility.cs` — `Entity`; `UpdateName`
- `CampingFacility.cs` — `Entity`; entidad de unión `CampingId` ↔ `FacilityId`
- `Favorite.cs` — `Entity`; `UserId`, `CampingId`, `CreatedAt`

**Interfaces de repositorio creadas:**
- `IUsersReadRepository.cs` / `IUsersWriteRepository.cs`
- `ICampingsReadRepository.cs` / `ICampingsWriteRepository.cs`
- `IReservationsReadRepository.cs` / `IReservationsWriteRepository.cs`
- `IFacilitiesReadRepository.cs` / `IFacilitiesWriteRepository.cs`
- `ICampingFacilitiesWriteRepository.cs` — incluye `DeleteByCampingIdAsync`
- `IFavoritesReadRepository.cs` / `IFavoritesWriteRepository.cs`

**Resultado:** `dotnet build` ✅
