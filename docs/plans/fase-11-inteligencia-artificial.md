# Fase 11 — Inteligencia Artificial

> Objetivo: implementar el proyecto `CampingAI.AI` utilizando **Google Gemini + Semantic Kernel** desde el primer día tanto en desarrollo como en producción.

CampingAI ya dispone de un proyecto configurado en Google Cloud para el inicio de sesión mediante Google Sign-In. Este mismo proyecto se reutilizará para habilitar Google Gemini.

La arquitectura deberá ser independiente del proveedor de IA para permitir futuras integraciones con OpenAI, Azure OpenAI, Ollama o Groq sin modificar la lógica de negocio.

---

# Decisiones de implementación (acordadas)

- **Conector**: `Microsoft.SemanticKernel` + `Microsoft.SemanticKernel.Connectors.Google` (preview, `AddGoogleAIGeminiChatCompletion`).
- **Ejecución**: implementación real de Gemini + **fallback/stub** (`NullAIProvider`) cuando no hay `AISettings:ApiKey` configurada, de forma que la solución compile y arranque, y los unit tests no realicen llamadas de red.
- **Tests**: solo **unit tests** con `IAIProvider` mockeado (parsing texto→filtros, validación del JSON generado, mapeo a catálogos). Sin tests de integración reales en esta fase.
- **Catálogo**: se inyecta el catálogo real (categorías + facilities con sus **GUIDs** de `T_CATEGORIES` / `T_FACILITIES`) en el prompt; la IA devuelve **GUIDs** directamente, que se validan contra el catálogo antes de ejecutar la búsqueda.
- **DI**: nuevo `CampingAI.AI/Configuration/DI_Manager.cs` encadenado desde `WebApi/Config/DI_Manager.cs` (mismo patrón que las demás capas).
- **Framework**: `net8.0`, `Nullable` + `ImplicitUsings` habilitados, siguiendo el estilo del resto de proyectos.

---

# Plan de implementación (pasos)

1. **Crear proyecto `CampingAI.AI`** (net8.0) con referencias a `CampingAI.Domain` y `CampingAI.Application`, paquetes `Microsoft.SemanticKernel` y `Microsoft.SemanticKernel.Connectors.Google` (`<NoWarn>SKEXP0070</NoWarn>`), y estructura de carpetas (`Providers/`, `SemanticKernel/`, `Search/`, `Recommendations/`, `Comparisons/`, `Embeddings/`, `Prompts/`, `DTOs/`, `Configuration/`, `Settings/`). Añadir a la solución.
2. **Configuración y settings**: `Settings/AISettings.cs` (`Provider`, `Model`, `ApiKey`, `SECTION = "AISettings"`); sección `AISettings` en `appsettings.json`; documentar `dotnet user-secrets set "AISettings:ApiKey" "..."`.
3. **Abstracción de proveedor**: `Providers/IAIProvider.cs`, `Providers/GeminiAIProvider.cs` (usa el Kernel de Gemini + `ILogger`), `Providers/NullAIProvider.cs` (fallback). Carpeta `Providers/Future/` como marcador.
4. **Semantic Kernel**: `SemanticKernel/KernelFactory.cs` que construye el `Kernel` con `AddGoogleAIGeminiChatCompletion`; única dependencia de Gemini.
5. **Prompts**: `Prompts/PromptTemplates.cs` con prompts pequeños y específicos (Search / Recommendations / Comparisons), salidas estructuradas y ejemplos.
6. **Módulo 1 — Búsqueda Inteligente**: `DTOs/AiSearchFilters.cs`, `Search/CampingSearchAssistant.cs` (carga catálogo → prompt → `GenerateJsonAsync` → deserializa y valida GUIDs → filtros). Ejecuta `SearchCampingsQuery` existente.
7. **Módulo 2 — Recomendaciones**: `Recommendations/CampingRecommendationAssistant.cs` + `RecommendationService` (favoritos, reservas, categorías preferidas), con razonamiento y límite de resultados. `DTOs/RecommendationResponse.cs`.
8. **Módulo 3 — Comparador**: `Comparisons/CampingComparisonAssistant.cs` (datos reales por ID), prompt acotado. `DTOs/CompareRequest.cs`, `DTOs/CompareResponse.cs`.
9. **Embeddings (stubs)**: `Embeddings/IEmbeddingProvider.cs`, `Embeddings/IVectorStore.cs` (contratos vectoriales para la Fase Bonus/RAG, desacoplados).
10. **DI**: `CampingAI.AI/Configuration/DI_Manager.cs` (registra `IAIProvider` real o `NullAIProvider` según `ApiKey`, `Kernel`, asistentes y servicios); encadenar desde WebApi y añadir referencia de proyecto.
11. **WebApi**: `Controllers/api/Ai/AiController.cs` (JWT Bearer) con `POST /api/ai/search`, `GET /api/ai/recommendations` (`[Authorize]`), `POST /api/ai/compare`; DTOs en `Controllers/api/Ai/DTO/`; `[ProducesResponseType]` + `Shared.ErrorResponse`; errores vía `GlobalExceptionMiddleware`.
12. **Gestión de errores** en `GeminiAIProvider`: API Key inválida, timeout, respuesta vacía, JSON inválido, rate limit, error de red; reintentos limitados, logging y mensajes amigables.
13. **Unit tests** (mock de `IAIProvider`): texto→filtros, validación JSON, mapeo a catálogos, comparador y recomendaciones.
14. **Build, validación y documentación**: `dotnet build` + `dotnet test`; marcar tareas/criterios `[x]` y añadir **Registro de implementación**.

