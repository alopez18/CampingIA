---
marp: true
theme: default
paginate: true
size: 16:9
header: 'CampingAI — Trabajo Fin de Máster'
footer: 'Aitor López Palomares · Máster en IA Aplicada'
style: |
  section {
    font-size: 26px;
  }
  h1 {
    color: #28a745;
  }
  h2 {
    color: #004085;
  }
  table {
    font-size: 20px;
  }
  section.lead {
    text-align: center;
  }
  section.lead h1 {
    font-size: 54px;
  }
  code {
    background: #f4f4f4;
  }
  .cols {
    display: flex;
    align-items: center;
    gap: 40px;
    margin-top: 10px;
  }
  .cols .imgs {
    display: flex;
    gap: 14px;
    flex: 0 0 auto;
  }
  .cols .imgs img {
    height: 470px;
    width: auto;
  }
  .cols .txt {
    flex: 1;
    font-size: 24px;
  }
  .cols .txt ul {
    margin: 0;
    padding-left: 22px;
  }
  .cols.dense {
    align-items: flex-start;
  }
  .cols.dense .txt {
    font-size: 20px;
  }
---

<!--
=====================================================================
 Presentación Marp — CampingAI
 Cómo usar:
   1. Instala la extensión "Marp for VS Code".
   2. Abre este archivo y pulsa el icono de vista previa (Marp).
   3. Exporta con la paleta de comandos: "Marp: Export Slide Deck..."
      -> PDF / PPTX / HTML.
 Nota: los diagramas Mermaid completos están en docs/diagrams/diagramas.md.
       Para incluirlos como imagen, expórtalos en mermaid.live y sustituye
       los bloques marcados con "[Diagrama: ...]" por la imagen (![](...)).
=====================================================================
-->

<!-- _class: lead -->

# CampingAI

## Plataforma de gestión y descubrimiento de campings con IA

**Aitor López Palomares**
Máster en Inteligencia Artificial Aplicada
Trabajo Fin de Máster · 2026

---

# 1. Problema y motivación

El turismo de camping en España mueve **millones de pernoctaciones al año**, pero la experiencia digital sigue siendo deficiente.

**Problemas identificados:**

- 🔍 Búsqueda basada en coincidencia exacta de filtros discretos
- 🤖 Ausencia de recomendaciones personalizadas
- ⚖️ Falta de comparación inteligente entre establecimientos
- 📱 Plataformas no diseñadas para móvil (sin PWA/app)
- 🏕️ Gestión de campings aislada, sin backoffice para gestores

> Un turista que busca *"un camping tranquilo con piscina cerca del mar para familia con niños"* debe marcar decenas de checkboxes.

---

# Motivación

Aplicar las técnicas de **Inteligencia Artificial** estudiadas en el máster:

- **LLMs** (Large Language Models)
- **Prompt Engineering**
- **Generación de texto estructurado** (JSON mode)

…para transformar la experiencia de búsqueda y descubrimiento de campings, demostrando que la IA **mejora de forma medible** la usabilidad de una plataforma real.

---

# 2. Objetivos

**Objetivo principal:** desarrollar una plataforma full-stack de gestión y descubrimiento de campings con módulos de IA integrados, aplicando LLMs en el ciclo completo de una aplicación empresarial.

| # | Objetivo secundario | ✅ |
|---|---|---|
| O1 | Clean Architecture + CQRS en .NET 8 | ✅ |
| O2 | API REST con JWT + OAuth (Google) | ✅ |
| O3 | IA: búsqueda semántica (NL → SQL) | ✅ |
| O4 | IA: recomendaciones personalizadas | ✅ |
| O5 | IA: comparación inteligente | ✅ |
| O6 | App móvil PWA (Ionic/Angular) | ✅ |
| O7 | Mapa interactivo + geolocalización | ✅ |
| O8 | Backoffice web para gestores | ✅ |

---

# La aplicación en acción

![w:470](screenshots/app-login.png) ![w:470](screenshots/app-home-recommendations.png)

App móvil PWA (Ionic + Angular 20) — login y home con recomendaciones IA

---

# Detalle de camping y mapa

![w:470](screenshots/app-camping-detail.png) ![w:470](screenshots/app-map.png)

Ficha con mapa Leaflet, categorías e instalaciones · Mapa interactivo con OpenStreetMap

---

# Reserva de campings

![w:440](screenshots/app-reservation-new.png) ![w:440](screenshots/app-reservations-list.png)

El usuario reserva un camping seleccionando **check-in / check-out**; el sistema calcula noches y precio total.

- Validación de fechas (`ReservationDateVO`: CheckIn < CheckOut)
- Cálculo automático del importe (nº noches × precio/noche)
- Estados de reserva (`Pending`, confirmada, cancelada) y **cancelación** desde el listado

---

