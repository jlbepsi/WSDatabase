USE [master]
GO
/****** Object:  Database [ServiceEPSI_SQLServer]    Script Date: 26/09/2019 13:43:56 ******/
CREATE DATABASE [ServiceEPSI_SQLServer]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'ServiceEPSI_SQLServer', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\ServiceEPSI_SQLServer.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'ServiceEPSI_SQLServer_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\ServiceEPSI_SQLServer_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ServiceEPSI_SQLServer].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET ARITHABORT OFF 
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET  DISABLE_BROKER 
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET RECOVERY FULL 
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET  MULTI_USER 
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET DB_CHAINING OFF 
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'ServiceEPSI_SQLServer', N'ON'
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET QUERY_STORE = OFF
GO
USE [ServiceEPSI_SQLServer]
GO
/****** Object:  Table [dbo].[ConfigurationServer]    Script Date: 26/09/2019 13:43:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConfigurationServer](
	[Name] [nvarchar](200) NOT NULL,
	[Value] [nvarchar](200) NULL
) ON [PRIMARY]
GO
INSERT [dbo].[ConfigurationServer] ([Name], [Value]) VALUES (N'DBDataDirectory', N'E:\SQLServer\Data')
/****** Object:  StoredProcedure [dbo].[DatabaseAddContributor]    Script Date: 26/09/2019 13:43:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- **************************************
--
-- Insertion des utilisateurs
--

CREATE PROCEDURE [dbo].[DatabaseAddContributor] (@dbName VARCHAR(30), @login VARCHAR(30), @userRights VARCHAR(300))
AS
BEGIN
	DECLARE @sqlStatement nvarchar(400)
	DECLARE @spot SMALLINT, @aRole VARCHAR(50)

	IF EXISTS(SELECT name FROM sys.databases WHERE name = @dbName)
	BEGIN
		SET @sqlStatement = 'USE [' + @dbName + ']; CREATE USER [' + @login + '] FOR LOGIN [' + @login + '];'
		EXEC sp_executesql @sqlStatement

		
		SET @sqlStatement = 'USE [' + @dbName + ']; GRANT ' + @userRights + ' TO [' + @login + '];'
		EXEC(@sqlStatement) 
			
		SET @sqlStatement = 'USE [ServiceEPSI_SQLServer];'
		EXEC sp_executesql @sqlStatement
	END
END
GO
/****** Object:  StoredProcedure [dbo].[DatabaseAddOrUpdateContributorGroupType]    Script Date: 26/09/2019 13:43:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- **************************************
--
-- Modification des utilisateurs
-- 

CREATE PROCEDURE [dbo].[DatabaseAddOrUpdateContributorGroupType] (@dbName VARCHAR(30), @login VARCHAR(30), @userRights VARCHAR(300), @doUpdate INT)
AS
BEGIN
	DECLARE @sqlStatement nvarchar(400)
	DECLARE @spot SMALLINT, @aRole VARCHAR(50)

	IF EXISTS(SELECT name FROM sys.databases WHERE name = @dbName)
	BEGIN
		-- Supression des droits
		IF (@doUpdate = 1)
		BEGIN
			SET @sqlStatement = 'USE [' + @dbName + ']; REVOKE ALTER, DELETE, EXECUTE, INSERT, SELECT, UPDATE, VIEW DEFINITION TO [' + @login + ']'
			EXEC(@sqlStatement) 
		END
			
		-- Ajout des droits	
		SET @sqlStatement = 'USE [' + @dbName + ']; GRANT ' + @userRights + ' TO [' + @login + ']'
		EXEC(@sqlStatement) 
			
		SET @sqlStatement = 'USE [ServiceEPSI_SQLServer];'
		EXEC sp_executesql @sqlStatement
	END
END
GO
/****** Object:  StoredProcedure [dbo].[DatabaseAddOrUpdateUser]    Script Date: 26/09/2019 13:43:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DatabaseAddOrUpdateUser] (@login nvarchar(50), @password nvarchar(50))
AS
BEGIN
	DECLARE @SqlStatement nvarchar(4000)

	IF EXISTS(SELECT loginname, dbname FROM master.dbo.syslogins WHERE name = @login)
	BEGIN
		SET @sqlStatement = 'USE Master; ALTER LOGIN "' + @login + '" WITH PASSWORD = ''' + @password + ''''
		EXEC(@sqlStatement)
	END
	ELSE
	BEGIN
		SET @SqlStatement = 'CREATE LOGIN [' + @login + '] WITH PASSWORD = ''' + @password + ''''
		EXEC sp_executesql @SqlStatement
	END	
END
GO
/****** Object:  StoredProcedure [dbo].[DatabaseAddUser]    Script Date: 26/09/2019 13:43:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DatabaseAddUser] (@login nvarchar(50), @password nvarchar(50))
AS
BEGIN
	DECLARE @SqlStatement nvarchar(4000)

	IF NOT EXISTS(SELECT loginname, dbname FROM master.dbo.syslogins WHERE name = @login)
	BEGIN
		SET @SqlStatement = 'CREATE LOGIN [' + @login + '] WITH PASSWORD = ''' + @password + ''''
		EXEC sp_executesql @SqlStatement
	END	
END
GO
/****** Object:  StoredProcedure [dbo].[DatabaseCreateDatabase]    Script Date: 26/09/2019 13:43:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- **************************************
--
-- Base de données
-- 

CREATE PROCEDURE [dbo].[DatabaseCreateDatabase] 
@dbName nvarchar(50),
@login nvarchar(50)
AS
BEGIN
	DECLARE @SqlStatement nvarchar(400)
	DECLARE @dataDirectory nvarchar(200)

	IF NOT EXISTS(SELECT name FROM sys.databases WHERE name = @dbName)
	BEGIN
		-- Obtention du répertoire de données des base de données
		SELECT @dataDirectory = cs.value 
		FROM ConfigurationServer AS cs
		WHERE cs.name = 'DBDataDirectory'
		
		SET @SqlStatement = 'USE [master]; CREATE DATABASE [' + @dbName + '] ON PRIMARY (NAME=' + @dbName 
											+ ', FILENAME=''' + @dataDirectory + @dbName + 'data.mdf'', SIZE=5, MAXSIZE=300)'
		EXEC sp_executesql @SqlStatement
		
		-- Fixe le proprietaire et donc les droits
		SET @sqlStatement = 'ALTER AUTHORIZATION ON DATABASE::[' + @dbName + '] TO [' + @login + ']';
		EXEC sp_executesql @sqlStatement
		
		SET @sqlStatement = 'USE [ServiceEPSI_SQLServer];'
		EXEC sp_executesql @sqlStatement
	END
END
GO
/****** Object:  StoredProcedure [dbo].[DatabaseDeleteDatabase]    Script Date: 26/09/2019 13:43:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DatabaseDeleteDatabase]
	@dbName nvarchar(50)
AS
BEGIN
	DECLARE @SqlStatement nvarchar(400)
	SET NOCOUNT ON;

	SET @sqlStatement = 'USE [master]; DROP DATABASE [' + @dbName + ']'
	EXEC sp_executesql @SqlStatement
	
	SET @sqlStatement = 'USE [ServiceEPSI_SQLServer];'
	EXEC sp_executesql @sqlStatement
END
GO
/****** Object:  StoredProcedure [dbo].[DatabaseExistsDB]    Script Date: 26/09/2019 13:43:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- **************************************
--
-- Obtention des informations
-- 

CREATE PROCEDURE [dbo].[DatabaseExistsDB] (@dbName VARCHAR(30), @exists INT OUTPUT)
AS
BEGIN
	DECLARE @sqlStatement nvarchar(400)
	DECLARE @spot SMALLINT, @aRole VARCHAR(50)

	IF EXISTS(SELECT name FROM sys.databases WHERE name = @dbName)
	BEGIN
		SET @exists = 1
	END
	ELSE
	BEGIN
		SET @exists = 0
	END
END
GO
/****** Object:  StoredProcedure [dbo].[DatabaseExistsUser]    Script Date: 26/09/2019 13:43:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DatabaseExistsUser] (@login VARCHAR(30), @exists INT OUTPUT)
AS
BEGIN
	IF EXISTS(SELECT name FROM sys.syslogins WHERE name = @login)
	BEGIN
		SET @exists = 1
	END
	ELSE
	BEGIN
		SET @exists = 0
	END
END
GO
/****** Object:  StoredProcedure [dbo].[DatabaseExistsUserInDB]    Script Date: 26/09/2019 13:43:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DatabaseExistsUserInDB]
	@dbName VARCHAR(30), 
	@login VARCHAR(30),
	@exists INT OUTPUT
AS
BEGIN
	DECLARE @sqlStatement nvarchar(400)
	DECLARE @ParmDefinition NVARCHAR(500)

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	SET @sqlStatement = 'USE [' + @dbName + ']; SELECT @userCountOUT=COUNT(*) FROM sys.database_principals WHERE name = ''' + @login + ''''

	SET @ParmDefinition = '@userCountOUT varchar(30) OUTPUT'
	EXEC sp_executesql 
		@sqlStatement,
		@ParmDefinition,
		@userCountOUT=@exists OUTPUT
		
	SET @sqlStatement = 'USE master;'
	EXEC sp_executesql @sqlStatement
END
GO
/****** Object:  StoredProcedure [dbo].[DatabaseListDatabases]    Script Date: 26/09/2019 13:43:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DatabaseListDatabases] (@login VARCHAR(30))
AS
BEGIN
	SELECT TDB.name AS dbName
	FROM sys.databases AS TDB, sys.syslogins AS TUser
	WHERE TDB.owner_sid = TUser.sid
	AND TUser.name = @login
	ORDER BY dbName
END
GO
/****** Object:  StoredProcedure [dbo].[DatabaseListDatabaseUsers]    Script Date: 26/09/2019 13:43:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DatabaseListDatabaseUsers] (@dbName nvarchar(50))
AS
BEGIN
	DECLARE @SqlStatement nvarchar(4000)

	SET @SqlStatement = 'SELECT allUsers.loginname  FROM ' +
		@dbName + '.sys.database_principals AS localUsers, master.sys.syslogins AS allUsers '+
		'WHERE localUsers.sid = allUsers.sid ' +
		'AND localUsers.type IN (''S'', ''U'') '+
		'AND localUsers.sid IS NOT NULL'

	EXEC sp_executesql @SqlStatement
END
GO
/****** Object:  StoredProcedure [dbo].[DatabaseRemoveContributor]    Script Date: 26/09/2019 13:43:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DatabaseRemoveContributor] 
	@dbName VARCHAR(30), 
	@login VARCHAR(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @sqlStatement nvarchar(400)
	DECLARE @spot SMALLINT, @aRole VARCHAR(50)

	IF EXISTS(SELECT name FROM sys.databases WHERE name = @dbName)
	BEGIN
		SET @sqlStatement = 'USE [' + @dbName + ']; DROP USER "' + @login + '"'
		EXEC(@sqlStatement) 
	END
END
GO
/****** Object:  StoredProcedure [dbo].[DatabaseRemoveDatabase]    Script Date: 26/09/2019 13:43:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DatabaseRemoveDatabase]
	@dbName nvarchar(50)
AS
BEGIN
	DECLARE @SqlStatement nvarchar(400)
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SET @sqlStatement = 'USE [master]; DROP DATABASE [' + @dbName + ']'
	EXEC sp_executesql @SqlStatement
	
	SET @sqlStatement = 'USE [ServiceEPSI_SQLServer];'
	EXEC sp_executesql @sqlStatement
END
GO
/****** Object:  StoredProcedure [dbo].[DatabaseRemoveUser]    Script Date: 26/09/2019 13:43:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- **************************************
--
-- Suppression des utilisateurs
-- 

CREATE PROCEDURE [dbo].[DatabaseRemoveUser] (@login nvarchar(50))
AS
BEGIN
	DECLARE @SqlStatement nvarchar(4000)

	IF EXISTS(SELECT loginname, dbname FROM master.dbo.syslogins WHERE name = @login)
	BEGIN
		SET @SqlStatement = 'DROP LOGIN [' + @login + '] '
		EXEC sp_executesql @SqlStatement
	END	
END
GO
/****** Object:  StoredProcedure [dbo].[DatabaseUpdateContributorGroupType]    Script Date: 26/09/2019 13:43:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DatabaseUpdateContributorGroupType] (@dbName VARCHAR(30), @login VARCHAR(30), @userRights VARCHAR(300))
AS
BEGIN
	DECLARE @sqlStatement nvarchar(400)
	DECLARE @spot SMALLINT, @aRole VARCHAR(50)

	IF EXISTS(SELECT name FROM sys.databases WHERE name = @dbName)
	BEGIN
		-- Supression des droits
		SET @sqlStatement = 'USE [' + @dbName + ']; REVOKE ALTER, DELETE, EXECUTE, INSERT, SELECT, UPDATE, VIEW DEFINITION TO [' + @login + ']'
		EXEC(@sqlStatement) 	
		-- Ajout des droits	
		SET @sqlStatement = 'USE [' + @dbName + ']; GRANT ' + @userRights + ' TO [' + @login + ']'
		EXEC(@sqlStatement) 
			
		SET @sqlStatement = 'USE [ServiceEPSI_SQLServer];'
		EXEC sp_executesql @sqlStatement
	END
END
GO
/****** Object:  StoredProcedure [dbo].[DatabaseUpdateContributorPassword]    Script Date: 26/09/2019 13:43:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DatabaseUpdateContributorPassword] 
	@dbName VARCHAR(30), 
	@login VARCHAR(30), 
	@password VARCHAR(100),
	@userUpdated INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @sqlStatement nvarchar(400)
	SET @userUpdated = 0

	IF EXISTS(SELECT name FROM sys.databases WHERE name = @dbName)
	BEGIN
		SET @sqlStatement = 'USE [' + @dbName + ']; ALTER LOGIN "' + @login + '" WITH PASSWORD = ''' + @password + ''''
		EXEC(@sqlStatement)
		SET @userUpdated = 1
	END
END
GO
USE [master]
GO
ALTER DATABASE [ServiceEPSI_SQLServer] SET  READ_WRITE 
GO
