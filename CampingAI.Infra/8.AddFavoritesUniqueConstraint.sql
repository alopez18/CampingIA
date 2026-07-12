-- ------------------------------------------------------------
-- Fase 7: Añade restricción de unicidad UserId+CampingId en T_FAVORITES
-- ------------------------------------------------------------
ALTER TABLE [dbo].[T_FAVORITES]
ADD CONSTRAINT [UQ_T_FAVORITES_UserId_CampingId]
UNIQUE ([FAV_UserId], [FAV_CampingId]);
GO