<!-- _class: lead -->

# Versión móvil

## App nativa Android (Ionic + Angular 20 · Capacitor)

Capturas reales sobre emulador **Pixel 10 Pro**

---

# App móvil — Acceso y descubrimiento

<div class="cols">
<div class="imgs">
<img src="screenshots/mobile/app-login.png" />
<img src="screenshots/mobile/app-home-recommendations.png" />
<img src="screenshots/mobile/app-campings-list.png" />
</div>
<div class="txt">

- **Login** con email/contraseña y Google OAuth
- **Home** con recomendaciones personalizadas por IA
- **Listado** de campings con buscador integrado

</div>
</div>

---

# App móvil — Búsqueda avanzada

<div class="cols">
<div class="imgs">
<img src="screenshots/mobile/app-advanced-search.png" />
</div>
<div class="txt">

Además del buscador por nombre, al abrir los **filtros** se accede a la **búsqueda avanzada**, que acota los resultados por:

- **Provincia** y **Categoría**
- **Precio mínimo / máximo** (€)
- **Instalaciones** disponibles

Los filtros se combinan y se aplican con **"Aplicar filtros"** (o se reinician con **"Limpiar"**).

</div>
</div>

---

# App móvil — Mapa y búsqueda con IA

<div class="cols">
<div class="imgs">
<img src="screenshots/mobile/app-camping-detail.png" />
<img src="screenshots/mobile/app-map.png" />
<img src="screenshots/mobile/app-ai-search.png" />
</div>
<div class="txt">

- **Ficha** de camping con mini-mapa
- **Mapa** con *clustering* sobre OpenStreetMap
- **Búsqueda inteligente** en lenguaje natural

</div>
</div>

---

# App móvil — Reservas y perfil

<div class="cols">
<div class="imgs">
<img src="screenshots/mobile/app-reservation-new.png" />
<img src="screenshots/mobile/app-reservations-list.png" />
<img src="screenshots/mobile/app-profile.png" />
</div>
<div class="txt">

- **Nueva reserva** (check-in / check-out)
- **Mis reservas** con estado y precio
- **Perfil** de usuario

</div>
</div>

---

# Distribución de la app móvil

<div class="cols">
<div class="imgs">
<img src="screenshots/mobile/backoffice-apk-download.png" />
</div>
<div class="txt">

Al acceder al **backoffice desde un dispositivo móvil**, la web detecta el *User-Agent* Android y muestra un botón **"Descargar app Android"** que sirve el **APK** directamente.

- Detección de plataforma en servidor (`User-Agent` → Android)
- Descarga directa del APK desde el backoffice (`Home/DownloadApk`)
- En escritorio, el mismo botón redirige a la **web/PWA**

</div>
</div>

---

# Backoffice de gestión

![w:520](screenshots/backoffice-home.png) ![w:520](screenshots/backoffice-login.png)

Portal web (MVC Razor) para gestores y administradores · Autenticación Cookie + Google OAuth

---

# Backoffice — Administrador

![w:470](screenshots/admin-dashboard.png) ![w:470](screenshots/admin-campings.png)

Panel de administrador con **gestión global**:

- Aprobación de **gestores pendientes**
- Gestión de **todos los campings** (1.600+) con búsqueda y CRUD
- Administración de **usuarios y roles**

---

# Backoffice — Usuarios y edición (Admin)

![w:470](screenshots/admin-users.png) ![w:470](screenshots/admin-camping-edit.png)

Gestión de usuarios por rol (Admin / Gestor / Común / Sistema) · Edición completa de camping (categorías, provincia, instalaciones)

---

# Backoffice — Gestor

![w:470](screenshots/manager-dashboard.png) ![w:470](screenshots/manager-my-campings.png)

El **gestor** administra únicamente **sus propios campings**:

- Panel con accesos a "Mis campings" y "Nuevo camping"
- Listado propio con acciones Editar / Borrar
- Registro sujeto a **aprobación previa** del administrador

---

# Backoffice — Alta de camping (Gestor)

![w:430](screenshots/manager-create-camping.png)

Formulario de creación: nombre, descripción, coordenadas, precio, categoría principal, categorías secundarias, provincia e instalaciones.

---

# 3. Arquitectura — Clean Architecture

Basada en la **Clean Architecture** de Robert C. Martin. Las dependencias fluyen siempre hacia el interior:

```
WebApi ──► Application ──► Domain
WebApi ──► AI          ──► Application
Infra  ──► Domain
```

- **Domain** — entidades, value objects, interfaces. Sin dependencias externas
- **Application** — handlers CQRS, DTOs, validadores, servicios
- **Infra** — Dapper, Unit of Work, mappers de BD
- **AI** — providers, assistants, Semantic Kernel
- **WebApi** — controladores REST + MVC, DI, middleware

