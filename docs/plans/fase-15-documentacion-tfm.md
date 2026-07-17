# Fase 15 — Documentación TFM

> Objetivo: diagramas y memoria del Trabajo Fin de Máster.

> **Nota:** el RAG es una **Fase Bonus** (`fase-bonus-rag.md`) que solo se implementará si da tiempo. Los elementos de documentación relativos a RAG se marcan como opcionales y solo se incluirán si dicha fase llega a implementarse.

## Diagramas
- [x] Arquitectura general.
- [x] Clean Architecture (capas y dependencias).
- [x] CQRS (Commands/Queries + Mediator propio).
- [x] Modelo de dominio (entidades y VOs).
- [x] Modelo de base de datos (tablas y relaciones).
- [x] Flujo IA (lenguaje natural → filtros → resultados).
- [ ] *(Opcional / Bonus RAG)* Flujo RAG (ingesta → embeddings → Qdrant → respuesta).

## Memoria
- [x] Problema y motivación.
- [x] Objetivos (principal + secundarios).
- [x] Arquitectura y decisiones de diseño.
- [x] Tecnologías utilizadas.
- [x] Proceso de desarrollo por fases.
- [x] IA aplicada (búsqueda, recomendaciones, comparador; *(opcional / Bonus)* RAG).
- [x] Resultados y evaluación (frente al criterio de éxito).
- [x] Futuras mejoras.

## Criterio de aceptación
- [x] Documento de memoria completo + set de diagramas exportados.
- [x] Trazabilidad entre criterio de éxito del MasterPlan y lo implementado.

---

## Registro de implementación

**Fecha:** 2025-07-17  
**Autor:** Copilot

### Ficheros creados
- `docs/diagrams/diagramas.md` — 7 diagramas Mermaid: arquitectura general, Clean Architecture, CQRS (secuencia + clases), modelo de dominio (classDiagram), modelo de BD (erDiagram), flujo IA × 3 módulos (búsqueda, recomendaciones, comparador), y diagrama de componentes de la app Ionic.
- `docs/memoria-tfm.md` — Memoria completa del TFM con 8 secciones: problema/motivación, objetivos, arquitectura, tecnologías, proceso por fases, IA aplicada, resultados/evaluación y futuras mejoras.

### Ficheros modificados
- `docs/plans/fase-15-documentacion-tfm.md` — Actualizado con tareas completadas `[x]`, criterios de aceptación y este registro.

### Resultado final
- Diagramas: ✅ 6/6 (el opcional RAG queda pendiente hasta implementar fase bonus)
- Memoria: ✅ 8/8 secciones
- Build: no aplica (documentación únicamente)
