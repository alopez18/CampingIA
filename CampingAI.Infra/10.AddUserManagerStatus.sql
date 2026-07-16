USE [CAMPING_AI_DB]
GO

-- ============================================================
-- Migración: añadir USR_ManagerStatus a T_USERS
-- ============================================================
-- Estado de aprobación de gestor:
--   0 = None, 1 = Pending, 2 = Approved, 3 = Rejected
-- Script idempotente.
-- ============================================================

IF NOT EXISTS (
	SELECT 1 FROM sys.columns
	WHERE Name = N'USR_ManagerStatus'
	  AND Object_ID = Object_ID(N'[dbo].[T_USERS]'))
BEGIN
	ALTER TABLE [dbo].[T_USERS]
		ADD [USR_ManagerStatus] [int] NOT NULL
		CONSTRAINT [DF_T_USERS_ManagerStatus] DEFAULT (0);

	PRINT 'Columna USR_ManagerStatus añadida a T_USERS.';
END
ELSE
BEGIN
	PRINT 'La columna USR_ManagerStatus ya existe. No se realizaron cambios.';
END
GO