**Decisión clave:** cada capa expone un `DI_Manager.Configure(services)` encadenado.

---

# 3.2 CQRS con Mediator propio

Implementado **sin MediatR**, con abstracciones propias:

- `ICommand` / `ICommandHandler<TCommand>` — escritura
- `IQuery<TResult>` / `IQueryHandler<TQuery,TResult>` — lectura
- `IMediator` — dispatcher que resuelve handlers desde el contenedor DI

**Flujo de escritura:** `Controller → IMediator → Handler → Repository → UnitOfWork → SQL Server`

> **Motivación:** aprendizaje profundo del patrón, control total del pipeline, cero dependencias de terceros en el núcleo.

---

# 3.3–3.4 Datos y Dominio

## Acceso a datos con Dapper

- Control total del SQL (clave para filtros dinámicos de búsqueda)
- SQL construido con `StringBuilder` + `nameof()` → sin strings hardcodeados
- Patrón **Unit of Work** para transacciones de escritura

## Value Objects

Campos con semántica propia validados en su constructor → `DomainException` si el valor es inválido. **Imposible crear entidades en estado inconsistente.**

`EmailVO` · `LatitudeVO` (−90/+90) · `LongitudeVO` (−180/+180) · `PriceVO` (≥0) · `ReservationDateVO` (CheckIn < CheckOut)

---

# 3.5 Autenticación dual

| Mecanismo | Uso |
|---|---|
| **JWT Bearer** | API REST consumida por la app Ionic |
| **Cookie + Google OAuth** | Acceso web al backoffice (gestores/admin) |

**Flujo de aprobación:** los gestores registrados quedan en estado `Pending` hasta que un administrador los aprueba desde el panel `/Admin`.

> Todos los secretos (JWT, Google ClientId/Secret, API key Gemini) se gestionan con **user-secrets**, nunca en código.

---

# 4. Tecnologías — Backend

| Tecnología | Versión | Uso |
|---|---|---|
| .NET / ASP.NET Core | 8.0 | Framework principal |
| C# | 12 | Lenguaje |
| Dapper | 2.x | ORM ligero |
| SQL Server | 2022 | Base de datos relacional |
| FluentValidation | 11.x | Validación commands/queries |
| Microsoft Semantic Kernel | 1.x | Orquestación de IA |
| Google Gemini (2.5 Flash) | API | LLM |
| Swagger | 6.x | Documentación API |
| xUnit + FluentAssertions + Moq | — | Testing |

---

# 4. Tecnologías — Frontend y DevOps

## Frontend (App Móvil)

| Tecnología | Uso |
|---|---|
| Ionic 8 + Angular 20 (standalone) | UI móvil + SPA |
| Capacitor 6 | Acceso a APIs nativas |
| Leaflet + markercluster | Mapa interactivo |
| OpenStreetMap | Tiles cartográficos (libre) |

## DevOps

**Docker + Docker Compose** (contenerización completa) · **Nginx** (reverse proxy PWA) · **GitHub** (control de versiones)

---

# 5. Proceso de desarrollo por fases

| Fase | Nombre | Fase | Nombre |
|---|---|---|---|
| 01 | Configuración inicial | 08 | Reservas |
| 02 | Dominio | 09 | App Ionic |
| 03 | Persistencia | 10 | Mapa interactivo |
| 04 | Gestión de usuarios | **11** | **Inteligencia Artificial** |
| 04-B | Usuarios gestores | 13 | Testing |
| 05 | Gestión de campings | 14 | Dockerización |
| 06 | Búsqueda avanzada | 15 | Documentación TFM |
| 07 | Favoritos | | |

**15 fases completadas** ✅ · Fase Bonus (RAG) pendiente

---

<!-- _class: lead -->

# 6. IA aplicada

## El núcleo del proyecto

---

# Arquitectura de la capa de IA

La capa `CampingAI.AI` se desacopla mediante la abstracción `IAIProvider`:

```
IAIProvider
  ├── GeminiAIProvider   (producción: Google Gemini 2.5 Flash)
  ├── CachingAIProvider  (decorador: caché en memoria)
  └── NullAIProvider     (fallback / tests: respuestas controladas)
```

- `KernelFactory` configura **Microsoft Semantic Kernel** con el conector de Google Gemini
- La API key se gestiona con user-secrets (`AISettings:ApiKey`)
- Patrón **Decorator** para caché · **Null Object** para tests

---

# Módulo 1 — Búsqueda Inteligente

<div class="cols dense">
<div class="txt">

`POST /api/ai/search` — el usuario escribe en lenguaje natural.

**Ejemplo:** *"camping familiar con piscina en Cataluña bajo 50€ la noche"*

El `CampingSearchAssistant`:

