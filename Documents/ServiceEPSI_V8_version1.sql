GO

/****** Object:  Table [dbo].[DatabaseDB]    Script Date: 21/12/2018 08:23:26 ******/
CREATE TABLE [dbo].[DatabaseDB](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ServerId] [int] NOT NULL,
	[NomBD] [varchar](50) NOT NULL,
	[UserLogin] [varchar](30) NOT NULL,
	[Nom] [varchar](30) NULL,
	[Prenom] [varchar](30) NULL,
	[DateCreation] [datetime] NULL,
	[Commentaire] [varchar](max) NULL,
	CONSTRAINT [PK_DatabaseDB] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
)
GO
/****** Object:  Table [dbo].[DatabaseGroupUser]    Script Date: 21/12/2018 08:23:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DatabaseGroupUser](
	[DbId] [int] NOT NULL,
	[SqlLogin] [varchar](30) NOT NULL,
	[UserLogin] [varchar](30) NOT NULL,,
	[GroupType] [int] NOT NULL,
 CONSTRAINT [PK_DatabaseGroupUser] PRIMARY KEY 
(
	[DbId] ASC,
	[SqlLogin] ASC
)
)
GO
/****** Object:  Table [dbo].[DatabaseServerName]    Script Date: 21/12/2018 08:23:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DatabaseServerName](
	[ServerId] [int] NOT NULL,
	[ServerTypeId] [int] NOT NULL,
	[Name] [varchar](30) NOT NULL,
	[IPLocale] [varchar](20) NOT NULL,
	[NomDNS] [varchar](50) NOT NULL,
	[Description] [varchar](200) NULL,
 CONSTRAINT [PK_DatabaseServerName] PRIMARY KEY CLUSTERED 
(
	[ServerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DatabaseServerType]    Script Date: 21/12/2018 08:23:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DatabaseServerType](
	[Id] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_DatabaseServerType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DatabaseServerUser]    Script Date: 21/12/2018 08:23:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DatabaseServerUser](
	[ServerId] [int] NOT NULL,
	[SqlLogin] [varchar](30) NOT NULL,
	[UserLogin] [varchar](30) NOT NULL,
 CONSTRAINT [PK_DatabaseUserServer] PRIMARY KEY CLUSTERED 
(
	[ServerId] ASC,
	[SqlLogin] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[DatabaseDB] ON 

INSERT [dbo].[DatabaseDB] ([Id], [ServerId], [NomBD], [UserLogin], [Nom], [Prenom], [DateCreation], [Commentaire]) VALUES (1, 0, N'DBTest', N'test.v8', N'V8', N'Test', CAST(N'2018-12-18T00:00:00.000' AS DateTime), N'Aucun')
INSERT [dbo].[DatabaseDB] ([Id], [ServerId], [NomBD], [UserLogin], [Nom], [Prenom], [DateCreation], [Commentaire]) VALUES (2, 0, N'DBTest2', N'test.v8', N'V8', N'Test', CAST(N'2018-12-18T16:59:03.297' AS DateTime), N'Aucun')
SET IDENTITY_INSERT [dbo].[DatabaseDB] OFF
INSERT [dbo].[DatabaseServerName] ([ServerId], [ServerTypeId], [Name], [IPLocale], [NomDNS], [Description]) VALUES (0, 0, N'ServerTest', N'0.0.0.0', N'aucun', N'Utilisé pour les tests')
INSERT [dbo].[DatabaseServerName] ([ServerId], [ServerTypeId], [Name], [IPLocale], [NomDNS], [Description]) VALUES (1, 1, N'MySQL 1', N'192.168.100.13', N'mysql.montpellier.epsi.fr', NULL)
INSERT [dbo].[DatabaseServerName] ([ServerId], [ServerTypeId], [Name], [IPLocale], [NomDNS], [Description]) VALUES (2, 2, N'SQL Server 1', N'192.168.100.12', N'sqlserverl.montpellier.epsi.fr', NULL)
INSERT [dbo].[DatabaseServerName] ([ServerId], [ServerTypeId], [Name], [IPLocale], [NomDNS], [Description]) VALUES (3, 3, N'Oracle', N'192.168.100.16', N'oraclel.montpellier.epsi.fr', NULL)
INSERT [dbo].[DatabaseServerName] ([ServerId], [ServerTypeId], [Name], [IPLocale], [NomDNS], [Description]) VALUES (4, 2, N'SQL Server 2', N'192.168.100.161', N'aucun', NULL)
INSERT [dbo].[DatabaseServerName] ([ServerId], [ServerTypeId], [Name], [IPLocale], [NomDNS], [Description]) VALUES (5, 3, N'Oracle 2', N'192.168.100.161', N'aucun', NULL)
INSERT [dbo].[DatabaseServerType] ([Id], [Name]) VALUES (0, N'TEST')
INSERT [dbo].[DatabaseServerType] ([Id], [Name]) VALUES (1, N'MYSQL')
INSERT [dbo].[DatabaseServerType] ([Id], [Name]) VALUES (2, N'SQLSERVER')
INSERT [dbo].[DatabaseServerType] ([Id], [Name]) VALUES (3, N'ORACLE')
INSERT [dbo].[DatabaseServerUser] ([ServerId], [SqlLogin], [UserLogin]) VALUES (0, N'test.v8', N'Test.V8')
ALTER TABLE [dbo].[DatabaseDB]  WITH CHECK ADD  CONSTRAINT [FK_DatabaseDB_DatabaseServerName] FOREIGN KEY([ServerId])
REFERENCES [dbo].[DatabaseServerName] ([ServerId])
GO
ALTER TABLE [dbo].[DatabaseDB] CHECK CONSTRAINT [FK_DatabaseDB_DatabaseServerName]
GO
ALTER TABLE [dbo].[DatabaseGroupUser]  WITH CHECK ADD  CONSTRAINT [FK_DatabaseGroupUser_DatabaseDB] FOREIGN KEY([DbId])
REFERENCES [dbo].[DatabaseDB] ([Id])
GO
ALTER TABLE [dbo].[DatabaseGroupUser] CHECK CONSTRAINT [FK_DatabaseGroupUser_DatabaseDB]
GO
ALTER TABLE [dbo].[DatabaseServerName]  WITH CHECK ADD  CONSTRAINT [FK_DatabaseServerName_DatabaseServerType] FOREIGN KEY([ServerTypeId])
REFERENCES [dbo].[DatabaseServerType] ([Id])
GO
ALTER TABLE [dbo].[DatabaseServerName] CHECK CONSTRAINT [FK_DatabaseServerName_DatabaseServerType]
GO
ALTER TABLE [dbo].[DatabaseServerUser]  WITH CHECK ADD  CONSTRAINT [FK_DatabaseServerUser_DatabaseServerName] FOREIGN KEY([ServerId])
REFERENCES [dbo].[DatabaseServerName] ([ServerId])
GO
ALTER TABLE [dbo].[DatabaseServerUser] CHECK CONSTRAINT [FK_DatabaseServerUser_DatabaseServerName]
GO
USE [master]
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET  READ_WRITE 
GO
