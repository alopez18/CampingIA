USE [CAMPING_AI_DB]
GO

-- ============================================================
-- Fase 6 — Añadir columnas de filtro a T_CAMPINGS
-- ============================================================

ALTER TABLE [dbo].[T_CAMPINGS]
	ADD [CMP_Provincia]          NVARCHAR(100) NULL;
GO

-- ============================================================
-- Fase 6 — Seed: instalación "Toboganes en piscina"
-- ============================================================

INSERT INTO [dbo].[T_FACILITIES] ([FAC_IdFacility], [FAC_Name])
VALUES (NEWID(), N'Toboganes en piscina');
GO
