USE [master]
GO
/****** Object:  Database [ServiceEPSI_V8_TEST]    Script Date: 22/08/2019 14:05:30 ******/
CREATE DATABASE [ServiceEPSI_V8_TEST]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'ServiceEPSI_V8_TEST', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\ServiceEPSI_V8_TEST.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'ServiceEPSI_V8_TEST_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\ServiceEPSI_V8_TEST_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ServiceEPSI_V8_TEST].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET ARITHABORT OFF 
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET  DISABLE_BROKER 
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET RECOVERY FULL 
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET  MULTI_USER 
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET DB_CHAINING OFF 
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'ServiceEPSI_V8_TEST', N'ON'
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET QUERY_STORE = OFF
GO
USE [ServiceEPSI_V8_TEST]
GO
/****** Object:  Table [dbo].[DatabaseDB]    Script Date: 22/08/2019 14:05:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DatabaseDB](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ServerId] [int] NOT NULL,
	[NomBD] [varchar](50) NOT NULL,
	[DateCreation] [datetime] NULL,
	[Commentaire] [varchar](max) NULL,
 CONSTRAINT [PK_DatabaseDB] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DatabaseGroupUser]    Script Date: 22/08/2019 14:05:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DatabaseGroupUser](
	[DbId] [int] NOT NULL,
	[SqlLogin] [varchar](30) NOT NULL,
	[UserLogin] [varchar](30) NULL,
	[UserFullName] [varchar](60) NULL,
	[GroupType] [int] NOT NULL,
	[AddedByUserLogin] [varchar](30) NOT NULL,
 CONSTRAINT [PK_DatabaseGroupUser] PRIMARY KEY CLUSTERED 
(
	[DbId] ASC,
	[SqlLogin] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DatabaseServerName]    Script Date: 22/08/2019 14:05:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DatabaseServerName](
	[Id] [int] NOT NULL,
	[Code] [char](15) NULL,
	[Name] [varchar](30) NOT NULL,
	[IPLocale] [varchar](20) NOT NULL,
	[NomDNS] [varchar](50) NOT NULL,
	[Description] [varchar](200) NULL,
	[CanAddDatabase] [int] NOT NULL,
 CONSTRAINT [PK_DatabaseServerName] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DatabaseServerUser]    Script Date: 22/08/2019 14:05:30 ******/
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