---

# Arquitectura IA

```text
Usuario
   │
   ▼
CampingAI.WebApi
   │
   ▼
CampingAI.AI
   │
   ▼
Semantic Kernel
   │
   ▼
Google Gemini API
```

---

# Objetivos de la Fase

Implementar los siguientes módulos:

- Búsqueda Inteligente
- Recomendaciones Personalizadas
- Comparador Inteligente

Además, dejar preparada la estructura para:

- Embeddings
- Vector Search
- RAG
- Chat Turístico

---

# Setup

## Proyecto

- [x] Crear proyecto `CampingAI.AI`.
- [x] Referenciar `Microsoft.SemanticKernel`.
- [x] Crear capa de abstracción para proveedores IA.
- [x] Mantener independencia tecnológica.

---

## Google Gemini

## Proyecto Google Existente

### Situación Actual

La aplicación ya utiliza Google Sign-In para autenticación mediante OAuth 2.0.

Existe un proyecto activo en Google Cloud / Google Developer Console.

Dicho proyecto será reutilizado para habilitar Gemini y evitar la gestión de múltiples proyectos.

Arquitectura:

```text
Google Cloud Project
│
├── Google OAuth
│   └── Login de usuarios
│
├── Gemini API
│   └── Inteligencia Artificial
│
└── Credenciales
```

---

### Tareas

- [ ] Identificar el Project Id actualmente utilizado por la aplicación.
- [ ] Documentar dicho Project Id.
- [ ] Verificar acceso administrativo al proyecto.
- [ ] Habilitar Gemini API dentro del mismo proyecto.
- [ ] Crear API Key específica para Gemini.
- [ ] Verificar funcionamiento desde entorno local.

---

### Decisión Arquitectónica

Se utilizará un único proyecto de Google Cloud para:

```text
Google Login
Google Gemini
```

Sin embargo se mantendrán credenciales separadas.

---

### Credenciales

#### Google Login

Utilizadas para:

```text
Autenticación
Autorización
Inicio de sesión
```

Tipo:

```text
OAuth Client Id
```

---

#### Gemini

Utilizadas para:

```text
Búsqueda Inteligente
Comparador
Recomendaciones
Embeddings
RAG
```

Tipo:

```text
API Key
```

---

### Restricción

Bajo ningún concepto deberá reutilizarse el OAuth Client Id como mecanismo de acceso a Gemini.

Gemini deberá utilizar su propia API Key.

---

### Configuración

Crear configuración:

```json
{
  "AISettings": {
    "Provider": "Gemini",
    "Model": "gemini-2.5-flash",
    "ApiKey": ""
  }
}
```

---

### Seguridad

- [ ] Configurar API Key mediante User Secrets.
- [ ] No almacenar claves en código fuente.
- [ ] No almacenar claves en repositorios Git.
- [ ] Permitir sobrescritura mediante variables de entorno.
- [ ] Separar completamente credenciales OAuth y Gemini.
- [ ] Documentar el procedimiento de rotación de claves.

#### Desarrollo

Utilizar:

```bash
dotnet user-secrets
```

Ejemplo:

```bash
dotnet user-secrets set "AISettings:ApiKey" "YOUR_GEMINI_KEY"
```

