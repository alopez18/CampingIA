# Fase 3 — Persistencia

> Objetivo del MasterPlan: implementar SqlConnectionFactory, Dapper, Unit Of Work y repositorios.

## Referencia
`EmployeesReadRepository` / `EmployeesWriteRepository`, `SqlConnectionFactory`,
`UnitOfWork`, `Models/REDARBOR_DB`, `ModelExtractor<T>` y `EmployeesMapper`.

## Tareas

### 1. Modelo de base de datos (SQL)
- [ ] Crear/actualizar scripts para las tablas:
  `Users`, `Campings`, `Categories`, `Services`, `Favorites`, `Reservations`,
  `CampingCategories`, `CampingServices`.
- [ ] Definir modelos POCO de BD en `CampingAI.Infra/Models` (uno por tabla).

### 2. Infraestructura base
- [ ] Confirmar `ISqlConnectionFactory` / `SqlConnectionFactory` (`using` por conexión).
- [ ] Confirmar `IUnitOfWork` / `UnitOfWork` (`SaveChangesAsync`).

### 3. Mappers (Infra → Domain)
- [ ] `UsersMapper`, `CampingsMapper`, `ReservationsMapper`, `FavoritesMapper`
  heredando de `SimpleMapper`/`CompleteMapper`, con `Map(IEnumerable<>)`.

### 4. Repositorios Dapper
Implementar en `CampingAI.Infra/Repositories` las interfaces de la Fase 2:
- [ ] Users (read/write)
- [ ] Campings (read/write) — incluye joins a Categories/Services vía tablas puente.
- [ ] Reservations (read/write)
- [ ] Favorites (read/write)

Cada repositorio:
- [ ] SQL construido con `StringBuilder` + `nameof(Models.<Tabla>.<Campo>)`.
- [ ] `ModelExtractor<T>` (`GetFieldNamesForSql()`, `GetTableNameForSql()`).
- [ ] try/catch + `ILogger<T>` + relanzar.
- [ ] `Guid` almacenado/consultado como `string` (`id.ToString()`).
- [ ] Escrituras: `SaveAsync(...)` + `IUnitOfWork.SaveChangesAsync()`.

### 5. DI
- [ ] Registrar repos y mappers en `CampingAI.Infra/Configuration/DI_Manager.cs`.

## Criterio de aceptación
- Repos compilan y resuelven en DI.
- Consulta/escritura básica contra SQL Server funciona (validar en Fase 13 con Infra.Tests).
