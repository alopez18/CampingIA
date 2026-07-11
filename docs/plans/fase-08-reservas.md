# Fase 8 — Reservas

> Objetivo: gestión de reservas.

## Domain
- [ ] Confirmar `Reservation` con `ReservationDateVO` (validación de fechas: inicio < fin, no en pasado).
- [ ] Estados de reserva (p. ej. `Pending`, `Confirmed`, `Cancelled`) y método `Cancel()`.

## Application
### Commands (`Commands/Reservation/...`)
- [ ] `CreateReservationCommand` + handler (valida disponibilidad/fechas, persiste).
- [ ] `CancelReservationCommand` + handler (transición de estado + borrado lógico si aplica).

### Queries (`Queries/Reservation/...`)
- [ ] `GetReservationByIdQuery` + handler.
- [ ] `GetUserReservationsQuery` + handler.

### Validators
- [ ] Create/Cancel validators.

## Infra
- [ ] Repositorios `Reservations` (read/write) con Dapper.

## WebApi
### Endpoints (`[Authorize]`)
- [ ] `POST   /api/reservations`
- [ ] `DELETE /api/reservations/{id}`
- [ ] `GET    /api/reservations`
- [ ] DTOs + mappers.

## DI
- [ ] Registrar handlers, mappers, validators.

## Tests
- [ ] Creación con fechas válidas/ inválidas; cancelación; listado por usuario.

## Criterio de aceptación
- Reservas creadas/canceladas con validación de fechas y consulta por usuario.