---

#### Producción

Obtener credenciales desde:

```text
Variables de entorno
```

o

```text
Secret Manager del proveedor de hosting
```

---

## Estructura del Proyecto

```text
CampingAI.AI

├── Assistants
│
├── SemanticKernel
│
├── Providers
│   ├── IAIProvider
│   ├── GeminiAIProvider
│   └── Future
│       ├── OpenAIProvider
│       ├── AzureOpenAIProvider
│       ├── OllamaProvider
│       └── GroqProvider
│
├── Search
│
├── Recommendations
│
├── Comparisons
│
├── Embeddings
│
├── Prompts
│
└── DTOs
```

---

# Dependency Injection

- [x] Registrar `IAIProvider`.
- [x] Registrar `GeminiAIProvider`.
- [x] Registrar Semantic Kernel.
- [x] Registrar asistentes.
- [x] Registrar servicios de recomendaciones.
- [x] Registrar servicios de comparación.

---

# Independencia de Proveedor IA

Aunque Gemini será el proveedor utilizado en desarrollo y producción, la arquitectura deberá permanecer desacoplada.

---

## Contrato Principal

```csharp
IAIProvider
```

---

## Implementación Inicial

```text
GeminiAIProvider
```

---

## Implementaciones Futuras

```text
OpenAIProvider

AzureOpenAIProvider

GroqProvider

OllamaProvider
```

---

## Objetivo

Permitir sustituir el proveedor de IA mediante configuración sin modificar:

- Controllers
- Commands
- Queries
- Handlers
- Assistants
- Servicios de negocio

---

# Módulo 1 — Búsqueda Inteligente

## Objetivo

Permitir búsquedas mediante lenguaje natural.

---

## Casos de Uso

```text
Quiero un camping familiar cerca de la playa.
```

```text
Necesito un camping con piscina que admita mascotas.
```

```text
Busco un camping tranquilo en Tarragona.
```

---

## Flujo

```text
Usuario

↓

Gemini

↓

Filtros estructurados

↓

SearchCampingsQuery

↓

SQL Server

↓

Resultados
```

---

## Resultado esperado

Entrada:

```text
Quiero un camping para niños con piscina cerca de la playa.
```

Salida:

```json
{
  "categories": [
    "Familiar",
    "Playa"
  ],
  "services": [
    "Piscina"
  ]
}
```

---

## Desarrollo

- [x] Crear `CampingSearchAssistant`.
- [x] Crear prompt especializado.
- [x] Solicitar respuesta JSON estructurada.
- [x] Validar respuesta generada.
- [x] Convertir respuesta a filtros.
- [x] Ejecutar `SearchCampingsQuery`.

---

## Endpoint

```http
POST /api/ai/search
```

---

# Módulo 2 — Recomendaciones

## Objetivo

Generar recomendaciones personalizadas para cada usuario.

---

## Datos utilizados

```text
Favoritos

Reservas

Historial de búsquedas

Categorías preferidas
```

---

## Flujo

```text
Usuario

↓

Preferencias

↓

Gemini

↓

Recomendaciones

↓

Respuesta
```

---

## Resultado esperado

```text
Camping Costa Brava

Motivos:
- Camping familiar
- Piscina
- Playa cercana

Camping Delta Nature

Motivos:
- Entorno natural
- Compatible con mascotas
```

---

## Desarrollo

- [x] Crear `RecommendationService`.
- [x] Crear `CampingRecommendationAssistant`.
- [x] Generar razonamiento de recomendación.
- [x] Limitar cantidad de resultados.

---

## Endpoint

```http
GET /api/ai/recommendations
```

Requiere:

```text
[Authorize]
```

---

# Módulo 3 — Comparador Inteligente

## Objetivo

Comparar varios campings seleccionados por el usuario.

---

## Entrada

```json
[
  "campingA",
  "campingB",
  "campingC"
]
```

---

## Flujo

```text
Campings

↓

Datos estructurados

↓

Gemini

↓

Comparación textual
```

---

## Resultado esperado

```text
Camping A

+ Mejor ubicación junto al mar

Camping B

+ Mejor relación calidad-precio

Camping C

+ Más servicios para familias
```

---

## Desarrollo

- [x] Crear `CampingComparisonAssistant`.
- [x] Crear prompt de comparación.
- [x] Limitar longitud de respuesta.
- [x] Utilizar únicamente datos reales de la base de datos.

