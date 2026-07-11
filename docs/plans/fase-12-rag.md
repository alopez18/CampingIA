# Fase 12 — RAG (Retrieval Augmented Generation)

> Objetivo: responder preguntas usando conocimiento contextual con Semantic Kernel + Embeddings + Qdrant.

## Infraestructura vectorial
- [ ] Añadir servicio `qdrant` (se desplegará en Fase 14).
- [ ] Cliente Qdrant en `CampingAI.AI/RAG`.
- [ ] Servicio de embeddings (OpenAI/Azure OpenAI).

## Ingesta / indexación
- [ ] Pipeline de ingestión del conocimiento:
  información turística, actividades, descripciones, normativas, información local.
- [ ] Chunking + generación de embeddings + upsert en Qdrant.
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
