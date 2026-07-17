# CampingAI — Diagramas de Arquitectura

> Diagramas generados en formato **Mermaid**. Se pueden exportar a imagen con extensiones como *Mermaid Preview* (VS Code) o [mermaid.live](https://mermaid.live).

---

## 1. Arquitectura General

```mermaid
graph TB
	subgraph Cliente["Clientes"]
		MOB["📱 App Ionic / PWA<br/>(Angular 20 + Capacitor)"]
		WEB["🌐 Web MVC Razor<br/>(Backoffice / Admin)"]
	end

	subgraph Backend["Backend — CampingAI.WebApi (.NET 8)"]
		API["REST API<br/>/api/*"]
		MVC["MVC Controllers<br/>/Account, /Manager, /Admin"]
		MW["GlobalExceptionMiddleware"]
	end

	subgraph AppLayer["Aplicación — CampingAI.Application"]
		MED["IMediator<br/>(CQRS Dispatcher)"]
		CMD["Command Handlers"]
		QRY["Query Handlers"]
	end

	subgraph AILayer["IA — CampingAI.AI"]
		SK["Semantic Kernel"]
		GEM["GeminiAIProvider<br/>(Google Gemini 2.5 Flash)"]
		SRCH["CampingSearchAssistant"]
		REC["CampingRecommendationAssistant"]
		CMP["CampingComparisonAssistant"]
	end

	subgraph InfraLayer["Infraestructura — CampingAI.Infra"]
		REPO["Repositories<br/>(Dapper)"]
		UOW["UnitOfWork"]
		CONN["SqlConnectionFactory"]
	end

	subgraph Dominio["Dominio — CampingAI.Domain"]
		ENT["Entidades + Value Objects"]
		IFACE["Interfaces de Repositorio"]
	end

	subgraph Datos["Persistencia"]
		SQL[("SQL Server<br/>CampingAI_DB")]
	end

	subgraph ExtAPI["APIs Externas"]
		GEMAPI["Google Gemini API"]
		OSM["OpenStreetMap<br/>(Leaflet)"]
	end

	MOB -->|JWT Bearer| API
	WEB -->|Cookie Auth| MVC
	API --> MW
	MVC --> MW
	MW --> MED
	MED --> CMD
	MED --> QRY
	CMD --> REPO
	QRY --> REPO
	API -->|/api/ai/*| SK
	SK --> GEM
	GEM -->|REST| GEMAPI
	SK --> SRCH
	SK --> REC
	SK --> CMP
	SRCH -->|SearchCampingsQuery| MED
	REC -->|GetFavorites / GetReservations| MED
	REPO --> CONN
	CONN --> SQL
	MOB -->|Tiles| OSM
```

---

## 2. Clean Architecture — Capas y Dependencias

```mermaid
graph TD
	subgraph WA["CampingAI.WebApi"]
		CTRL["Controllers (API + MVC)"]
		DI_W["DI_Manager (WebApi)"]
		SW["Swagger / Auth"]
	end

	subgraph APP["CampingAI.Application"]
		MED2["IMediator"]
		HAND["Command & Query Handlers"]
		VAL["FluentValidation Validators"]
		MAPAPP["Mappers (Application)"]
		DI_A["DI_Manager (Application)"]
	end

	subgraph AI["CampingAI.AI"]
		PROV["IAIProvider"]
		ASS["Search / Rec / Compare Assistants"]
		DI_AI["DI_Manager (AI)"]
	end

	subgraph INF["CampingAI.Infra"]
		REPOI["Repository Implementations<br/>(Dapper)"]
		UOWI["UnitOfWork"]
		MAPINF["Mappers (Infra)"]
		DI_I["DI_Manager (Infra)"]
	end

	subgraph DOM["CampingAI.Domain"]
		ENTI["Entities"]
		VO["Value Objects"]
		IREPO["IReadRepository / IWriteRepository"]
		EXC["DomainException"]
	end

	WA -->|references| APP
	WA -->|references| AI
	APP -->|references| DOM
	AI -->|references| APP
	INF -->|references| DOM
	DI_W -->|chains| DI_A
	DI_A -->|chains| DI_I

	style DOM fill:#d4edda,stroke:#28a745
	style INF fill:#cce5ff,stroke:#004085
	style APP fill:#fff3cd,stroke:#856404
	style AI fill:#f8d7da,stroke:#721c24
	style WA fill:#e2e3e5,stroke:#383d41
```

---

## 3. CQRS — Commands, Queries y Mediator

```mermaid
sequenceDiagram
	participant C as Controller
	participant M as IMediator
	participant CH as ICommandHandler<br/>/ IQueryHandler
	participant R as IRepository
	participant UW as IUnitOfWork
	participant DB as SQL Server

	Note over C,DB: Flujo de escritura (Command)
	C->>M: SendCommandAsync(command)
	M->>CH: HandleAsync(command)
	CH->>R: SaveAsync(entity)
	R->>DB: INSERT / UPDATE (Dapper)
	DB-->>R: OK
	CH->>UW: SaveChangesAsync()
	UW-->>CH: committed
	CH-->>M: result
	M-->>C: response

	Note over C,DB: Flujo de lectura (Query)
	C->>M: SendQueryAsync(query)
	M->>CH: HandleAsync(query)
	CH->>R: GetByIdAsync() / SearchAsync()
	R->>DB: SELECT (Dapper)
	DB-->>R: rows
	R-->>CH: entities
	CH-->>M: DTO
	M-->>C: response
```

```mermaid
classDiagram
	class ICommand
	class ICommandHandlerT {
		<<interface>>
		+HandleAsync(command) Task
	}
	class ICommandHandlerTR {
		<<interface>>
		+HandleAsync(command) Task~TResult~
	}
	class IQuery~TResult~ {
		<<interface>>
	}
	class IQueryHandler~TQuery_TResult~ {
		<<interface>>
		+HandleAsync(query) Task~TResult~
	}
	class IMediator {
		<<interface>>
		+SendCommandAsync(command) Task
		+SendCommandAsync(command) Task~TResult~
		+SendQueryAsync(query) Task~TResult~
	}
	class Mediator {
		-IServiceProvider _provider
		+SendCommandAsync(command) Task
		+SendQueryAsync(query) Task~TResult~
	}

	IMediator <|.. Mediator
	ICommandHandlerT <-- Mediator : resolves
	IQueryHandler <-- Mediator : resolves
```

---

## 4. Modelo de Dominio — Entidades y Value Objects

```mermaid
classDiagram
	class Entity {
		+Guid Id
	}
	class Deleteable {
		+DateTime? DeletedOn
	}
	class IAuditableEntity {
		<<interface>>
		+DateFromPastVO CreatedOn
		+DateFromPastVO UpdatedOn
	}

	class User {
		+EmailVO Email
		+PasswordHashedVO PasswordHashed
		+string? Name
		+RoleVO Role
		+ManagerApprovalStatus ManagerStatus
		+CreateNew(email, password, name, role) User$
		+UpdateIdentity(name)
		+UpdatePassword(hash)
	}

	class Camping {
		+CampingNameVO Name
		+CampingDescriptionVO Description
		+LatitudeVO Latitude
		+LongitudeVO Longitude
		+PriceVO PricePerNight
		+Guid OwnerId
		+Guid CategoryId
		+Guid? ProvinciaId
		+IReadOnlyList~Guid~ FacilityIds
		+IReadOnlyList~Guid~ AdditionalCategoryIds
		+CreateNew(...) Camping$
		+Update(name, desc, lat, lng, price)
	}

	class Reservation {
		+Guid UserId
		+Guid CampingId
		+ReservationDateVO Dates
		+PriceVO TotalPrice
		+int StatusId
		+CreateNew(...) Reservation$
		+Cancel()
	}

	class Favorite {
		+Guid UserId
		+Guid CampingId
		+DateTime CreatedAt
		+CreateNew(userId, campingId) Favorite$
	}

	class Category {
		+CampingNameVO Name
	}

	class Facility {
		+CampingNameVO Name
	}

	class CampingCategory {
		+Guid CampingId
		+Guid CategoryId
	}

	class CampingFacility {
		+Guid CampingId
		+Guid FacilityId
	}

	%% Value Objects
	class EmailVO { +string Value }
	class PasswordHashedVO { +string Value }
	class RoleVO { +int Value }
	class CampingNameVO { +string Value }
	class CampingDescriptionVO { +string Value }
	class LatitudeVO { +decimal Value }
	class LongitudeVO { +decimal Value }
	class PriceVO { +decimal Value }
	class ReservationDateVO { +DateTime CheckIn; +DateTime CheckOut }
	class DateFromPastVO { +DateTime Value }

	Entity <|-- Deleteable
	Deleteable <|-- User
	Deleteable <|-- Camping
	Deleteable <|-- Reservation
	Entity <|-- Favorite
	Entity <|-- Category
	Entity <|-- Facility
	Entity <|-- CampingCategory
	Entity <|-- CampingFacility

	IAuditableEntity <|.. User
	IAuditableEntity <|.. Camping
	IAuditableEntity <|.. Reservation

	User --> EmailVO
	User --> PasswordHashedVO
	User --> RoleVO
	Camping --> CampingNameVO
	Camping --> CampingDescriptionVO
	Camping --> LatitudeVO
	Camping --> LongitudeVO
	Camping --> PriceVO
	Reservation --> ReservationDateVO
	Reservation --> PriceVO

	User "1" --> "0..*" Camping : owns
	User "1" --> "0..*" Reservation
	User "1" --> "0..*" Favorite
	Camping "1" --> "0..*" Reservation
	Camping "1" --> "0..*" Favorite
	Camping "1" --> "0..*" CampingFacility
	Camping "1" --> "0..*" CampingCategory
	Facility "1" --> "0..*" CampingFacility
	Category "1" --> "0..*" CampingCategory
```

---

## 5. Modelo de Base de Datos — Tablas y Relaciones

```mermaid
erDiagram
	T_USERS {
		uniqueidentifier USR_IdUser PK
		nvarchar USR_Email
		nvarchar USR_PasswordHashed
		nvarchar USR_Name "nullable"
		int USR_RoleId
		int USR_ManagerStatus
		datetime USR_CreatedOn
		datetime USR_UpdatedOn
		datetime USR_DeletedOn "nullable"
	}

	T_CAMPINGS {
		uniqueidentifier CMP_IdCamping PK
		nvarchar CMP_Name
		nvarchar CMP_Description
		decimal CMP_Latitude
		decimal CMP_Longitude
		decimal CMP_PricePerNight
		uniqueidentifier CMP_OwnerId FK
		uniqueidentifier CMP_CategoryId FK
		uniqueidentifier CMP_ProvinciaId FK "nullable"
		datetime CMP_CreatedOn
		datetime CMP_UpdatedOn
		datetime CMP_DeletedOn "nullable"
	}

	T_RESERVATIONS {
		uniqueidentifier RES_IdReservation PK
		uniqueidentifier RES_UserId FK
		uniqueidentifier RES_CampingId FK
		datetime RES_CheckIn
		datetime RES_CheckOut
		decimal RES_TotalPrice
		int RES_StatusId
		datetime RES_CreatedOn
		datetime RES_UpdatedOn
		datetime RES_DeletedOn "nullable"
	}

	T_FAVORITES {
		uniqueidentifier FAV_IdFavorite PK
		uniqueidentifier FAV_UserId FK
		uniqueidentifier FAV_CampingId FK
		datetime FAV_CreatedAt
	}

	T_CATEGORIES {
		uniqueidentifier CAT_IdCategory PK
		nvarchar CAT_Name
	}

	T_FACILITIES {
		uniqueidentifier FAC_IdFacility PK
		nvarchar FAC_Name
	}

	T_CAMPING_CATEGORIES {
		uniqueidentifier CCA_IdCampingCategory PK
		uniqueidentifier CCA_CampingId FK
		uniqueidentifier CCA_CategoryId FK
	}

	T_CAMPING_FACILITIES {
		uniqueidentifier CFE_IdCampingFacility PK
		uniqueidentifier CFE_CampingId FK
		uniqueidentifier CFE_FacilityId FK
	}

	T_PROVINCES {
		uniqueidentifier PRV_IdProvince PK
		nvarchar PRV_Name
		uniqueidentifier PRV_CountryId FK
	}

	T_COUNTRIES {
		uniqueidentifier CTR_IdCountry PK
		nvarchar CTR_Name
	}

	T_USERS ||--o{ T_CAMPINGS : "owns (OwnerId)"
	T_USERS ||--o{ T_RESERVATIONS : "makes"
	T_USERS ||--o{ T_FAVORITES : "saves"
	T_CAMPINGS ||--o{ T_RESERVATIONS : "has"
	T_CAMPINGS ||--o{ T_FAVORITES : "saved in"
	T_CAMPINGS ||--o{ T_CAMPING_CATEGORIES : "has"
	T_CAMPINGS ||--o{ T_CAMPING_FACILITIES : "has"
	T_CATEGORIES ||--o{ T_CAMPINGS : "primary category"
	T_CATEGORIES ||--o{ T_CAMPING_CATEGORIES : "additional"
	T_FACILITIES ||--o{ T_CAMPING_FACILITIES : "assigned to"
	T_COUNTRIES ||--o{ T_PROVINCES : "contains"
	T_PROVINCES ||--o{ T_CAMPINGS : "located in"
```

---

## 6. Flujo IA — Lenguaje Natural → Filtros → Resultados

### Módulo 1: Búsqueda Inteligente

```mermaid
sequenceDiagram
	participant U as Usuario (App Ionic)
	participant API as POST /api/ai/search
	participant SA as CampingSearchAssistant
	participant AI as GeminiAIProvider<br/>(Gemini 2.5 Flash)
	participant VAL as JSON Validator
	participant MED as IMediator
	participant REPO as CampingsRepository
	participant DB as SQL Server

	U->>API: { "query": "camping tranquilo con piscina en Cataluña" }
	API->>SA: SearchAsync(query, catalog)
	SA->>SA: Build system prompt<br/>(categories + facilities catalog)
	SA->>AI: CompleteAsync(prompt + query)
	AI-->>SA: JSON { provincia, minPrice, maxPrice,<br/>facilityIds, categoryIds, ... }
	SA->>VAL: Validate & parse AiSearchFilters
	VAL-->>SA: SearchCampingsQuery
	SA->>MED: SendQueryAsync(SearchCampingsQuery)
	MED->>REPO: SearchAsync(filters)
	REPO->>DB: SELECT parametrizado (Dapper)
	DB-->>REPO: rows
	REPO-->>MED: List~Camping~
	MED-->>SA: results
	SA-->>API: CampingSearchResponse
	API-->>U: 200 OK [ campings ]
```

### Módulo 2: Recomendaciones Personalizadas

```mermaid
sequenceDiagram
	participant U as Usuario Autenticado
	participant API as GET /api/ai/recommendations
	participant RS as RecommendationService
	participant RA as CampingRecommendationAssistant
	participant AI as GeminiAIProvider
	participant MED as IMediator

	U->>API: (JWT token)
	API->>MED: GetUserFavoritesQuery(userId)
	MED-->>API: favorites
	API->>MED: GetUserReservationsQuery(userId)
	MED-->>API: reservations
	API->>RS: GetRecommendationsAsync(context)
	RS->>RA: RecommendAsync(favorites, reservations, allCampings)
	RA->>AI: CompleteAsync(prompt con historial usuario)
	AI-->>RA: JSON [ { campingId, reason } ]
	RA-->>RS: List~RecommendationResponse~
	RS-->>API: recommendations
	API-->>U: 200 OK [ recomendaciones con motivo ]
```

### Módulo 3: Comparador Inteligente

```mermaid
sequenceDiagram
	participant U as Usuario
	participant API as POST /api/ai/compare
	participant CA as CampingComparisonAssistant
	participant AI as GeminiAIProvider
	participant MED as IMediator

	U->>API: { "campingIds": [id1, id2, id3] }
	API->>MED: GetCampingByIdQuery x N
	MED-->>API: List~Camping~
	API->>CA: CompareAsync(campings)
	CA->>AI: CompleteAsync(prompt con datos reales)
	AI-->>CA: JSON { summary, winner, comparisons[] }
	CA-->>API: CompareResponse
	API-->>U: 200 OK { análisis comparativo }
```

---

## 7. Diagrama de Componentes — App Móvil (Ionic)

```mermaid
graph TD
	subgraph Ionic["App Ionic 8 + Angular 20"]
		AUTH["AuthModule<br/>(login, registro)"]
		HOME["HomePage<br/>(búsqueda + filtros)"]
		LIST["CampingsPage<br/>(lista paginada + infinite scroll)"]
		DET["CampingDetailPage<br/>(info + reservar + favorito)"]
		MAP["MapPage<br/>(Leaflet + clustering)"]
		FAV["FavoritesPage"]
		RES["ReservationsPage"]
		PROF["ProfilePage"]
		AI_P["AI SearchPage<br/>(búsqueda en lenguaje natural)"]
	end

	subgraph Services["Angular Services"]
		ASRV["AuthService (JWT)"]
		CSRV["CampingsService"]
		FSRV["FavoritesService"]
		RSRV["ReservationsService"]
		AISRV["AiService"]
		GEO["GeolocationService<br/>(@capacitor/geolocation)"]
	end

	subgraph Backend["CampingAI.WebApi"]
		BAPI["REST API"]
	end

	AUTH --> ASRV
	HOME --> CSRV
	HOME --> AISRV
	LIST --> CSRV
	DET --> CSRV
	DET --> FSRV
	DET --> RSRV
	MAP --> CSRV
	MAP --> GEO
	FAV --> FSRV
	RES --> RSRV
	PROF --> ASRV
	AI_P --> AISRV

	ASRV -->|HTTP + JWT| BAPI
	CSRV -->|HTTP| BAPI
	FSRV -->|HTTP| BAPI
	RSRV -->|HTTP| BAPI
	AISRV -->|HTTP| BAPI
```
