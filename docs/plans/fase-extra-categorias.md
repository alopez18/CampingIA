# Fase Extra — Catálogo de Categorías de Camping

> Objetivo: introducir un **catálogo de categorías** de camping con relación **1:n** (un camping tiene una categoría principal y, opcionalmente, varias categorías adicionales), como fase previa a la Fase 11 (Inteligencia Artificial).

Esta fase es un prerrequisito de la Fase 11: el módulo de Búsqueda Inteligente resolverá la categoría principal + categorías adicionales y las facilities a partir del texto en lenguaje natural, por lo que necesita un catálogo real en base de datos.

---

## Contexto y modelo actual

- `T_FACILITIES` son **servicios/instalaciones** (Piscina, Wifi, Restaurante, Parque infantil…), en relación N:N con el camping mediante `T_CAMPING_FACILITIES`.
- `T_CAMPINGS.CMP_CategoryId` es hoy un **`int` NOT NULL único por camping**, **sin tabla catálogo ni FK**. Todos los campings importados usan `DefaultCategoryId = 1` (`CampingMigrationService`).

## Decisiones de diseño

1. **Categoría principal + puente**: se mantiene la categoría principal en `T_CAMPINGS.CMP_CategoryId` y se añade una tabla puente `T_CAMPING_CATEGORIES` para categorías adicionales (relación 1:n camping→categorías).
2. **Tipo Guid**: el catálogo usa `uniqueidentifier` (consistente con `T_FACILITIES`). `CMP_CategoryId` se migra de `int` → `Guid`.
3. Las categorías representan el **tipo/temática principal** del camping y **no** deben solaparse con facilities (p. ej. "Piscina" o "Con mascotas" siguen siendo facilities).

## Catálogo de categorías (Guids fijos, patrón `B1000001-0000-...`)

| # | Categoría | Descripción |
|---|-----------|-------------|
| 1 | **Familiar** | Orientado a familias con niños (recibe la migración del actual `CategoryId = 1`). |
| 2 | **Playa / Costero** | Junto al mar o primera línea de playa. |
| 3 | **Montaña** | En entorno de montaña o alta cota. |
| 4 | **Rural / Naturaleza** | Inmerso en naturaleza, entorno rural o parajes naturales. |
| 5 | **Lago / Río** | Junto a lago, embalse o río. |
| 6 | **Glamping / Lujo** | Alojamiento premium (safari tents, cabañas de lujo). |
| 7 | **Aventura / Deportivo** | Enfocado a deporte y actividades al aire libre. |
| 8 | **Tranquilo / Relax** | Ambiente silencioso, para desconectar. |
| 9 | **Urbano / City** | Cercano a una ciudad o núcleo urbano. |
| 10 | **Naturista** | Camping naturista / nudista. |

---

# Plan de implementación

## Paso 1 — Scripts SQL (Infra, idempotentes y numerados)

- [ ] `10.CreateCategoriesTables.sql`: crea `T_CATEGORIES` (`CAT_IdCategory uniqueidentifier` PK, `CAT_Name nvarchar(100)`) y `T_CAMPING_CATEGORIES` (`CCA_IdCampingCategory`, `CCA_CampingId`, `CCA_CategoryId` + PKs/FKs a `T_CAMPINGS` y `T_CATEGORIES`).
- [ ] `11.SeedCategories.sql`: seed idempotente de las 10 categorías con Guids fijos.
- [ ] `12.MigrateCampingCategoryToGuid.sql`: migra `CMP_CategoryId` de `int` → `uniqueidentifier` (1 → Familiar, resto → Familiar por defecto) y crea la FK `FK_CAMPINGS_CATEGORY`.
- [ ] Actualizar el orden de ejecución / `0.CreateDatabase.sql` si aplica.

## Paso 2 — Domain

- [ ] Entidad `Category` (Guid + `CampingNameVO`, `CreateNew` / `UpdateName`), espejo de `Facility`.
- [ ] Entidad puente `CampingCategory` (espejo de `CampingFacility`).
- [ ] `Camping`: `CategoryId` pasa de `int` a `Guid` (principal) + colección `Categories` / `AdditionalCategoryIds`, con métodos de intención (`UpdateCategory`, `SetAdditionalCategories`, `AddCategory`, `RemoveCategory`).
- [ ] Interfaces: `ICategoriesReadRepository` (`GetAllAsync`, `GetByIdAsync`, `GetByCampingIdAsync`), `ICategoriesWriteRepository`, `ICampingCategoriesWriteRepository` (`AddAsync`, `DeleteAsync`).

