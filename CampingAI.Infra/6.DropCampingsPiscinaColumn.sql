-- Fase: eliminación de la columna CMP_Piscina de T_CAMPINGS
-- La información de piscina queda cubierta por la tabla T_FACILITIES (registro 'Piscina').

-- Eliminar la restricción DEFAULT antes de eliminar la columna
IF EXISTS (
	SELECT 1 FROM sys.default_constraints
	WHERE parent_object_id = OBJECT_ID(N'[dbo].[T_CAMPINGS]')
	  AND name = N'DF_T_CAMPINGS_Piscina'
)
	ALTER TABLE [dbo].[T_CAMPINGS] DROP CONSTRAINT [DF_T_CAMPINGS_Piscina];

-- Eliminar la columna
IF EXISTS (
	SELECT 1 FROM sys.columns
	WHERE object_id = OBJECT_ID(N'[dbo].[T_CAMPINGS]')
	  AND name = N'CMP_Piscina'
)
	ALTER TABLE [dbo].[T_CAMPINGS] DROP COLUMN [CMP_Piscina];
