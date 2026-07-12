# CampingAI - Plan de Importación de Campings desde OpenStreetMap

## Objetivo

Construir un sistema de importación de campings reales utilizando OpenStreetMap y Overpass API como fuente principal de datos.

El objetivo es disponer de una base de datos propia de campings sobre la que se desarrollarán posteriormente:

- Búsquedas avanzadas
- Mapa interactivo
- Reservas
- Favoritos
- Recomendaciones IA
- Comparador IA
- Sistema RAG

La aplicación NO consultará Overpass API en tiempo real.

OpenStreetMap se utilizará únicamente como fuente de carga inicial de datos.

---

# Arquitectura

```text
OpenStreetMap
       │
       ▼
 Overpass API
       │
       ▼
CampingAI.DataImporter
       │
       ▼
 SQL Server
       │
       ▼
CampingAI API
       │
       ▼
Ionic App
```

---

# Objetivos de la Fase

## Corto plazo

Disponer de una base de datos con cientos o miles de campings reales.

## Medio plazo

Poder buscar y visualizar campings desde la aplicación.

## Largo plazo

Enriquecer los campings mediante IA para generar:

- Categorías
- Tags
- Resúmenes
- Recomendaciones

---

# Proyecto Nuevo

## CampingAI.DataImporter

Crear un proyecto independiente.

### Responsabilidades

- Consumir Overpass API.
- Descargar datos de campings.
- Transformar resultados.
- Normalizar datos.
- Insertar datos en SQL Server.
- Evitar duplicados.
- Permitir futuras sincronizaciones.

---

# Fuente de Datos

## OpenStreetMap

Tag principal:

```text
tourism=camp_site
```

Todos los campings importados deberán provenir inicialmente de este tipo.

---

# Modelo de Datos Inicial

## Tabla Campings

```text
Campings
```

Campos:

```text
Id (Guid)

ExternalId

Name

Description

Latitude

Longitude

Address

PostalCode

City

Province

Country

Phone

Email

Website

Source

CreatedOn

UpdatedOn
```

---

# Campo Source

Permite identificar el origen.

Ejemplo:

```text
OpenStreetMap
```

---

# Identificador Externo

## ExternalId

Guardar el identificador original de OpenStreetMap.

Objetivo:

- Evitar duplicados.
- Permitir futuras actualizaciones.

---

# Estrategia de Importación

## Fase 1

Importar únicamente:

```text
Nombre

Coordenadas

Datos de contacto

Dirección
```

No realizar todavía enriquecimiento IA.

---

## Fase 2

Normalización.

Actualizar:

```text
Provincia

Código postal

País

Municipio
```

---

## Fase 3

Enriquecimiento IA.

Generar:

```text
Resumen IA

Categorías IA

Tags IA
```

---

# Estructura del Importador

```text
CampingAI.DataImporter

├── Services

├── Clients

├── Mappers

├── DTOs

├── Importers

└── Configuration
```

---

# DTO Overpass

Crear DTOs para representar la respuesta JSON de Overpass.

Ejemplo:

```text
OverpassResponse

OverpassElement

OverpassTags
```

---

# Cliente Overpass

## OverpassClient

Responsabilidades:

- Ejecutar consultas.
- Gestionar reintentos.
- Deserializar respuestas.
- Registrar errores.

---

# Servicio de Transformación

## CampingMapper

Convertirá:

```text
OverpassElement
```

en

```text
CampingEntity
```

---

# Servicio de Persistencia

## CampingImportService

Responsabilidades:

- Comprobar existencia.
- Insertar nuevos campings.
- Actualizar campings existentes.
- Mantener consistencia.

---

# Estrategia de Duplicados

Antes de insertar:

Buscar por:

```text
ExternalId
```

Si existe:

```text
Actualizar
```

Si no existe:

```text
Insertar
```

---

# Orden de Importación

## Paso 1

Cataluña

---

## Paso 2

Comunidad Valenciana

---

## Paso 3

Aragón

---

## Paso 4

Madrid

---

## Paso 5

Andalucía

---

## Paso 6

Resto de España

---

# Consultas de Validación

Implementar queries para verificar:

## Número de Campings

```sql
SELECT COUNT(*)
FROM Campings
```

---

## Campings Sin Coordenadas

```sql
SELECT *
FROM Campings
WHERE Latitude IS NULL
OR Longitude IS NULL
```

---

## Duplicados

```sql
Agrupar por ExternalId
```

Debe devolver 0 registros duplicados.

---

# Futuras Tablas

## Categories

```text
Id

Name
```

---

## CampingCategories

```text
CampingId

CategoryId
```

---

## Services

```text
Id

Name
```

---

## CampingServices

```text
CampingId

ServiceId
```

---

# Enriquecimiento IA

## Objetivo

Generar metadatos útiles para búsquedas.

---

# Campos IA

Añadir posteriormente:

```text
AiSummary

AiCategories

AiTags

AiProcessedAt
```

---

# Categorías Iniciales

```text
Playa

Montaña

Familiar

Pet Friendly

Glamping

Naturaleza

Aventura

Lujo
```

---

# Tags Iniciales

```text
Piscina

Restaurante

Supermercado

Animación

Senderismo

Bungalows

Caravanas

Wifi
```

---

# Flujo IA

```text
Camping

↓

Prompt

↓

OpenAI

↓

Clasificación

↓

Actualización SQL
```

---

# Integración con API

Una vez cargados los datos:

Implementar:

```text
GetCampingsQuery

GetCampingByIdQuery

SearchCampingsQuery

GetNearbyCampingsQuery
```

---

# Integración con Mapa

Campos obligatorios:

```text
Latitude

Longitude
```

Estos datos permitirán:

- Marcadores
- Distancias
- Cercanía
- Búsquedas geográficas

---

# Criterios de Finalización

La fase de importación se considerará finalizada cuando:

✅ Exista el proyecto CampingAI.DataImporter.

✅ Se puedan importar campings desde OpenStreetMap.

✅ Los campings se almacenen en SQL Server.

✅ No existan duplicados.

✅ Se almacenen coordenadas geográficas.

✅ Existan al menos 1.000 campings de*