## Paso 3 — Infra (Dapper)

- [ ] Modelos `T_CATEGORIES` y `T_CAMPING_CATEGORIES` en `Models/CampingAI_DB`; ajustar `T_CAMPINGS.CMP_CategoryId` a `Guid`.
- [ ] `CategoriesMapper` (`CompleteMapper`), `CategoriesReadRepository`, `CategoriesWriteRepository`, `CampingCategoriesWriteRepository` (StringBuilder, `nameof`, `ModelExtractor`, try/catch + `ILogger`).
- [ ] Ajustar `CampingsMapper` (int → Guid en categoría principal) y la hidratación de `Camping` para cargar categorías adicionales vía join (como facilities).
- [ ] Registrar `ModelExtractor<T_CATEGORIES>`, `ModelExtractor<T_CAMPING_CATEGORIES>`, mappers y repos en `Infra/Configuration/DI_Manager.cs`.

## Paso 4 — Persistencia de camping

- [ ] Ajustar create/update de camping (commands, handlers, `CampingsWriteRepository`) para guardar la categoría principal (`CMP_CategoryId` Guid) y sincronizar las adicionales en la puente.

## Paso 5 — Búsqueda

- [ ] `CampingSearchFilters`: cambiar `int? CategoryId` por `IEnumerable<Guid>? CategoryIds` (busca en principal + puente).
- [ ] `CampingsReadRepository.SearchAsync`: filtrar por categorías (principal OR puente), técnica análoga a `FacilityIds`.
- [ ] Actualizar `SearchCampingsQuery` / `SearchCampingsQueryHandler` / `SearchCampingsQueryValidator` y el DTO `SearchCampingsRequest`.
- [ ] Revisar/ajustar `GetByCategoryAsync` (ahora Guid).

## Paso 6 — Application

- [ ] Query `GetCategories` (+ handler + result) que devuelve el catálogo completo.
- [ ] Registrar en `Application/Configuration/DI_Manager.cs`.

## Paso 7 — WebApi

- [ ] `CategoriesController` (`GET /api/categories`) + DTO `CategoryResponse` + mapper (registrado en `WebApi/Config/DI_Manager.cs`).
- [ ] Actualizar `CampingResponse` / `CampingResponseMapper` para exponer categoría principal + adicionales.
- [ ] Actualizar DTOs de camping (`CreateCampingRequest`, `UpdateCampingRequest`, backoffice `CampingFormRequest`) para `CategoryId` (Guid principal) + `AdditionalCategoryIds`.

## Paso 8 — DataImporter

- [ ] Ajustar `CampingMigrationService` (`DefaultCategoryId`) para usar el Guid de "Familiar" como categoría principal.

## Paso 9 — Tests

- [ ] Unit tests de nuevos handlers/repos (mock) y de la búsqueda por múltiples categorías.
- [ ] Ajustar tests existentes que dependan de `CategoryId (int)`.

## Paso 10 — Build, tests y documentación

- [ ] `dotnet build CampingAI.sln` ✅.
- [ ] `dotnet test` ✅.
- [ ] Actualizar este documento (marcar tareas y añadir el Registro de implementación).

---

# Criterios de Aceptación

- [ ] Existe tabla catálogo `T_CATEGORIES` con las 10 categorías sembradas.
- [ ] Existe tabla puente `T_CAMPING_CATEGORIES` con FKs correctas.
- [ ] `T_CAMPINGS.CMP_CategoryId` es `uniqueidentifier` con FK a `T_CATEGORIES` (categoría principal).
- [ ] Un camping puede tener una categoría principal y N categorías adicionales (relación 1:n).
- [ ] Endpoint `GET /api/categories` devuelve el catálogo.
- [ ] La búsqueda permite filtrar por una o varias categorías.
- [ ] Create/Update de camping persiste categoría principal + adicionales.
- [ ] Los campings existentes quedan migrados a la categoría "Familiar".
- [ ] Build y tests en verde.

---

## Registro de implementación

_(Pendiente de completar tras la implementación.)_
