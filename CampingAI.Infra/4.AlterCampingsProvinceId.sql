USE [CAMPING_AI_DB]
GO

-- ============================================================
-- Localización — Migrar T_CAMPINGS.CMP_Provincia (texto)
--               a CMP_ProvinciaId (FK a T_PROVINCES)
-- NOTA: Ejecutar DESPUÉS de 3.CreateLocationTables.sql
-- ============================================================

-- 1. Eliminar la columna de texto añadida en Fase 6
ALTER TABLE [dbo].[T_CAMPINGS]
	DROP CONSTRAINT [DF_T_CAMPINGS_Mascotas],
	COLUMN [CMP_Provincia];
GO

-- 2. Añadir la nueva columna de FK
ALTER TABLE [dbo].[T_CAMPINGS]
	ADD [CMP_ProvinciaId] [uniqueidentifier] NULL;
GO

-- 3. Restricción de clave foránea
ALTER TABLE [dbo].[T_CAMPINGS]
	ADD CONSTRAINT [FK_T_CAMPINGS_PROVINCE]
		FOREIGN KEY ([CMP_ProvinciaId])
		REFERENCES [dbo].[T_PROVINCES] ([PRV_IdProvince]);
GO
