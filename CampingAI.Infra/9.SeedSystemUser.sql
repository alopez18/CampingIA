USE [CAMPING_AI_DB]
GO

-- ============================================================
-- Seed: usuario Sistema
-- ============================================================
-- Script idempotente: crea un usuario "Sistema" con un GUID fijo
-- y controlable, usado como propietario (CMP_OwnerId) de todos los
-- campings traspasados desde T_CAMPINGS_IMPORT hacia T_CAMPINGS.
-- El hash de contraseña es un placeholder no válido para login.
-- ============================================================

DECLARE @SystemUserId uniqueidentifier = N'10000000-0000-0000-0000-000000000001';

IF NOT EXISTS (SELECT 1 FROM [dbo].[T_USERS] WHERE [USR_IdUser] = @SystemUserId)
BEGIN
	INSERT INTO [dbo].[T_USERS]
		([USR_IdUser], [USR_Email], [USR_PasswordHashed], [USR_Name], [USR_RoleId])
	VALUES
		(@SystemUserId, N'system@campingai.local', N'!DISABLED!', N'Sistema', 1);

	PRINT 'Usuario Sistema creado correctamente.';
END
ELSE
BEGIN
	PRINT 'El usuario Sistema ya existe. No se realizaron cambios.';
END
GO