INSERT [dbo].[DatabaseDB] ([Id], [ServerId], [NomBD], [DateCreation], [Commentaire]) VALUES (1, 1, N'MySQL Test', CAST(N'2018-12-18T00:00:00.000' AS DateTime), N'UpdateDatabase depuis les Tests Unitaires')
INSERT [dbo].[DatabaseDB] ([Id], [ServerId], [NomBD], [DateCreation], [Commentaire]) VALUES (9, 2, N'DBTest2', CAST(N'2018-12-21T00:00:00.000' AS DateTime), N'Aucun')
INSERT [dbo].[DatabaseDB] ([Id], [ServerId], [NomBD], [DateCreation], [Commentaire]) VALUES (11, 3, N'DBTest', CAST(N'2018-12-21T00:00:00.000' AS DateTime), N'Pour les tests')
INSERT [dbo].[DatabaseDB] ([Id], [ServerId], [NomBD], [DateCreation], [Commentaire]) VALUES (12, 4, N'DBUbunitTest', CAST(N'2018-12-21T15:18:54.230' AS DateTime), N'Depuis les Tests Unitaires')
INSERT [dbo].[DatabaseDB] ([Id], [ServerId], [NomBD], [DateCreation], [Commentaire]) VALUES (14, 1, N'test20190922', CAST(N'2019-08-22T13:57:01.533' AS DateTime), NULL)
SET IDENTITY_INSERT [dbo].[DatabaseDB] OFF
INSERT [dbo].[DatabaseGroupUser] ([DbId], [SqlLogin], [UserLogin], [UserFullName], [GroupType], [AddedByUserLogin]) VALUES (1, N'ajout', NULL, NULL, 3, N'test.v8')
INSERT [dbo].[DatabaseGroupUser] ([DbId], [SqlLogin], [UserLogin], [UserFullName], [GroupType], [AddedByUserLogin]) VALUES (1, N'ajoutTestUnit', NULL, NULL, 3, N'test.v8')
INSERT [dbo].[DatabaseGroupUser] ([DbId], [SqlLogin], [UserLogin], [UserFullName], [GroupType], [AddedByUserLogin]) VALUES (1, N'test.v8', N'test.v8', N'V8 Test', 1, N'test.v8')
INSERT [dbo].[DatabaseGroupUser] ([DbId], [SqlLogin], [UserLogin], [UserFullName], [GroupType], [AddedByUserLogin]) VALUES (11, N'ajoutTestUnit', NULL, NULL, 3, N'test.v8')
INSERT [dbo].[DatabaseGroupUser] ([DbId], [SqlLogin], [UserLogin], [UserFullName], [GroupType], [AddedByUserLogin]) VALUES (11, N'test.v8', N'test.v8', N'V8 Test', 1, N'test.v8')
INSERT [dbo].[DatabaseGroupUser] ([DbId], [SqlLogin], [UserLogin], [UserFullName], [GroupType], [AddedByUserLogin]) VALUES (12, N'test.v8', N'test.v8', N'V8 Test', 1, N'test.v8')
INSERT [dbo].[DatabaseGroupUser] ([DbId], [SqlLogin], [UserLogin], [UserFullName], [GroupType], [AddedByUserLogin]) VALUES (14, N'test.v8', N'Test.V8', N'V8 Test', 1, N'Test.V8')
INSERT [dbo].[DatabaseServerName] ([Id], [Code], [Name], [IPLocale], [NomDNS], [Description], [CanAddDatabase]) VALUES (0, N'TEST           ', N'ServerTest', N'0.0.0.0', N'aucun', N'Utilisé pour les tests', 1)
INSERT [dbo].[DatabaseServerName] ([Id], [Code], [Name], [IPLocale], [NomDNS], [Description], [CanAddDatabase]) VALUES (1, N'MYSQL          ', N'My SQL 1', N'192.168.100.13', N'mysql.montpellier.epsi.fr', NULL, 1)
INSERT [dbo].[DatabaseServerName] ([Id], [Code], [Name], [IPLocale], [NomDNS], [Description], [CanAddDatabase]) VALUES (2, N'SQLSERVER      ', N'SQL Server 1', N'192.168.100.12', N'sqlserverl.montpellier.epsi.fr', NULL, 1)
INSERT [dbo].[DatabaseServerName] ([Id], [Code], [Name], [IPLocale], [NomDNS], [Description], [CanAddDatabase]) VALUES (3, N'ORACLE         ', N'Oracle', N'192.168.100.16', N'oraclel.montpellier.epsi.fr', NULL, 0)
INSERT [dbo].[DatabaseServerName] ([Id], [Code], [Name], [IPLocale], [NomDNS], [Description], [CanAddDatabase]) VALUES (4, N'SQLSERVER      ', N'SQL Server 2', N'192.168.100.161', N'aucun', NULL, 1)
INSERT [dbo].[DatabaseServerName] ([Id], [Code], [Name], [IPLocale], [NomDNS], [Description], [CanAddDatabase]) VALUES (5, N'ORACLE         ', N'Oracle 2', N'192.168.100.161', N'aucun', NULL, 0)
INSERT [dbo].[DatabaseServerUser] ([ServerId], [SqlLogin], [UserLogin]) VALUES (0, N'test.v2', N'test.v2')
INSERT [dbo].[DatabaseServerUser] ([ServerId], [SqlLogin], [UserLogin]) VALUES (1, N'test.v2', N'test.v2')
INSERT [dbo].[DatabaseServerUser] ([ServerId], [SqlLogin], [UserLogin]) VALUES (1, N'test.v8', N'Test.V8')
INSERT [dbo].[DatabaseServerUser] ([ServerId], [SqlLogin], [UserLogin]) VALUES (4, N'test.v2', N'test.v2')
INSERT [dbo].[DatabaseServerUser] ([ServerId], [SqlLogin], [UserLogin]) VALUES (4, N'test.v5', N'test.v5')
INSERT [dbo].[DatabaseServerUser] ([ServerId], [SqlLogin], [UserLogin]) VALUES (4, N'test.v8', N'test.v8')
ALTER TABLE [dbo].[DatabaseServerName] ADD  DEFAULT ((1)) FOR [CanAddDatabase]
GO
ALTER TABLE [dbo].[DatabaseDB]  WITH CHECK ADD  CONSTRAINT [FK_DatabaseDB_DatabaseServerName] FOREIGN KEY([ServerId])
REFERENCES [dbo].[DatabaseServerName] ([Id])
GO
ALTER TABLE [dbo].[DatabaseDB] CHECK CONSTRAINT [FK_DatabaseDB_DatabaseServerName]
GO
ALTER TABLE [dbo].[DatabaseGroupUser]  WITH CHECK ADD  CONSTRAINT [FK_DatabaseGroupUser_DatabaseDB] FOREIGN KEY([DbId])
REFERENCES [dbo].[DatabaseDB] ([Id])
GO
ALTER TABLE [dbo].[DatabaseGroupUser] CHECK CONSTRAINT [FK_DatabaseGroupUser_DatabaseDB]
GO
ALTER TABLE [dbo].[DatabaseServerUser]  WITH CHECK ADD  CONSTRAINT [FK_DatabaseServerUser_DatabaseServerName] FOREIGN KEY([ServerId])
REFERENCES [dbo].[DatabaseServerName] ([Id])
GO
ALTER TABLE [dbo].[DatabaseServerUser] CHECK CONSTRAINT [FK_DatabaseServerUser_DatabaseServerName]
GO
USE [master]
GO
ALTER DATABASE [ServiceEPSI_V8_TEST] SET  READ_WRITE 
GO
