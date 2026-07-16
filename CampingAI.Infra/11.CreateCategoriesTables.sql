USE [CAMPING_AI_DB]
GO

-- ============================================================
-- Fase Extra — Catálogo de Categorías: creación de tablas
-- ============================================================
-- Script idempotente: crea T_CATEGORIES (catálogo) y
-- T_CAMPING_CATEGORIES (tabla puente Camping <-> Category para
-- categorías adicionales). Espejo de T_FACILITIES / T_CAMPING_FACILITIES.
-- ============================================================

-- ------------------------------------------------------------
-- T_CATEGORIES
-- ------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'T_CATEGORIES')
BEGIN
	CREATE TABLE [dbo].[T_CATEGORIES](
		[CAT_IdCategory] [uniqueidentifier] NOT NULL,
		[CAT_Name]       [nvarchar](100)    NOT NULL,
		CONSTRAINT [PK_T_CATEGORIES] PRIMARY KEY CLUSTERED ([CAT_IdCategory] ASC)
	) ON [PRIMARY]
END
GO

-- ------------------------------------------------------------
-- T_CAMPING_CATEGORIES  (tabla puente Camping <-> Category)
-- ------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'T_CAMPING_CATEGORIES')
BEGIN
	CREATE TABLE [dbo].[T_CAMPING_CATEGORIES](
		[CCA_IdCampingCategory] [uniqueidentifier] NOT NULL,
		[CCA_CampingId]         [uniqueidentifier] NOT NULL,
		[CCA_CategoryId]        [uniqueidentifier] NOT NULL,
		CONSTRAINT [PK_T_CAMPING_CATEGORIES] PRIMARY KEY CLUSTERED ([CCA_IdCampingCategory] ASC),
		CONSTRAINT [FK_CAMPING_CATEGORIES_CAMPING]  FOREIGN KEY ([CCA_CampingId])  REFERENCES [dbo].[T_CAMPINGS]   ([CMP_IdCamping]),
		CONSTRAINT [FK_CAMPING_CATEGORIES_CATEGORY] FOREIGN KEY ([CCA_CategoryId]) REFERENCES [dbo].[T_CATEGORIES] ([CAT_IdCategory])
	) ON [PRIMARY]
END
GO
