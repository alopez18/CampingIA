USE [CAMPING_AI_DB]
GO

-- ============================================================
-- Localización — Tablas de países y provincias
-- ============================================================

-- ------------------------------------------------------------
-- T_COUNTRIES
-- ------------------------------------------------------------
CREATE TABLE [dbo].[T_COUNTRIES](
	[CNT_IdCountry] [uniqueidentifier] NOT NULL,
	[CNT_Code]      [nvarchar](3)      NOT NULL,   -- ISO 3166-1 alpha-2/3  (ej. "ES")
	[CNT_Name]      [nvarchar](255)    NOT NULL,
	CONSTRAINT [PK_T_COUNTRIES] PRIMARY KEY CLUSTERED ([CNT_IdCountry] ASC)
) ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_T_COUNTRIES_Code]
	ON [dbo].[T_COUNTRIES] ([CNT_Code] ASC)
GO

-- ------------------------------------------------------------
-- T_PROVINCES
-- ------------------------------------------------------------
CREATE TABLE [dbo].[T_PROVINCES](
	[PRV_IdProvince] [uniqueidentifier] NOT NULL,
	[PRV_Code]       [nvarchar](10)     NOT NULL,   -- human-friendly  (ej. "MAD", "BCN")
	[PRV_Name]       [nvarchar](255)    NOT NULL,
	[PRV_CountryId]  [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_T_PROVINCES] PRIMARY KEY CLUSTERED ([PRV_IdProvince] ASC),
	CONSTRAINT [FK_T_PROVINCES_COUNTRY] FOREIGN KEY ([PRV_CountryId])
		REFERENCES [dbo].[T_COUNTRIES] ([CNT_IdCountry])
) ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_T_PROVINCES_Code]
	ON [dbo].[T_PROVINCES] ([PRV_Code] ASC)
GO

-- ============================================================
-- SEED — España + 50 provincias
-- ============================================================

DECLARE @ES uniqueidentifier = '11111111-0000-0000-0000-000000000001'

INSERT INTO [dbo].[T_COUNTRIES] ([CNT_IdCountry], [CNT_Code], [CNT_Name])
VALUES (@ES, 'ES', N'España')
GO

DECLARE @ES uniqueidentifier = '11111111-0000-0000-0000-000000000001'

INSERT INTO [dbo].[T_PROVINCES] ([PRV_IdProvince], [PRV_Code], [PRV_Name], [PRV_CountryId]) VALUES
-- Andalucía
(NEWID(), 'ALM', N'Almería',        @ES),
(NEWID(), 'CAD', N'Cádiz',          @ES),
(NEWID(), 'COR', N'Córdoba',        @ES),
(NEWID(), 'GRA', N'Granada',        @ES),
(NEWID(), 'HUE', N'Huelva',         @ES),
(NEWID(), 'JAE', N'Jaén',           @ES),
(NEWID(), 'MAL', N'Málaga',         @ES),
(NEWID(), 'SEV', N'Sevilla',        @ES),
-- Aragón
(NEWID(), 'HUS', N'Huesca',         @ES),
(NEWID(), 'TER', N'Teruel',         @ES),
(NEWID(), 'ZAR', N'Zaragoza',       @ES),
-- Asturias
(NEWID(), 'AST', N'Asturias',       @ES),
-- Baleares
(NEWID(), 'BAL', N'Illes Balears',  @ES),
-- Canarias
(NEWID(), 'LPA', N'Las Palmas',     @ES),
(NEWID(), 'TFE', N'Santa Cruz de Tenerife', @ES),
-- Cantabria
(NEWID(), 'CAN', N'Cantabria',      @ES),
-- Castilla-La Mancha
(NEWID(), 'ALB', N'Albacete',       @ES),
(NEWID(), 'CIU', N'Ciudad Real',    @ES),
(NEWID(), 'CUE', N'Cuenca',         @ES),
(NEWID(), 'GUA', N'Guadalajara',    @ES),
(NEWID(), 'TOL', N'Toledo',         @ES),
-- Castilla y León
(NEWID(), 'AVI', N'Ávila',          @ES),
(NEWID(), 'BUR', N'Burgos',         @ES),
(NEWID(), 'LEO', N'León',           @ES),
(NEWID(), 'PAL', N'Palencia',       @ES),
(NEWID(), 'SAL', N'Salamanca',      @ES),
(NEWID(), 'SEG', N'Segovia',        @ES),
(NEWID(), 'SOR', N'Soria',          @ES),
(NEWID(), 'VAL', N'Valladolid',     @ES),
(NEWID(), 'ZAM', N'Zamora',         @ES),
-- Cataluña
(NEWID(), 'BCN', N'Barcelona',      @ES),
(NEWID(), 'GIR', N'Girona',         @ES),
(NEWID(), 'LLE', N'Lleida',         @ES),
(NEWID(), 'TAR', N'Tarragona',      @ES),
-- Extremadura
(NEWID(), 'BAD', N'Badajoz',        @ES),
(NEWID(), 'CAC', N'Cáceres',        @ES),
-- Galicia
(NEWID(), 'COR2', N'A Coruña',      @ES),
(NEWID(), 'LUG', N'Lugo',           @ES),
(NEWID(), 'OUR', N'Ourense',        @ES),
(NEWID(), 'PON', N'Pontevedra',     @ES),
-- La Rioja
(NEWID(), 'RIO', N'La Rioja',       @ES),
-- Madrid
(NEWID(), 'MAD', N'Madrid',         @ES),
-- Murcia
(NEWID(), 'MUR', N'Murcia',         @ES),
-- Navarra
(NEWID(), 'NAV', N'Navarra',        @ES),
-- País Vasco
(NEWID(), 'ALA', N'Álava',          @ES),
(NEWID(), 'GUI', N'Guipúzcoa',      @ES),
(NEWID(), 'VIZ', N'Vizcaya',        @ES),
-- Valencia
(NEWID(), 'ALC', N'Alicante',       @ES),
(NEWID(), 'CST', N'Castellón',      @ES),
(NEWID(), 'VLC', N'Valencia',       @ES),
-- Ceuta y Melilla
(NEWID(), 'CEU', N'Ceuta',          @ES),
(NEWID(), 'MEL', N'Melilla',        @ES)
GO