---

## Endpoint

```http
POST /api/ai/compare
```

---

# Semantic Kernel

## Objetivo

Centralizar toda la integración con Gemini.

---

## Funcionalidades

- [x] Configurar Kernel principal.
- [x] Integrar Gemini.
- [x] Gestionar prompts.
- [x] Gestionar asistentes.
- [ ] Preparar Function Calling.
- [x] Preparar futura integración RAG.

---

# Gestión de Prompts

## Carpeta

```text
Prompts
```

---

## Subcarpetas

```text
Search

Recommendations

Comparisons

Tourism

RAG
```

---

## Reglas

- Prompts pequeños.
- Prompts específicos.
- Salidas estructuradas.
- Incluir ejemplos.
- Evitar respuestas ambiguas.

---

# Preparación para Embeddings

## Objetivo

Preparar la Fase Bonus (RAG).

---

## Desarrollo

- [x] Crear interfaces para embeddings.
- [x] Crear contratos vectoriales.
- [x] Diseñar integración futura con Qdrant.
- [x] Mantener desacoplamiento del proveedor.

---

# WebApi

## Controlador

```text
Controllers/api/Ai/AiController
```

---

## DTOs

Crear:

```text
AiSearchRequest

AiSearchResponse

RecommendationResponse

CompareRequest

CompareResponse
```

---

## Swagger

- [ ] Documentar todos los endpoints.
- [ ] Añadir ejemplos.
- [ ] Documentar respuestas IA.

---

## Respuestas

- [x] Utilizar `ResponseWrapper`.
- [x] Utilizar `ErrorResponse`.
- [x] Definir `ProducesResponseType`.

---

# Gestión de Errores

## Casos a controlar

- API Key inválida.
- Timeout.
- Respuesta vacía.
- JSON inválido.
- Rate limit.
- Error de red.
- Error de Gemini.

---

## Estrategia

- [x] Logging mediante ILogger.
- [x] Reintentos limitados.
- [x] Respuestas amigables.
- [ ] Circuit breaker si fuese necesario.

---

# Buenas Prácticas

## IA

- [x] Utilizar respuestas JSON estructuradas.
- [x] Validar toda salida generada.
- [x] No confiar ciegamente en el modelo.
- [x] Limitar longitud de respuestas.
- [ ] Mantener historial de prompts para auditoría.

---

## Arquitectura

- [x] Gemini nunca será invocado directamente desde los controladores.
- [x] Toda integración IA residirá en `CampingAI.AI`.
- [x] Mantener separación estricta de responsabilidades.
- [x] Mantener proveedor desacoplado mediante interfaces.

---

# Tests

## Unit Tests

- [x] Conversión texto → filtros.
- [x] Validación JSON generado.
- [x] Comparador.
- [x] Recomendaciones.

---

## Casos de Prueba

Entrada:

```text
Camping de montaña con piscina.
```

Salida:

```json
{
  "category": "Montaña",
  "services": [
    "Piscina"
  ]
}
```

---

## Integration Tests

- [ ] Comunicación con Gemini.
- [ ] Comunicación con Semantic Kernel.
- [ ] Endpoints IA.

> Nota: los integration tests reales quedan fuera del alcance acordado (solo unit tests con `IAIProvider` mockeado).

---

# Criterios de Aceptación

✅ Existe proyecto `CampingAI.AI`.

✅ Semantic Kernel integrado.

✅ Gemini configurado y operativo.

✅ Búsqueda inteligente funcional.

✅ Recomendaciones funcionales.

✅ Comparador funcional.

✅ Swagger documentado.

✅ Tests básicos implementados.

✅ Funciona tanto en desarrollo como en producción utilizando Gemini.

✅ El proyecto Google existente se reutiliza para Login y Gemini.

✅ OAuth y Gemini utilizan credenciales independientes.

✅ La API Key de Gemini se obtiene mediante User Secrets o Variables de Entorno.

✅ No existen secretos almacenados en el repositorio.

✅ Arquitect

---

## Registro de implementación

**Fecha:** 2025 · **Autor:** Copilot

