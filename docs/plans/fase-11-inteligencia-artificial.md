# Fase 11 — Inteligencia Artificial

> Objetivo: proyecto `CampingAI.AI` con búsqueda inteligente, recomendaciones y comparador.

## Setup
- [ ] Crear proyecto `CampingAI.AI` (dependencias: Semantic Kernel, cliente OpenAI/Azure OpenAI).
- [ ] Configurar claves desde configuración/user-secrets (no hardcodear).
- [ ] Estructura: `Assistants`, `SemanticKernel`, `Embeddings`, `Recommendations`, `Prompts`.
- [ ] Registrar servicios IA en DI (respetando dirección de dependencias).

## Módulo 1 — Búsqueda Inteligente
- [ ] Servicio que convierte lenguaje natural → filtros de `SearchCampingsQuery`.
  - Flujo: Usuario → IA (function calling / structured output) → filtros → `SearchCampingsQuery` → resultado.
- [ ] Endpoint `POST /api/ai/search`.

## Módulo 2 — Recomendaciones
- [ ] Servicio de recomendación basado en favoritos + reservas + búsquedas.
- [ ] Endpoint `GET /api/ai/recommendations` (`[Authorize]`).

## Módulo 3 — Comparador
- [ ] Servicio que compara campings (características, precio, servicios).
- [ ] Endpoint `POST /api/ai/compare`.

## WebApi
- [ ] DTOs + controlador `AiController` bajo `Controllers/api/Ai`.
- [ ] `[ProducesResponseType(...)]` + `ErrorResponse`.

## Best practices
- [ ] Aplicar Azure/AI best practices (agents, prompts, manejo de errores/límites).

## Tests
- [ ] Tests de conversión NL→filtros con entradas representativas.

## Criterio de aceptación
- Búsqueda en lenguaje natural devuelve resultados coherentes; recomendaciones y comparador operativos.
