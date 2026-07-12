-- Eliminación de la columna CMP_MascotasPermitidas de T_CAMPINGS
-- La información de mascotas queda cubierta por la tabla T_FACILITIES
-- (registros 'Admite mascotas' y 'Zona de lavado para mascotas').

-- Eliminar la restricción DEFAULT antes de eliminar la columna
IF EXISTS (
	SELECT 1 FROM sys.default_constraints
	WHERE parent_object_id = OBJECT_ID(N'[dbo].[T_CAMPINGS]')
	  AND name = N'DF_T_CAMPINGS_Mascotas'
)
	ALTER TABLE [dbo].[T_CAMPINGS] DROP CONSTRAINT [DF_T_CAMPINGS_Mascotas];

-- Eliminar la columna
IF EXISTS (
	SELECT 1 FROM sys.columns
	WHERE object_id = OBJECT_ID(N'[dbo].[T_CAMPINGS]')
	  AND name = N'CMP_MascotasPermitidas'
)
	ALTER TABLE [dbo].[T_CAMPINGS] DROP COLUMN [CMP_MascotasPermitidas];
