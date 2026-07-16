USE [CAMPING_AI_DB]
GO

-- ============================================================
-- Fase Extra — Seed: catálogo base de T_CATEGORIES
-- ============================================================
-- Script idempotente: inserta solo si el nombre no existe.
-- La categoría 'Familiar' recibe la migración del antiguo
-- CMP_CategoryId = 1 (ver 13.MigrateCampingCategoryToGuid.sql).
-- ============================================================

IF NOT EXISTS (SELECT 1 FROM [dbo].[T_CATEGORIES] WHERE [CAT_Name] = N'Familiar')
	INSERT INTO [dbo].[T_CATEGORIES] ([CAT_IdCategory], [CAT_Name])
	VALUES (N'B1000001-0000-0000-0000-000000000001', N'Familiar');
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[T_CATEGORIES] WHERE [CAT_Name] = N'Playa / Costero')
	INSERT INTO [dbo].[T_CATEGORIES] ([CAT_IdCategory], [CAT_Name])
	VALUES (N'B1000001-0000-0000-0000-000000000002', N'Playa / Costero');
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[T_CATEGORIES] WHERE [CAT_Name] = N'Montaña')
	INSERT INTO [dbo].[T_CATEGORIES] ([CAT_IdCategory], [CAT_Name])
	VALUES (N'B1000001-0000-0000-0000-000000000003', N'Montaña');
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[T_CATEGORIES] WHERE [CAT_Name] = N'Rural / Naturaleza')
	INSERT INTO [dbo].[T_CATEGORIES] ([CAT_IdCategory], [CAT_Name])
	VALUES (N'B1000001-0000-0000-0000-000000000004', N'Rural / Naturaleza');
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[T_CATEGORIES] WHERE [CAT_Name] = N'Lago / Río')
	INSERT INTO [dbo].[T_CATEGORIES] ([CAT_IdCategory], [CAT_Name])
	VALUES (N'B1000001-0000-0000-0000-000000000005', N'Lago / Río');
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[T_CATEGORIES] WHERE [CAT_Name] = N'Glamping / Lujo')
	INSERT INTO [dbo].[T_CATEGORIES] ([CAT_IdCategory], [CAT_Name])
	VALUES (N'B1000001-0000-0000-0000-000000000006', N'Glamping / Lujo');
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[T_CATEGORIES] WHERE [CAT_Name] = N'Aventura / Deportivo')
	INSERT INTO [dbo].[T_CATEGORIES] ([CAT_IdCategory], [CAT_Name])
	VALUES (N'B1000001-0000-0000-0000-000000000007', N'Aventura / Deportivo');
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[T_CATEGORIES] WHERE [CAT_Name] = N'Tranquilo / Relax')
	INSERT INTO [dbo].[T_CATEGORIES] ([CAT_IdCategory], [CAT_Name])
	VALUES (N'B1000001-0000-0000-0000-000000000008', N'Tranquilo / Relax');
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[T_CATEGORIES] WHERE [CAT_Name] = N'Urbano / City')
	INSERT INTO [dbo].[T_CATEGORIES] ([CAT_IdCategory], [CAT_Name])
	VALUES (N'B1000001-0000-0000-0000-000000000009', N'Urbano / City');
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[T_CATEGORIES] WHERE [CAT_Name] = N'Naturista')
	INSERT INTO [dbo].[T_CATEGORIES] ([CAT_IdCategory], [CAT_Name])
	VALUES (N'B1000001-0000-0000-0000-000000000010', N'Naturista');
GO
