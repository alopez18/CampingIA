USE [CAMPING_AI_DB]
GO

-- ============================================================
-- Fase Extra — Migración de CMP_CategoryId (int -> uniqueidentifier)
-- ============================================================
-- Convierte la categoría principal del camping de un int suelto a
-- un uniqueidentifier con FK a T_CATEGORIES.
--   int 1 (Familiar)  -> B1000001-0000-0000-0000-000000000001
--   resto (fallback)  -> Familiar
-- Requiere haber ejecutado 12.SeedCategories.sql previamente.
-- Script idempotente: si la columna ya es uniqueidentifier no hace nada.
-- ============================================================

DECLARE @FamiliarId uniqueidentifier = N'B1000001-0000-0000-0000-000000000001';

-- Solo migrar si la columna sigue siendo int
IF EXISTS (
	SELECT 1 FROM sys.columns c
	INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
	WHERE c.object_id = OBJECT_ID(N'[dbo].[T_CAMPINGS]')
	  AND c.name = N'CMP_CategoryId'
	  AND t.name = N'int'
)
BEGIN
	-- 1. Columna temporal Guid
	IF NOT EXISTS (
		SELECT 1 FROM sys.columns
		WHERE object_id = OBJECT_ID(N'[dbo].[T_CAMPINGS]') AND name = N'CMP_CategoryIdGuid'
	)
		ALTER TABLE [dbo].[T_CAMPINGS] ADD [CMP_CategoryIdGuid] [uniqueidentifier] NULL;

	-- Necesario para poder referenciar la nueva columna en el mismo batch
	EXEC(N'UPDATE [dbo].[T_CAMPINGS]
			SET [CMP_CategoryIdGuid] = CASE [CMP_CategoryId]
				WHEN 1 THEN N''B1000001-0000-0000-0000-000000000001''
				ELSE N''B1000001-0000-0000-0000-000000000001''
			END;');

	-- 2. Eliminar la columna int
	ALTER TABLE [dbo].[T_CAMPINGS] DROP COLUMN [CMP_CategoryId];

	-- 3. Renombrar la columna Guid a CMP_CategoryId
	EXEC sp_rename N'[dbo].[T_CAMPINGS].[CMP_CategoryIdGuid]', N'CMP_CategoryId', N'COLUMN';

	-- 4. Hacerla NOT NULL
	ALTER TABLE [dbo].[T_CAMPINGS] ALTER COLUMN [CMP_CategoryId] [uniqueidentifier] NOT NULL;
END
GO

-- 5. Crear la FK a T_CATEGORIES si no existe
IF NOT EXISTS (
	SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CAMPINGS_CATEGORY'
)
	ALTER TABLE [dbo].[T_CAMPINGS]
		ADD CONSTRAINT [FK_CAMPINGS_CATEGORY]
		FOREIGN KEY ([CMP_CategoryId]) REFERENCES [dbo].[T_CATEGORIES] ([CAT_IdCategory]);
GO
