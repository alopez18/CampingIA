# Plan de Obtención de Datos Oficiales de Campings y Recursos Turísticos de las Comunidades Autónomas

## Proyecto

CampingAI

## Objetivo

Crear un sistema de adquisición, normalización y consolidación de información turística oficial procedente de los portales de datos abiertos de las Comunidades Autónomas de España.

La finalidad es construir una base de datos propia, fiable y actualizable que complemente los datos obtenidos desde OpenStreetMap y permita enriquecer la experiencia de búsqueda, recomendación e inteligencia artificial de CampingAI.

---

# Objetivos Específicos

## Obtener datos oficiales de:

- Campings.
- Alojamientos turísticos.
- Municipios.
- Oficinas de turismo.
- Recursos turísticos.
- Monumentos.
- Museos.
- Espacios naturales.
- Playas.
- Rutas turísticas.

---

## Crear una fuente unificada

Todos los conjuntos de datos deberán transformarse a un modelo común para evitar diferencias entre comunidades autónomas.

---

## Disponer de datos reutilizables para:

- Buscador.
- Mapa interactivo.
- Recomendador IA.
- Sistema RAG.
- Comparador de destinos.
- Información turística contextual.

---

# Estrategia General

## Fase 1

Identificación de fuentes oficiales.

## Fase 2

Descarga automática de datasets.

## Fase 3

Normalización.

## Fase 4

Persistencia en SQL Server.

## Fase 5

Enriquecimiento mediante IA.

---

# Arquitectura

```text
Portales Open Data

       │

       ▼

DataImporters

       │

       ▼

Transformadores

       │

       ▼

Normalizador

       │

       ▼

SQL Server

       │

       ▼

CampingAI API

       │

       ▼

Módulos IA
```

---

# Proyecto Técnico

## Nuevo Proyecto

```text
CampingAI.DataSources
```

Responsabilidades:

- Conectarse a fuentes externas.
- Descargar datasets.
- Validar estructura.
- Normalizar datos.
- Actualizar información.
- Gestionar errores.

---

# Comunidad Autónoma por Comunidad Autónoma

## Cataluña

Portal:

```text
https://analisi.transparenciacatalunya.cat
```

Datos de interés:

- Campings.
- Alojamientos turísticos.
- Municipios.
- Espacios naturales.
- Recursos turísticos.

Prioridad:

```text
ALTA
```

---

## Comunidad Valenciana

Portal:

```text
https://dadesobertes.gva.es
```

Datos de interés:

- Campings registrados.
- Recursos turísticos.
- Playas.
- Espacios naturales.

Prioridad:

```text
ALTA
```

---

## Andalucía

Portal:

```text
https://www.juntadeandalucia.es/datosabiertos
```

Datos de interés:

- Campings.
- Recursos turísticos.
- Patrimonio.
- Espacios protegidos.

Prioridad:

```text
ALTA
```

---

## Galicia

Portal:

```text
https://abertos.xunta.gal
```

Datos de interés:

- Campings.
- Turismo rural.
- Playas.
- Patrimonio.

Prioridad:

```text
MEDIA
```

---

## Aragón

Portal:

```text
https://opendata.aragon.es
```

Datos de interés:

- Campings.
- Turismo de naturaleza.
- Espacios protegidos.

Prioridad:

```text
MEDIA
```

---

## Madrid

Portal:

```text
https://datos.comunidad.madrid
```

Datos de interés:

- Alojamientos turísticos.
- Recursos culturales.
- Espacios turísticos.

Prioridad:

```text
MEDIA
```

---

## Castilla y León

Portal:

```text
https://datosabiertos.jcyl.es
```

Datos de interés:

- Campings.
- Turismo rural.
- Patrimonio histórico.

Prioridad:

```text
MEDIA
```

---

## Castilla-La Mancha

Portal:

```text
https://datosabiertos.castillalamancha.es
```

Prioridad:

```text
MEDIA
```

---

## Extremadura

Portal:

```text
https://opendata.juntaex.es
```

Prioridad:

```text
MEDIA
```

---

## Asturias

Portal:

```text
https://datos.asturias.es
```

Prioridad:

```text
MEDIA
```

---

## Cantabria

Portal:

