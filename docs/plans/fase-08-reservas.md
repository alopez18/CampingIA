# Fase 8 — Reservas

> Objetivo: gestión de reservas.

## Domain
- [x] Confirmar `Reservation` con `ReservationDateVO` (validación de fechas: inicio < fin, no en pasado).
- [x] Estados de reserva (`Pending`, `Confirmed`, `Cancelled`) y método `Cancel()`.

## Application
### Commands (`Commands/Reservation/...`)
- [x] `CreateReservationCommand` + handler (valida disponibilidad/fechas, persiste).
- [x] `CancelReservationCommand` + handler (transición de estado + borrado lógico si aplica).

### Queries (`Queries/Reservation/...`)
- [x] `GetReservationByIdQuery` + handler.
- [x] `GetUserReservationsQuery` + handler.

### Validators
- [x] Create/Cancel validators.

## Infra
- [x] Repositorios `Reservations` (read/write) con Dapper.

## WebApi
### Endpoints (`[Authorize]`)
- [x] `POST   /api/reservations`
- [x] `DELETE /api/reservations/{id}`
- [x] `GET    /api/reservations`
- [x] DTOs + mappers.

## DI
- [x] Registrar handlers, mappers, validators.

## Tests
- [x] Creación con fechas válidas/ inválidas; cancelación; listado por usuario.

## Criterio de aceptación
- [x] Reservas creadas/canceladas con validación de fechas y consulta por usuario.

---

## Registro de implementación

**Fecha:** 2025-07-14 | **Autor:** GitHub Copilot

### Ficheros creados
| Fichero | Descripción |
|---|---|
| `CampingAI.Domain/Enums/ReservationStatus.cs` | Enum `Pending`, `Confirmed`, `Cancelled` |
| `CampingAI.Application/Commands/Reservation/CreateReservation/CreateReservationCommand.cs` | Record command de creación |
| `CampingAI.Application/Commands/Reservation/CreateReservation/CreateReservationCommandValidator.cs` | Validador FluentValidation |
| `CampingAI.Application/Commands/Reservation/CreateReservation/CreateReservationCommandHandler.cs` | Handler: crea reserva en estado Pending |
| `CampingAI.Application/Commands/Reservation/CancelReservation/CancelReservationCommand.cs` | Record command de cancelación |
| `CampingAI.Application/Commands/Reservation/CancelReservation/CancelReservationCommandValidator.cs` | Validador FluentValidation |
| `CampingAI.Application/Commands/Reservation/CancelReservation/CancelReservationCommandHandler.cs` | Handler: verifica propiedad y llama `Cancel()` |
| `CampingAI.Application/Queries/Reservation/GetReservationById/GetReservationByIdQuery.cs` | Query por id con control de ownership |
| `CampingAI.Application/Queries/Reservation/GetReservationById/GetReservationByIdQueryHandler.cs` | Handler de la query anterior |
| `CampingAI.Application/Queries/Reservation/GetUserReservations/GetUserReservationsQuery.cs` | Query de reservas del usuario actual |
| `CampingAI.Application/Queries/Reservation/GetUserReservations/GetUserReservationsQueryHandler.cs` | Handler de la query anterior |
| `CampingAI.WebApi/Controllers/api/Reservations/DTO/CreateReservationRequest.cs` | DTO de petición POST |
| `CampingAI.WebApi/Controllers/api/Reservations/DTO/ReservationResponse.cs` | DTO de respuesta |
| `CampingAI.WebApi/Controllers/api/Reservations/Mappers/ReservationResponseMapper.cs` | Mapper `Reservation` → `ReservationResponse` |
| `CampingAI.WebApi/Controllers/api/Reservations/ReservationsController.cs` | Controller con POST, DELETE, GET |
| `CampingAI.Domain.Tests/ValueObjects/ReservationDateVOTests.cs` | Tests de validación de fechas |
| `CampingAI.Domain.Tests/Entities/ReservationTests.cs` | Tests de `Cancel()` y `CreateNew()` |
| `CampingAI.Application.Tests/Commands/Reservation/CreateReservationCommandHandlerTests.cs` | Tests del handler de creación |
| `CampingAI.Application.Tests/Commands/Reservation/CancelReservationCommandHandlerTests.cs` | Tests del handler de cancelación (owner, not found, not owner) |
| `CampingAI.Application.Tests/Queries/Reservation/GetUserReservationsQueryHandlerTests.cs` | Tests del query handler de listado |

### Ficheros modificados
| Fichero | Descripción |
|---|---|
| `CampingAI.Domain/Entities/Reservation.cs` | Añadido método `Cancel()` |
| `CampingAI.Application/Configuration/DI_Manager.cs` | Registro de 2 command handlers, 2 query handlers y 2 validators |
| `CampingAI.WebApi/Config/DI_Manager.cs` | Registro de `ReservationResponseMapper` |

### Paquetes NuGet añadidos
Ninguno.

### Resultado final
- `dotnet build` ✅
- `105/105 tests` ✅

