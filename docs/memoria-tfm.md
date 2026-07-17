# CampingAI — Memoria del Trabajo Fin de Máster

> **Máster en Inteligencia Artificial Aplicada**  
> Autor: Alejandro López  
> Repositorio: https://github.com/alopez18/CampingIA

---

## Índice

1. [Problema y motivación](#1-problema-y-motivación)
2. [Objetivos](#2-objetivos)
3. [Arquitectura y decisiones de diseño](#3-arquitectura-y-decisiones-de-diseño)
4. [Tecnologías utilizadas](#4-tecnologías-utilizadas)
5. [Proceso de desarrollo por fases](#5-proceso-de-desarrollo-por-fases)
6. [IA aplicada](#6-ia-aplicada)
7. [Resultados y evaluación](#7-resultados-y-evaluación)
8. [Futuras mejoras](#8-futuras-mejoras)

---

## 1. Problema y motivación

El sector del turismo de camping en España mueve millones de pernoctaciones al año, pero la experiencia digital del usuario sigue siendo deficiente. Los portales de reserva existentes ofrecen búsquedas basadas en filtros simples (provincia, precio), sin capacidad de entender la intención real del usuario expresada en lenguaje natural. Un turista que quiere "un camping tranquilo con piscina cerca del mar ideal para familia con niños pequeños" se ve obligado a marcar decenas de checkboxes y revisar resultados manualmente.

**Problemas identificados:**

- Búsqueda deficiente basada en coincidencia exacta de filtros discretos.
- Ausencia de recomendaciones personalizadas basadas en el historial del usuario.
- Falta de herramientas de comparación inteligente entre establecimientos.
- Plataformas no diseñadas para dispositivos móviles (ausencia de PWA/app nativa).
- Gestión de campings aislada, sin portal de backoffice para gestores.

**Motivación:** aplicar las técnicas de Inteligencia Artificial estudiadas en el máster (LLMs, Prompt Engineering, generación de texto estructurado) para transformar la experiencia de búsqueda y descubrimiento de campings, demostrando que la IA puede mejorar de forma medible la usabilidad de una plataforma real.

---

## 2. Objetivos

### Objetivo principal

Desarrollar una plataforma full-stack de gestión y descubrimiento de campings con módulos de Inteligencia Artificial integrados, que demuestre la aplicación práctica de LLMs en el ciclo completo de una aplicación empresarial.

### Objetivos secundarios

| # | Objetivo | Estado |
|---|---|---|
| O1 | Implementar Clean Architecture + CQRS en .NET 8 como base mantenible y testeable | ✅ |
| O2 | Desarrollar API REST completa con autenticación JWT y OAuth (Google) | ✅ |
| O3 | Módulo IA de búsqueda semántica: lenguaje natural → filtros SQL → resultados | ✅ |
| O4 | Módulo IA de recomendaciones personalizadas basadas en historial del usuario | ✅ |
| O5 | Módulo IA de comparación inteligente entre campings con análisis cualitativo | ✅ |
| O6 | App móvil PWA con Ionic/Angular con funcionalidad completa offline-ready | ✅ |
| O7 | Mapa interactivo con geolocalización, clustering y búsqueda por área geográfica | ✅ |
| O8 | Panel de backoffice web para gestores con flujo de aprobación por administrador | ✅ |
| O9 | Suite de tests unitarios con cobertura de las capas Domain, Application e Infra | ✅ |
| O10 | Contenerización completa con Docker Compose (API + App + SQL Server) | ✅ |

---

## 3. Arquitectura y decisiones de diseño

### 3.1 Clean Architecture

El proyecto sigue la **Clean Architecture** de Robert C. Martin (Uncle Bob). Las dependencias fluyen siempre hacia el interior:

```
CampingAI.WebApi  →  CampingAI.Application  →  CampingAI.Domain
				  →  CampingAI.AI            →  CampingAI.Application
CampingAI.Infra   →  CampingAI.Domain
```

- **Domain:** entidades, value objects, interfaces de repositorio. Sin ninguna dependencia externa.
- **Application:** handlers CQRS, DTOs, validadores FluentValidation, servicios de aplicación.
- **Infra:** implementaciones Dapper, Unit of Work, SqlConnectionFactory, mappers de BD.
- **AI:** providers de IA, assistants (Search/Recommendations/Comparison), Semantic Kernel.
- **WebApi:** controladores REST, controladores MVC Razor, DI wiring, middleware.

**Decisión clave:** cada capa expone un `DI_Manager.Configure(services)` que encadena con la capa interior. Esto mantiene el registro de dependencias cohesionado y facilita las pruebas.

### 3.2 CQRS con Mediator propio

Se implementó **CQRS** (Command Query Responsibility Segregation) sin depender de librerías externas como MediatR, utilizando abstracciones propias:

- `ICommand` / `ICommandHandler<TCommand>` — operaciones de escritura.
- `IQuery<TResult>` / `IQueryHandler<TQuery,TResult>` — operaciones de lectura.
- `IMediator` — dispatcher que resuelve handlers del contenedor DI por reflection.

**Motivación:** aprendizaje profundo del patrón; control total sobre el pipeline; sin dependencias de terceros en el núcleo del dominio.

### 3.3 Acceso a datos con Dapper

Se eligió **Dapper** (micro-ORM) sobre Entity Framework Core por:

- Control total del SQL generado (importante para filtros dinámicos complejos en búsqueda).
- Menor overhead para operaciones de lectura masiva.
- SQL construido con `StringBuilder` + `nameof()` sobre modelos POCO para evitar strings hardcodeados.

El patrón **Unit of Work** envuelve las transacciones de escritura.

### 3.4 Value Objects y validación en dominio

Todos los campos con semántica propia están modelados como **Value Objects** que validan en su constructor y lanzan `DomainException` si el valor es inválido. Esto elimina la posibilidad de crear entidades en estado inconsistente.

Ejemplos: `EmailVO`, `LatitudeVO` (−90 a +90), `LongitudeVO` (−180 a +180), `PriceVO` (≥ 0), `ReservationDateVO` (CheckIn < CheckOut).

### 3.5 Autenticación dual

- **JWT Bearer:** para la API REST consumida por la app Ionic.
- **Cookie + Google OAuth:** para el acceso web al backoffice (gestores y administradores).
- **Flujo de aprobación:** los gestores registrados quedan en estado `Pending` hasta que un administrador los aprueba desde el panel `/Admin`.

---

## 4. Tecnologías utilizadas

### Backend

| Tecnología | Versión | Uso |
|---|---|---|
| .NET / ASP.NET Core | 8.0 | Framework principal |
| C# | 12 | Lenguaje |
| Dapper | 2.x | ORM ligero / acceso a datos |
| SQL Server | 2022 | Base de datos relacional |
| FluentValidation | 11.x | Validación de commands/queries |
| Microsoft Semantic Kernel | 1.x | Orquestación de IA |
| Google Gemini (gemini-2.5-flash) | API | Modelo de lenguaje grande (LLM) |
| Swashbuckle / Swagger | 6.x | Documentación API |
| xUnit + FluentAssertions + Moq | — | Testing |

### Frontend (App Móvil)

| Tecnología | Versión | Uso |
|---|---|---|
| Ionic | 8 | Framework UI móvil |
| Angular | 20 (standalone) | Framework SPA |
| Capacitor | 6 | Acceso a APIs nativas |
| Leaflet + leaflet.markercluster | — | Mapa interactivo |
| OpenStreetMap | — | Tiles cartográficos (libre) |
| @capacitor/geolocation | — | Geolocalización del dispositivo |

### DevOps / Infraestructura

| Tecnología | Uso |
|---|---|
| Docker + Docker Compose | Contenerización completa |
| Nginx | Reverse proxy para la app Ionic (PWA) |
| GitHub | Control de versiones |

---

## 5. Proceso de desarrollo por fases

| Fase | Nombre | Estado | Resultado |
|---|---|---|---|
| 01 | Configuración inicial | ✅ | Clean Architecture, JWT, Docker, Swagger, user-secrets |
| 02 | Dominio | ✅ | 9 entidades, 10+ Value Objects, interfaces de repositorio |
| 03 | Persistencia | ✅ | Repositorios Dapper, POCOs, mappers, script SQL |
| 04 | Gestión de usuarios | ✅ | Registro, login JWT, Google OAuth, perfil |
| 04-B | Usuarios gestores | ✅ | Flujo aprobación, backoffice MVC, panel admin |
| 05 | Gestión de campings | ✅ | CRUD completo, borrado lógico, paginación |
| 06 | Búsqueda avanzada | ✅ | Filtros combinables, provincia, precio, instalaciones, categorías |
| 07 | Favoritos | ✅ | Add/remove, UNIQUE constraint, listado |
| 08 | Reservas | ✅ | Create/cancel, validación de fechas, estados |
| 09 | App Ionic | ✅ | PWA + Android; login, lista, detalle, reservas, perfil |
| 10 | Mapa interactivo | ✅ | Leaflet, clustering, geolocalización, búsqueda por área |
| 11 | Inteligencia Artificial | ✅ | 3 módulos IA (búsqueda, recomendaciones, comparador) |
| 13 | Testing | ✅ | 253/253 tests (Domain + Application + Infra + AI) |
| 14 | Dockerización | ✅ | docker-compose con API + App + SQL Server + healthchecks |
| 15 | Documentación TFM | ✅ | Diagramas Mermaid + esta memoria |

> **Fase 12** no asignada. **Fase Bonus (RAG)** pendiente de implementación si hay tiempo disponible.

---

## 6. IA aplicada

La capa de IA (`CampingAI.AI`) está desacoplada del resto mediante la abstracción `IAIProvider`:

```
IAIProvider
  ├── GeminiAIProvider   (producción: Google Gemini 2.5 Flash)
  ├── CachingAIProvider  (decorador: caché en memoria)
  └── NullAIProvider     (fallback / tests: respuestas vacías controladas)
```

El `KernelFactory` configura **Microsoft Semantic Kernel** con el conector de Google Gemini. La clave API se gestiona mediante user-secrets (`AISettings:ApiKey`), nunca en código.

### Módulo 1 — Búsqueda Inteligente (`POST /api/ai/search`)

El usuario escribe en lenguaje natural ("camping familiar con piscina en Cataluña bajo 50€ la noche"). El `CampingSearchAssistant`:

1. Construye un **system prompt** que incluye el catálogo completo de categorías e instalaciones con sus GUIDs.
2. Envía el prompt + la query del usuario a Gemini.
3. Gemini devuelve un JSON estructurado (`AiSearchFilters`) con los filtros inferidos.
4. El assistant valida y parsea el JSON, construyendo una `SearchCampingsQuery`.
5. La query se despacha por el Mediator al repositorio → SQL dinámico parametrizado → resultados.

**Técnica clave:** *Structured Output / JSON mode* con validación posterior. Si el JSON no es válido, se reintentan hasta 3 veces con retroalimentación del error al modelo.

### Módulo 2 — Recomendaciones Personalizadas (`GET /api/ai/recommendations`)

El `CampingRecommendationAssistant` recibe el historial del usuario (campings favoritos + reservas previas) y el catálogo completo. Gemini genera una lista ordenada de recomendaciones con el motivo de cada una en lenguaje natural.

**Valor añadido:** las recomendaciones son dinámicas y personalizadas; dos usuarios con distintos historiales reciben resultados completamente diferentes.

### Módulo 3 — Comparador Inteligente (`POST /api/ai/compare`)

El `CampingComparisonAssistant` recibe los datos reales de 2–5 campings (nombre, descripción, precio, instalaciones, categorías) y genera un análisis comparativo estructurado: resumen, ganador recomendado y tabla de comparación por dimensión.

**Dato real vs. generación pura:** el LLM recibe los datos reales de la base de datos, no genera información inventada. Esto garantiza la fiabilidad de las comparaciones.

---

## 7. Resultados y evaluación

### Tests

| Proyecto | Tests | Estado |
|---|---|---|
| CampingAI.Domain.Tests | Value Objects, Entidades | ✅ |
| CampingAI.Application.Tests | Handlers, Validators | ✅ |
| CampingAI.Infra.Tests | Mappers | ✅ |
| CampingAI.AI.Tests | NullAIProvider, SearchAssistant, ComparisonAssistant | ✅ |
| CampingAI.DataImporter.Tests | Importador de datos | ✅ |
| **Total** | **253/253** | **✅** |

### Criterios de éxito del MasterPlan

| Criterio | Resultado |
|---|---|
| API REST funcional con autenticación JWT | ✅ Implementado + Swagger documentado |
| Módulo IA de búsqueda semántica operativo | ✅ NL → filtros → SQL → resultados |
| Módulo IA de recomendaciones personalizadas | ✅ Basado en historial real del usuario |
| Módulo IA de comparación inteligente | ✅ Con datos reales de BD |
| App móvil PWA funcional | ✅ Ionic + Angular, compilada para producción |
| App Android generada | ✅ Capacitor (iOS pendiente por requisito de macOS) |
| Mapa interactivo con geolocalización | ✅ Leaflet + clustering + búsqueda por área |
| Panel de gestión para gestores | ✅ Backoffice MVC con flujo de aprobación |
| Suite de tests unitarios | ✅ 253 tests, sin fallos |
| Contenerización Docker | ✅ docker-compose con 3 servicios + healthchecks |

### Calidad del código

- **Nullable habilitado** en todos los proyectos: sin null-reference warnings.
- **Estilo uniforme** (K&R braces, `var`, `_camelCase` para privados).
- **Sin dependencias circulares** entre capas, verificado por convención de proyectos.
- **Sin strings SQL hardcodeados:** todos los nombres de tabla/columna provienen de `nameof()`.
- **Sin secretos en código:** JWT secret, Google ClientId/Secret y API key de Gemini en user-secrets.

---

## 8. Futuras mejoras

| Mejora | Prioridad | Fase relacionada |
|---|---|---|
| **RAG (Retrieval Augmented Generation):** ingestión de guías turísticas, regulaciones de campings y contenido descriptivo en Qdrant; endpoint `/api/ai/ask` para preguntas abiertas con contexto real | Alta | Fase Bonus |
| **Tests de integración:** repositorios Dapper contra SQL Server en contenedor (TestContainers) | Media | Fase 13 extensión |
| **Tests E2E API:** colección Postman/Newman integrada en CI | Media | Fase 13 extensión |
| **Notificaciones push:** confirmación de reserva por email (SendGrid) o push nativa (Capacitor Push) | Media | Nueva fase |
| **App iOS nativa:** requiere entorno macOS + Xcode; la base Capacitor ya está preparada | Media | Fase 09 extensión |
| **Catálogo de categorías con UI:** panel admin para CRUD de categorías/instalaciones | Baja | Fase Extra |
| **Valoraciones y reseñas:** sistema de ratings post-estancia, input adicional para el módulo de recomendaciones | Baja | Nueva fase |
| **CI/CD:** pipeline GitHub Actions (build → test → docker build → push a registry) | Media | DevOps |
| **Internacionalización (i18n):** soporte EN/ES en la app Ionic | Baja | Nueva fase |
| **Caché distribuida:** Redis para los resultados de búsqueda AI y queries frecuentes | Baja | Optimización |
