USE [master]
GO
/****** Object:  Database [HOGentGraslanden]    Script Date: 21/04/2026 13:52:30 ******/
CREATE DATABASE [HOGentGraslanden]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'HOGentGraslanden', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\HOGentGraslanden.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'HOGentGraslanden_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\HOGentGraslanden_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [HOGentGraslanden] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [HOGentGraslanden].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [HOGentGraslanden] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [HOGentGraslanden] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [HOGentGraslanden] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [HOGentGraslanden] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [HOGentGraslanden] SET ARITHABORT OFF 
GO
ALTER DATABASE [HOGentGraslanden] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [HOGentGraslanden] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [HOGentGraslanden] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [HOGentGraslanden] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [HOGentGraslanden] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [HOGentGraslanden] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [HOGentGraslanden] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [HOGentGraslanden] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [HOGentGraslanden] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [HOGentGraslanden] SET  DISABLE_BROKER 
GO
ALTER DATABASE [HOGentGraslanden] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [HOGentGraslanden] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [HOGentGraslanden] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [HOGentGraslanden] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [HOGentGraslanden] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [HOGentGraslanden] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [HOGentGraslanden] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [HOGentGraslanden] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [HOGentGraslanden] SET  MULTI_USER 
GO
ALTER DATABASE [HOGentGraslanden] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [HOGentGraslanden] SET DB_CHAINING OFF 
GO
ALTER DATABASE [HOGentGraslanden] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [HOGentGraslanden] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [HOGentGraslanden] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [HOGentGraslanden] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [HOGentGraslanden] SET QUERY_STORE = ON
GO
ALTER DATABASE [HOGentGraslanden] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [HOGentGraslanden]
GO
/****** Object:  Table [dbo].[grass_plot]    Script Date: 21/04/2026 13:52:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[grass_plot](
	[code] [varchar](50) NOT NULL,
	[campus] [varchar](200) NOT NULL,
	[area_sq_meter] [float] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[inventoried_plot]    Script Date: 21/04/2026 13:52:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[inventoried_plot](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[inventory_id] [int] NOT NULL,
	[plot_code] [varchar](50) NOT NULL,
	[management_type] [int] NOT NULL,
	[plot_type] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[inventory]    Script Date: 21/04/2026 13:52:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[inventory](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[date] [datetime2](7) NOT NULL,
	[name] [nvarchar](500) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[management_type]    Script Date: 21/04/2026 13:52:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[management_type](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[type] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[measurement]    Script Date: 21/04/2026 13:52:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[measurement](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[inventoried_plot_id] [int] NOT NULL,
	[species_id] [int] NOT NULL,
	[coverage] [nvarchar](50) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[species]    Script Date: 21/04/2026 13:52:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[species](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](500) NOT NULL,
	[rating] [nvarchar](50) NOT NULL,
	[moisture] [int] NOT NULL,
	[ph] [int] NOT NULL,
	[nitrogen] [int] NOT NULL,
	[nectar_production] [int] NULL,
	[biodiversity_relevance] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE TABLE message (
	id int identity(1,1),
	object_id int,
	object_type nvarchar(50),
	inventory_id int,
	description nvarchar(500) not null,
	is_solved bit DEFAULT 0 not null,
	message_type nvarchar(50) not null,
	CONSTRAINT pk_message PRIMARY KEY (id)
)
GO
/****** Object:  Index [index_campus]    Script Date: 21/04/2026 13:52:31 ******/
CREATE NONCLUSTERED INDEX [index_campus] ON [dbo].[grass_plot]
(
	[campus] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[inventoried_plot]  WITH CHECK ADD  CONSTRAINT [fk_inventory_id] FOREIGN KEY([inventory_id])
REFERENCES [dbo].[inventory] ([id])
GO
ALTER TABLE [dbo].[inventoried_plot] CHECK CONSTRAINT [fk_inventory_id]
GO
ALTER TABLE [dbo].[inventoried_plot]  WITH CHECK ADD  CONSTRAINT [fk_management_type] FOREIGN KEY([management_type])
REFERENCES [dbo].[management_type] ([id])
GO
ALTER TABLE [dbo].[inventoried_plot] CHECK CONSTRAINT [fk_management_type]
GO
ALTER TABLE [dbo].[inventoried_plot]  WITH CHECK ADD  CONSTRAINT [fk_plot_code] FOREIGN KEY([plot_code])
REFERENCES [dbo].[grass_plot] ([code])
GO
ALTER TABLE [dbo].[inventoried_plot] CHECK CONSTRAINT [fk_plot_code]
GO
ALTER TABLE [dbo].[measurement]  WITH CHECK ADD  CONSTRAINT [fk_inventoried_plot_id] FOREIGN KEY([inventoried_plot_id])
REFERENCES [dbo].[inventoried_plot] ([id])
GO
ALTER TABLE [dbo].[measurement] CHECK CONSTRAINT [fk_inventoried_plot_id]
GO
ALTER TABLE [dbo].[measurement]  WITH CHECK ADD  CONSTRAINT [fk_species_id] FOREIGN KEY([species_id])
REFERENCES [dbo].[species] ([id])
GO
ALTER TABLE [dbo].[measurement] CHECK CONSTRAINT [fk_species_id]
GO

INSERT INTO [dbo].[management_type] VALUES ('Intensief'),('Extensief'),('Netheidsboord'),('Schapenweide')
GO

USE [master]
GO
ALTER DATABASE [HOGentGraslanden] SET  READ_WRITE 
GO

