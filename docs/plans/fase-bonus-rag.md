# Fase Bonus — RAG (Retrieval Augmented Generation)

> **Estado: OPCIONAL / BONUS.** Esta fase se abordará **únicamente si el tiempo lo permite**, tras completar las fases 13 (Testing), 14 (Dockerización) y 15 (Documentación TFM). No forma parte del alcance obligatorio del TFM.

> Objetivo: responder preguntas usando conocimiento contextual con Semantic Kernel + Embeddings de **Google Gemini** + Qdrant.

> **Decisión (coherente con Fase 11)**: se utiliza **Google Cloud siempre**. Los embeddings se generan con **Google Gemini** a través de `Microsoft.SemanticKernel.Connectors.Google`, reutilizando el mismo proyecto de Google Cloud y la misma API Key (`AISettings:ApiKey`) de Fase 11. La abstracción permanece desacoplada del proveedor para futuras integraciones.

## Infraestructura vectorial
- [ ] Añadir servicio `qdrant` (se desplegará en Fase 14).
- [ ] Cliente Qdrant en `CampingAI.AI/RAG`.
- [ ] Servicio de embeddings basado en **Google Gemini** (`Microsoft.SemanticKernel.Connectors.Google`, `AddGoogleAIEmbeddingGeneration`, p. ej. `text-embedding-004`), implementando el contrato `IEmbeddingProvider` definido como stub en Fase 11 y reutilizando `AISettings:ApiKey`. Mantener la abstracción independiente del proveedor.

## Ingesta / indexación
- [ ] Pipeline de ingestión del conocimiento:
  información turística, actividades, descripciones, normativas, información local.
- [ ] Chunking + generación de embeddings (modelo de embeddings de Gemini) + upsert en Qdrant.
- [ ] Metadatos para filtrar por camping/zona.

## Consulta (RAG)
- [ ] Retrieval por similitud + construcción de prompt con contexto.
- [ ] Generación de respuesta con citación/contexto.
- [ ] Endpoint `POST /api/ai/ask` (o equivalente) para casos:
  - "¿Qué puedo visitar cerca del Camping X?"
  - "¿Qué actividades son adecuadas para niños?"
  - "¿Hay rutas de senderismo cercanas?"

## WebApi
- [ ] DTOs + endpoint en `AiController`.

## Tests
- [ ] Tests de retrieval (dado un índice mock) y de construcción de prompt.

## Criterio de aceptación
- Preguntas de ejemplo devuelven respuestas fundamentadas en el conocimiento indexado.
