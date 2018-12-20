USE [master]
GO
/****** Object:  Database [ServiceEPSI_V8]    Script Date: 18/12/2018 10:11:57 ******/
CREATE DATABASE [ServiceEPSI_V8]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'ServiceEPSI_V8', FILENAME = N'E:\SQLSERVER\MSSQL\Data\ServiceEPSI_V8.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'ServiceEPSI_V8_log', FILENAME = N'E:\SQLSERVER\MSSQL\Data\ServiceEPSI_V8_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [ServiceEPSI_V8] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ServiceEPSI_V8].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [ServiceEPSI_V8] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [ServiceEPSI_V8] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [ServiceEPSI_V8] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [ServiceEPSI_V8] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [ServiceEPSI_V8] SET ARITHABORT OFF 
GO
ALTER DATABASE [ServiceEPSI_V8] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [ServiceEPSI_V8] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [ServiceEPSI_V8] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [ServiceEPSI_V8] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [ServiceEPSI_V8] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [ServiceEPSI_V8] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [ServiceEPSI_V8] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [ServiceEPSI_V8] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [ServiceEPSI_V8] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [ServiceEPSI_V8] SET  DISABLE_BROKER 
GO
ALTER DATABASE [ServiceEPSI_V8] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [ServiceEPSI_V8] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [ServiceEPSI_V8] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [ServiceEPSI_V8] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [ServiceEPSI_V8] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [ServiceEPSI_V8] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [ServiceEPSI_V8] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [ServiceEPSI_V8] SET RECOVERY FULL 
GO
ALTER DATABASE [ServiceEPSI_V8] SET  MULTI_USER 
GO
ALTER DATABASE [ServiceEPSI_V8] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [ServiceEPSI_V8] SET DB_CHAINING OFF 
GO
ALTER DATABASE [ServiceEPSI_V8] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [ServiceEPSI_V8] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [ServiceEPSI_V8] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'ServiceEPSI_V8', N'ON'
GO
USE [ServiceEPSI_V8]
GO
/****** Object:  Table [dbo].[DatabaseDB]    Script Date: 18/12/2018 10:11:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DatabaseGroupUser]    Script Date: 18/12/2018 10:11:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DatabaseGroupUser](
	[DbId] [int] NOT NULL,
	[SqlLogin] [varchar](30) NOT NULL,
	[UserEpsi] [bit] NOT NULL,
	[GroupType] [int] NOT NULL,
 CONSTRAINT [PK_DatabaseGroupUser] PRIMARY KEY CLUSTERED 
(
	[DbId] ASC,
	[SqlLogin] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DatabaseServerName]    Script Date: 18/12/2018 10:11:58 ******/
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
/****** Object:  Table [dbo].[DatabaseServerType]    Script Date: 18/12/2018 10:11:58 ******/
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
/****** Object:  Table [dbo].[DatabaseServerUser]    Script Date: 18/12/2018 10:11:58 ******/
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
INSERT [dbo].[DatabaseServerUser] ([ServerId], [SqlLogin], [UserLogin]) VALUES (1, N'test.v6', N'test.v6')
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
ALTER DATABASE [ServiceEPSI_V8] SET  READ_WRITE 
GO