```text
https://datosabiertos.cantabria.es
```

Prioridad:

```text
MEDIA
```

---

## País Vasco

Portal:

```text
https://opendata.euskadi.eus
```

Prioridad:

```text
ALTA
```

---

## Navarra

Portal:

```text
https://gobiernoabierto.navarra.es
```

Prioridad:

```text
MEDIA
```

---

## Murcia

Portal:

```text
https://datosabiertos.carm.es
```

Prioridad:

```text
MEDIA
```

---

## Baleares

Portal:

```text
https://dadesobertes.caib.es
```

Prioridad:

```text
ALTA
```

---

## Canarias

Portal:

```text
https://datos.canarias.es
```

Prioridad:

```text
ALTA
```

---

# Modelo de Datos Unificado

## Tabla Campings

```text
Id

ExternalId

Name

Description

Category

Address

PostalCode

City

Province

Region

Country

Latitude

Longitude

Phone

Email

Website

Source

SourceRecordId

CreatedOn

UpdatedOn
```

---

# Tabla TouristResources

```text
Id

ExternalId

Name

Description

Type

Latitude

Longitude

Address

Municipality

Province

Region

Website

Source

CreatedOn
```

---

# Tabla Beaches

```text
Id

Name

Municipality

Province

Latitude

Longitude

Description
```

---

# Tabla NaturalParks

```text
Id

Name

Province

Latitude

Longitude

Description
```

---

# Estrategia de Importación

## Extracción

Crear un importador por comunidad autónoma.

Ejemplo:

```text
CataloniaImporter

ValenciaImporter

AndalusiaImporter

AragonImporter
```

---

## Transformación

Cada dataset deberá convertirse a DTOs comunes.

Ejemplo:

```text
CampingRawDto

ResourceRawDto
```

↓

```text
CampingNormalizedDto

TouristResourceNormalizedDto
```

---

## Carga

Persistir utilizando:

```text
CampingAI.Infra

Repositories

Dapper
```

---

# Gestión de Duplicados

Identificar registros mediante:

```text
Source

SourceRecordId
```

y además:

```text
Nombre
+
Municipio
+
Coordenadas
```

---

# Normalización

## Provincias

Unificar nombres.

Ejemplo:

```text
Barcelona

BARCELONA

barcelona
```

↓

```text
Barcelona
```

---

## Municipios

Eliminar duplicidades.

---

## Coordenadas

Convertir todos los formatos a:

```text
WGS84
```

---

# Actualizaciones

## Estrategia

Ejecución automática:

```text
Mensual
```

o

```text
Trimestral
```

---

# Integración con IA

## Fase Posterior

Generar automáticamente:

```text
AiSummary
AiTags
AiCategories
```

---

## Ejemplo

Entrada:

```text
Camping situado frente al mar con piscina,
animación infantil y restaurante.
```

Salida:

```text
Categorías:

- Playa
- Familiar

Tags:

- Piscina
- Restaurante
- Niños

Resumen:

Camping familiar ideal para vacaciones junto al mar.
```

---

# Integración con RAG

Los datos oficiales servirán como base documental para:

```text
¿Qué puedo visitar cerca de este camping?

¿Qué actividades familiares existen?

¿Qué playas cercanas son recomendables?

¿Qué parques naturales hay en la zona?
```

---

# Roadmap

## Sprint 1

Identificación de datasets.

---

## Sprint 2

Implementación de Cataluña.

---

## Sprint 3

Implementación de Comunidad Valenciana.

---

## Sprint 4

Implementación de Andalucía.

---

## Sprint 5

Implementación de Aragón y Madrid.

---

## Sprint 6

Resto de comunidades autónomas.

---

## Sprint 7

Normalización global.

---

## Sprint 8

Integración SQL Server.

---

## Sprint 9

Integración API.

---

## Sprint 10

Integración IA.

---

## Sprint 11

Integración RAG.

---

# Criterios de Éxito

✅ Datos de todas las comunidades autónomas.

✅ Base de datos unificada.

✅ Información oficial normalizada.

✅ Actualizaciones automáticas.

✅ Integración con CampingAI.

✅ Integración con IA.

✅ Integración con RAG.

✅ Base documental turística nacional.