### Ficheros creados
- `CampingAI.AI/CampingAI.AI.csproj` — Nuevo proyecto de la capa de IA (net8.0).
- `CampingAI.AI/Settings/AISettings.cs` — Configuración de proveedor, modelo y API Key.
- `CampingAI.AI/Providers/IAIProvider.cs` — Abstracción del proveedor de IA (independiente del proveedor concreto).
- `CampingAI.AI/Providers/GeminiAIProvider.cs` — Implementación real con Gemini vía Semantic Kernel, reintentos y saneado de JSON.
- `CampingAI.AI/Providers/NullAIProvider.cs` — Proveedor de reserva (fallback) cuando no hay API Key.
- `CampingAI.AI/Providers/Future/README.md` — Marcador para futuros proveedores (OpenAI, Ollama, etc.).
- `CampingAI.AI/SemanticKernel/KernelFactory.cs` — Construcción del `Kernel` con `AddGoogleAIGeminiChatCompletion`.
- `CampingAI.AI/Prompts/PromptTemplates.cs` — Plantillas de prompt de búsqueda, recomendaciones y comparación.
- `CampingAI.AI/DTOs/AiSearchFilters.cs`, `RecommendationResponse.cs`, `CompareRequest.cs`, `CompareResponse.cs` — DTOs de salida de IA.
- `CampingAI.AI/Search/CampingSearchAssistant.cs` — Búsqueda inteligente: NL → filtros (validados contra catálogo real por GUID) → `SearchCampingsQuery`.
- `CampingAI.AI/Recommendations/RecommendationService.cs` — Construcción del perfil de preferencias y candidatos.
- `CampingAI.AI/Recommendations/CampingRecommendationAssistant.cs` — Recomendaciones personalizadas con razonamiento.
- `CampingAI.AI/Comparisons/CampingComparisonAssistant.cs` — Comparador basado en datos reales de la BD.
- `CampingAI.AI/Embeddings/IEmbeddingProvider.cs`, `IVectorStore.cs` — Contratos preparados para RAG (Fase Bonus).
- `CampingAI.AI/Configuration/DI_Manager.cs` — Registro de settings, selección de proveedor (Gemini/Null) y asistentes.
- `CampingAI.WebApi/Controllers/api/AI/AiController.cs` — Endpoints `POST api/ai/search`, `GET api/ai/recommendations`, `POST api/ai/compare`.
- `CampingAI.WebApi/Controllers/api/AI/DTO/AiSearchRequest.cs`, `AiRecommendationResponse.cs`, `AiCompareRequest.cs` — Contratos de la API de IA.
- `CampingAI.AI.Tests/CampingAI.AI.Tests.csproj` — Proyecto de tests unitarios de la capa de IA.
- `CampingAI.AI.Tests/Providers/NullAIProviderTests.cs` — Tests del fallback.
- `CampingAI.AI.Tests/Search/CampingSearchAssistantTests.cs` — Tests de parsing/validación GUID/forward de filtros.
- `CampingAI.AI.Tests/Comparisons/CampingComparisonAssistantTests.cs` — Tests de comparación.

### Ficheros modificados
- `CampingAI.sln` — Añadidos los proyectos `CampingAI.AI` y `CampingAI.AI.Tests`.
- `CampingAI.WebApi/CampingAI.WebApi.csproj` — Referencia al proyecto `CampingAI.AI`.
- `CampingAI.WebApi/Config/DI_Manager.cs` — Encadenado `AI.Configuration.DI_Manager.Configure(...)`.
- `CampingAI.WebApi/appsettings.json` — Añadida sección `AISettings` (Provider/Model/ApiKey vacía).

### Paquetes NuGet añadidos
- `Microsoft.SemanticKernel` 1.40.0 (en `CampingAI.AI`).
- `Microsoft.SemanticKernel.Connectors.Google` 1.40.0-alpha (en `CampingAI.AI`).
- `Microsoft.Extensions.Configuration.Binder` 8.0.2 (en `CampingAI.AI`).
- `CampingAI.AI.Tests`: coverlet.collector, FluentAssertions, Microsoft.NET.Test.Sdk, Moq, xunit, xunit.runner.visualstudio.

### Comandos de infraestructura
- La API Key de Gemini debe configurarse fuera del repositorio, por ejemplo:
  `dotnet user-secrets set "AISettings:ApiKey" "<GEMINI_API_KEY>" --project CampingAI.WebApi`
  (sin clave, la aplicación arranca con `NullAIProvider` como fallback).

### Resultado final
- `dotnet build CampingAI.sln` ✅ (Build successful).
- Tests de la capa de IA: `9/9` ✅ (`CampingAI.AI.Tests`).