1. Construye un **system prompt** con el catálogo (categorías e instalaciones con GUIDs)
2. Envía prompt + query a Gemini
3. Gemini devuelve **JSON estructurado** (`AiSearchFilters`)
4. Valida y parsea → `SearchCampingsQuery`
5. Despacha por Mediator → SQL dinámico → resultados

> **Técnica clave:** *Structured Output / JSON mode* con reintento (hasta 3×) y retroalimentación del error al modelo.

</div>
<div class="imgs">
<img src="screenshots/app-ai-search.png" style="width:520px;height:auto;" />
</div>
</div>

---

# Módulo 2 — Recomendaciones personalizadas

<div class="cols">
<div class="txt">

`GET /api/ai/recommendations`

El `CampingRecommendationAssistant` recibe:

- El **historial del usuario** (favoritos + reservas previas)
- El **catálogo completo** de campings

Gemini genera una **lista ordenada** de recomendaciones, cada una con su motivo en lenguaje natural.

> **Valor añadido:** las recomendaciones son dinámicas y personalizadas — dos usuarios con distintos historiales reciben resultados completamente diferentes.

</div>
<div class="imgs">
<img src="screenshots/app-home-recommendations.png" style="width:520px;height:auto;" />
</div>
</div>

---

# Módulo 3 — Comparador inteligente

<div class="cols dense">
<div class="txt">

`POST /api/ai/compare`

El `CampingComparisonAssistant` recibe los **datos reales** de 2–5 campings (nombre, descripción, precio, instalaciones, categorías) y genera:

- 📝 Un resumen comparativo
- 🏆 Un ganador recomendado
- 📊 Una tabla de comparación por dimensión

Accesible desde **Favoritos** en la app móvil: se seleccionan 2+ campings y se lanza la comparación.

> **Dato real vs. generación pura:** el LLM recibe los datos reales de la base de datos, **no inventa información**. Esto garantiza la fiabilidad de las comparaciones.

</div>
<div class="imgs">
<img src="screenshots/mobile/app-ai-compare.png" />
</div>
</div>

---

# 7. Resultados — Testing

| Proyecto | Cobertura |
|---|---|
| CampingAI.Domain.Tests | Value Objects, Entidades |
| CampingAI.Application.Tests | Handlers, Validators |
| CampingAI.Infra.Tests | Mappers |
| CampingAI.AI.Tests | Providers y Assistants de IA |
| CampingAI.DataImporter.Tests | Importador de datos |

## ✅ 253 / 253 tests — sin fallos

---

# 7. Calidad del código

- ✅ **Nullable habilitado** en todos los proyectos → sin null-reference warnings
- ✅ **Estilo uniforme** (K&R braces, `var`, `_camelCase` para privados)
- ✅ **Sin dependencias circulares** entre capas
- ✅ **Sin strings SQL hardcodeados** → nombres vía `nameof()`
- ✅ **Sin secretos en código** → user-secrets
- ✅ **Contenerización completa** → docker-compose con 3 servicios + healthchecks

**Todos los criterios de éxito del MasterPlan cumplidos.**

---

# 8. Futuras mejoras

| Mejora | Prioridad |
|---|---|
| **RAG** — ingestión de guías/regulaciones en Qdrant + `/api/ai/ask` | Alta |
| Tests de integración (TestContainers) | Media |
| Tests E2E API (Postman/Newman en CI) | Media |
| Notificaciones push (SendGrid / Capacitor Push) | Media |
| App iOS nativa (base Capacitor ya lista) | Media |
| CI/CD (GitHub Actions: build → test → docker) | Media |
| Valoraciones y reseñas post-estancia | Baja |
| Internacionalización (i18n EN/ES) | Baja |
| Caché distribuida (Redis) | Baja |

---

# Entornos y credenciales de prueba

Para que el revisor pueda probar la plataforma en vivo:

## 🖥️ Backoffice — [campingaibackoffice.runasp.net](https://campingaibackoffice.runasp.net/)

| Rol | Usuario | Contraseña |
|---|---|---|
| Administrador | `admin@campingai.local` | `C@mpingsAI!` |
| Gestor | `gestor@campingai.local` | `123456` |

## 📱 Front (PWA) — [campingai.runasp.net](https://campingai.runasp.net/)

| Rol | Usuario | Contraseña |
|---|---|---|
| Cliente | `braism@campingai.local` | `Br@isM0ur€!` |

---

<!-- _class: lead -->

# Conclusiones

CampingAI demuestra que la **IA generativa (LLMs)** puede integrarse de forma **práctica y fiable** en el ciclo completo de una aplicación empresarial real, sobre una base de **Clean Architecture** mantenible y testeada.

**3 módulos de IA · 253 tests · Full-stack contenerizado**

---

<!-- _class: lead -->

# ¡Gracias!

**Aitor López Palomares**
github.com/alopez18/CampingIA
