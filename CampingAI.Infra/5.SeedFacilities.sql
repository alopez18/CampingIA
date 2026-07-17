USE [CAMPING_AI_DB]
GO

-- ============================================================
-- Fase 6 — Seed: instalaciones base de T_FACILITIES
-- ============================================================
-- Script idempotente: inserta solo si el nombre no existe.
-- "Toboganes en piscina" ya fue insertado en 2.AlterCampingsAddFilters.sql
-- y se omite aquí para evitar duplicados.
-- ============================================================

-- Ocio acuático
IF NOT EXISTS (SELECT 1 FROM [dbo].[T_FACILITIES] WHERE [FAC_Name] = N'Piscina')
	INSERT INTO [dbo].[T_FACILITIES] ([FAC_IdFacility], [FAC_Name])
	VALUES (N'A1000001-0000-0000-0000-000000000001', N'Piscina');
GO

-- Servicios básicos
IF NOT EXISTS (SELECT 1 FROM [dbo].[T_FACILITIES] WHERE [FAC_Name] = N'Zona de duchas')
	INSERT INTO [dbo].[T_FACILITIES] ([FAC_IdFacility], [FAC_Name])
	VALUES (N'A1000001-0000-0000-0000-000000000002', N'Zona de duchas');
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[T_FACILITIES] WHERE [FAC_Name] = N'Baños / WC')
	INSERT INTO [dbo].[T_FACILITIES] ([FAC_IdFacility], [FAC_Name])
	VALUES (N'A1000001-0000-0000-0000-000000000003', N'Baños / WC');
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[T_FACILITIES] WHERE [FAC_Name] = N'Lavandería')
	INSERT INTO [dbo].[T_FACILITIES] ([FAC_IdFacility], [FAC_Name])
	VALUES (N'A1000001-0000-0000-0000-000000000004', N'Lavandería');
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[T_FACILITIES] WHERE [FAC_Name] = N'Recepción 24h')
	INSERT INTO [dbo].[T_FACILITIES] ([FAC_IdFacility], [FAC_Name])
	VALUES (N'A1000001-0000-0000-0000-000000000005', N'Recepción 24h');
GO

-- Conectividad
IF NOT EXISTS (SELECT 1 FROM [dbo].[T_FACILITIES] WHERE [FAC_Name] = N'Wifi gratuito')
	INSERT INTO [dbo].[T_FACILITIES] ([FAC_IdFacility], [FAC_Name])
	VALUES (N'A1000001-0000-0000-0000-000000000006', N'Wifi gratuito');
GO

-- Comercios y restauración
IF NOT EXISTS (SELECT 1 FROM [dbo].[T_FACILITIES] WHERE [FAC_Name] = N'Supermercado / tienda')
	INSERT INTO [dbo].[T_FACILITIES] ([FAC_IdFacility], [FAC_Name])
	VALUES (N'A1000001-0000-0000-0000-000000000007', N'Supermercado / tienda');
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[T_FACILITIES] WHERE [FAC_Name] = N'Bar / Cafetería')
	INSERT INTO [dbo].[T_FACILITIES] ([FAC_IdFacility], [FAC_Name])
	VALUES (N'A1000001-0000-0000-0000-000000000008', N'Bar / Cafetería');
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[T_FACILITIES] WHERE [FAC_Name] = N'Restaurante')
	INSERT INTO [dbo].[T_FACILITIES] ([FAC_IdFacility], [FAC_Name])
	VALUES (N'A1000001-0000-0000-0000-000000000009', N'Restaurante');
GO

-- Ocio
IF NOT EXISTS (SELECT 1 FROM [dbo].[T_FACILITIES] WHERE [FAC_Name] = N'Zona de barbacoa')
	INSERT INTO [dbo].[T_FACILITIES] ([FAC_IdFacility], [FAC_Name])
	VALUES (N'A1000001-0000-0000-0000-000000000010', N'Zona de barbacoa');
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[T_FACILITIES] WHERE [FAC_Name] = N'Parque infantil')
	INSERT INTO [dbo].[T_FACILITIES] ([FAC_IdFacility], [FAC_Name])
	VALUES (N'A1000001-0000-0000-0000-000000000011', N'Parque infantil');
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[T_FACILITIES] WHERE [FAC_Name] = N'Animación y actividades')
	INSERT INTO [dbo].[T_FACILITIES] ([FAC_IdFacility], [FAC_Name])
	VALUES (N'A1000001-0000-0000-0000-000000000012', N'Animación y actividades');
GO

-- Deporte
IF NOT EXISTS (SELECT 1 FROM [dbo].[T_FACILITIES] WHERE [FAC_Name] = N'Pista deportiva')
	INSERT INTO [dbo].[T_FACILITIES] ([FAC_IdFacility], [FAC_Name])
	VALUES (N'A1000001-0000-0000-0000-000000000013', N'Pista deportiva');
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[T_FACILITIES] WHERE [FAC_Name] = N'Alquiler de bicicletas')
	INSERT INTO [dbo].[T_FACILITIES] ([FAC_IdFacility], [FAC_Name])
	VALUES (N'A1000001-0000-0000-0000-000000000014', N'Alquiler de bicicletas');
GO

-- Accesibilidad
IF NOT EXISTS (SELECT 1 FROM [dbo].[T_FACILITIES] WHERE [FAC_Name] = N'Acceso para discapacitados')
	INSERT INTO [dbo].[T_FACILITIES] ([FAC_IdFacility], [FAC_Name])
	VALUES (N'A1000001-0000-0000-0000-000000000015', N'Acceso para discapacitados');
GO

-- Mascotas
IF NOT EXISTS (SELECT 1 FROM [dbo].[T_FACILITIES] WHERE [FAC_Name] = N'Admite mascotas')
	INSERT INTO [dbo].[T_FACILITIES] ([FAC_IdFacility], [FAC_Name])
	VALUES (N'A1000001-0000-0000-0000-000000000016', N'Admite mascotas');
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[T_FACILITIES] WHERE [FAC_Name] = N'Zona de lavado para mascotas')
	INSERT INTO [dbo].[T_FACILITIES] ([FAC_IdFacility], [FAC_Name])
	VALUES (N'A1000001-0000-0000-0000-000000000017', N'Zona de lavado para mascotas');
GO

-- Transporte
IF NOT EXISTS (SELECT 1 FROM [dbo].[T_FACILITIES] WHERE [FAC_Name] = N'Aparcamiento')
	INSERT INTO [dbo].[T_FACILITIES] ([FAC_IdFacility], [FAC_Name])
	VALUES (N'A1000001-0000-0000-0000-000000000018', N'Aparcamiento');
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[T_FACILITIES] WHERE [FAC_Name] = N'Recarga para vehículos eléctricos')
	INSERT INTO [dbo].[T_FACILITIES] ([FAC_IdFacility], [FAC_Name])
	VALUES (N'A1000001-0000-0000-0000-000000000019', N'Recarga para vehículos eléctricos');
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[T_FACILITIES] WHERE [FAC_Name] = N'Toboganes en piscina')
	INSERT INTO [dbo].[T_FACILITIES] ([FAC_IdFacility], [FAC_Name])
	VALUES (N'A1000001-0000-0000-0000-000000000020', N'Toboganes en piscina');
GO
