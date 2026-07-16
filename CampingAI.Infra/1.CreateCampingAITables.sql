USE [CAMPING_AI_DB]
GO

-- ============================================================
-- Fase 3 — Tablas de CampingAI
-- ============================================================

-- ------------------------------------------------------------
-- T_USERS
-- ------------------------------------------------------------
-- USR_RoleId (catálogo de roles, validado en el dominio vía RoleVO):
--   1  = Sistema
--   2  = Gestor
--   3  = Usuario común (rol por defecto)
--   99 = Admin
CREATE TABLE [dbo].[T_USERS](
	[USR_IdUser]         [uniqueidentifier] NOT NULL,
	[USR_Email]          [nvarchar](255)    NOT NULL,
	[USR_PasswordHashed] [nvarchar](255)    NOT NULL,
	[USR_Name]           [nvarchar](255)    NULL,
	[USR_RoleId]         [int]              NOT NULL,
	[USR_ManagerStatus]  [int]              NOT NULL CONSTRAINT [DF_T_USERS_ManagerStatus] DEFAULT (0),
	[USR_CreatedOn]      [datetime2](7)  NOT NULL CONSTRAINT [DF_T_USERS_CreatedOn] DEFAULT (getutcdate()),
	[USR_UpdatedOn]      [datetime2](7)  NOT NULL CONSTRAINT [DF_T_USERS_UpdatedOn] DEFAULT (getutcdate()),
	[USR_DeletedOn]      [datetime2](7)  NULL,
	CONSTRAINT [PK_T_USERS] PRIMARY KEY CLUSTERED ([USR_IdUser] ASC)
) ON [PRIMARY]
GO

-- ------------------------------------------------------------
-- T_CAMPINGS
-- ------------------------------------------------------------
CREATE TABLE [dbo].[T_CAMPINGS](
	[CMP_IdCamping]    [uniqueidentifier] NOT NULL,
	[CMP_Name]         [nvarchar](255)    NOT NULL,
	[CMP_Description]  [nvarchar](2000)   NOT NULL,
	[CMP_Latitude]     [decimal](9, 6)    NOT NULL,
	[CMP_Longitude]    [decimal](9, 6)    NOT NULL,
	[CMP_PricePerNight][decimal](10, 2)   NOT NULL,
	[CMP_OwnerId]      [uniqueidentifier] NOT NULL,
	[CMP_CategoryId]   [int]            NOT NULL,
	[CMP_CreatedOn]    [datetime2](7)   NOT NULL CONSTRAINT [DF_T_CAMPINGS_CreatedOn] DEFAULT (getutcdate()),
	[CMP_UpdatedOn]    [datetime2](7)   NOT NULL CONSTRAINT [DF_T_CAMPINGS_UpdatedOn] DEFAULT (getutcdate()),
	[CMP_DeletedOn]    [datetime2](7)   NULL,
	CONSTRAINT [PK_T_CAMPINGS] PRIMARY KEY CLUSTERED ([CMP_IdCamping] ASC)
) ON [PRIMARY]
GO

-- ------------------------------------------------------------
-- T_RESERVATIONS
-- ------------------------------------------------------------
CREATE TABLE [dbo].[T_RESERVATIONS](
	[RES_IdReservation] [uniqueidentifier] NOT NULL,
	[RES_UserId]        [uniqueidentifier] NOT NULL,
	[RES_CampingId]     [uniqueidentifier] NOT NULL,
	[RES_CheckIn]       [datetime2](7)   NOT NULL,
	[RES_CheckOut]      [datetime2](7)   NOT NULL,
	[RES_TotalPrice]    [decimal](10, 2) NOT NULL,
	[RES_StatusId]      [int]            NOT NULL,
	[RES_CreatedOn]     [datetime2](7)   NOT NULL CONSTRAINT [DF_T_RESERVATIONS_CreatedOn] DEFAULT (getutcdate()),
	[RES_UpdatedOn]     [datetime2](7)   NOT NULL CONSTRAINT [DF_T_RESERVATIONS_UpdatedOn] DEFAULT (getutcdate()),
	[RES_DeletedOn]     [datetime2](7)   NULL,
	CONSTRAINT [PK_T_RESERVATIONS] PRIMARY KEY CLUSTERED ([RES_IdReservation] ASC)
) ON [PRIMARY]
GO

-- ------------------------------------------------------------
-- T_FACILITIES
-- ------------------------------------------------------------
CREATE TABLE [dbo].[T_FACILITIES](
	[FAC_IdFacility] [uniqueidentifier] NOT NULL,
	[FAC_Name]       [nvarchar](255)    NOT NULL,
	CONSTRAINT [PK_T_FACILITIES] PRIMARY KEY CLUSTERED ([FAC_IdFacility] ASC)
) ON [PRIMARY]
GO

-- ------------------------------------------------------------
-- T_CAMPING_FACILITIES  (tabla puente Camping <-> Facility)
-- ------------------------------------------------------------
CREATE TABLE [dbo].[T_CAMPING_FACILITIES](
	[CFE_IdCampingFacility] [uniqueidentifier] NOT NULL,
	[CFE_CampingId]         [uniqueidentifier] NOT NULL,
	[CFE_FacilityId]        [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_T_CAMPING_FACILITIES] PRIMARY KEY CLUSTERED ([CFE_IdCampingFacility] ASC),
	CONSTRAINT [FK_CAMPING_FACILITIES_CAMPING]  FOREIGN KEY ([CFE_CampingId])  REFERENCES [dbo].[T_CAMPINGS]  ([CMP_IdCamping]),
	CONSTRAINT [FK_CAMPING_FACILITIES_FACILITY] FOREIGN KEY ([CFE_FacilityId]) REFERENCES [dbo].[T_FACILITIES] ([FAC_IdFacility])
) ON [PRIMARY]
GO

-- ------------------------------------------------------------
-- T_FAVORITES
-- ------------------------------------------------------------
CREATE TABLE [dbo].[T_FAVORITES](
	[FAV_IdFavorite] [uniqueidentifier] NOT NULL,
	[FAV_UserId]     [uniqueidentifier] NOT NULL,
	[FAV_CampingId]  [uniqueidentifier] NOT NULL,
	[FAV_CreatedAt]  [datetime2](7) NOT NULL CONSTRAINT [DF_T_FAVORITES_CreatedAt] DEFAULT (getutcdate()),
	CONSTRAINT [PK_T_FAVORITES] PRIMARY KEY CLUSTERED ([FAV_IdFavorite] ASC)
) ON [PRIMARY]
GO
