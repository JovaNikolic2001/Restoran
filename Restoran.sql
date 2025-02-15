USE [Proba2]
GO
ALTER TABLE [dbo].[Usluga] DROP CONSTRAINT [FK_Usluga_Stavka]
GO
ALTER TABLE [dbo].[Usluga] DROP CONSTRAINT [FK_Usluga_Porudzbina]
GO
ALTER TABLE [dbo].[Porudzbina] DROP CONSTRAINT [FK_Porudzbina_Sto]
GO
ALTER TABLE [dbo].[Porudzbina] DROP CONSTRAINT [FK_Porudzbina_Osoblje]
GO
ALTER TABLE [dbo].[Osoblje] DROP CONSTRAINT [FK_Osoblje_Smene]
GO
/****** Object:  Table [dbo].[Usluga]    Script Date: 11.6.2024. 14:54:30 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Usluga]') AND type in (N'U'))
DROP TABLE [dbo].[Usluga]
GO
/****** Object:  Table [dbo].[Sto]    Script Date: 11.6.2024. 14:54:30 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Sto]') AND type in (N'U'))
DROP TABLE [dbo].[Sto]
GO
/****** Object:  Table [dbo].[Stavka]    Script Date: 11.6.2024. 14:54:30 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Stavka]') AND type in (N'U'))
DROP TABLE [dbo].[Stavka]
GO
/****** Object:  Table [dbo].[Smene]    Script Date: 11.6.2024. 14:54:30 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Smene]') AND type in (N'U'))
DROP TABLE [dbo].[Smene]
GO
/****** Object:  Table [dbo].[Porudzbina]    Script Date: 11.6.2024. 14:54:30 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Porudzbina]') AND type in (N'U'))
DROP TABLE [dbo].[Porudzbina]
GO
/****** Object:  Table [dbo].[Osoblje]    Script Date: 11.6.2024. 14:54:30 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Osoblje]') AND type in (N'U'))
DROP TABLE [dbo].[Osoblje]
GO
USE [master]
GO
/****** Object:  Database [Proba2]    Script Date: 11.6.2024. 14:54:30 ******/
DROP DATABASE [Proba2]
GO
/****** Object:  Database [Proba2]    Script Date: 11.6.2024. 14:54:30 ******/
CREATE DATABASE [Proba2]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Proba2', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\Proba2.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Proba2_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\Proba2_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [Proba2] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Proba2].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Proba2] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Proba2] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Proba2] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Proba2] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Proba2] SET ARITHABORT OFF 
GO
ALTER DATABASE [Proba2] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Proba2] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Proba2] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Proba2] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Proba2] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Proba2] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Proba2] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Proba2] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Proba2] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Proba2] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Proba2] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Proba2] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Proba2] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Proba2] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Proba2] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Proba2] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Proba2] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Proba2] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Proba2] SET  MULTI_USER 
GO
ALTER DATABASE [Proba2] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Proba2] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Proba2] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Proba2] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Proba2] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Proba2] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [Proba2] SET QUERY_STORE = ON
GO
ALTER DATABASE [Proba2] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [Proba2]
GO
/****** Object:  Table [dbo].[Osoblje]    Script Date: 11.6.2024. 14:54:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Osoblje](
	[OsobljeID] [int] NOT NULL,
	[Ime] [nvarchar](30) NOT NULL,
	[Prezime] [nvarchar](30) NOT NULL,
	[Uloga] [nvarchar](30) NOT NULL,
	[Kontakt] [nvarchar](15) NOT NULL,
	[Plata] [money] NOT NULL,
	[Admin] [bit] NOT NULL,
	[SmenaID] [int] NOT NULL,
 CONSTRAINT [PK_Osoblje] PRIMARY KEY CLUSTERED 
(
	[OsobljeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Porudzbina]    Script Date: 11.6.2024. 14:54:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Porudzbina](
	[PorudzbinaID] [int] NOT NULL,
	[StoID] [int] NOT NULL,
	[ZaposleniID] [int] NOT NULL,
	[VremePorudzbine] [smalldatetime] NOT NULL,
 CONSTRAINT [PK_Porudzbina] PRIMARY KEY CLUSTERED 
(
	[PorudzbinaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Smene]    Script Date: 11.6.2024. 14:54:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Smene](
	[SmenaID] [int] NOT NULL,
	[RadniDani] [int] NOT NULL,
	[StartSmene] [time](7) NOT NULL,
	[KrajSmene] [time](7) NOT NULL,
 CONSTRAINT [PK_Smene] PRIMARY KEY CLUSTERED 
(
	[SmenaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Stavka]    Script Date: 11.6.2024. 14:54:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Stavka](
	[StavkaID] [int] NOT NULL,
	[Naziv] [nvarchar](30) NOT NULL,
	[Opis] [nvarchar](30) NOT NULL,
	[Cena] [money] NOT NULL,
	[Kategorija] [nvarchar](30) NOT NULL,
 CONSTRAINT [PK_Stavka] PRIMARY KEY CLUSTERED 
(
	[StavkaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sto]    Script Date: 11.6.2024. 14:54:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sto](
	[StoID] [int] NOT NULL,
	[Kapacitet] [int] NOT NULL,
	[Lokacija] [nvarchar](30) NOT NULL,
	[Rezervisano] [bit] NOT NULL,
	[Napomena] [nvarchar](100) NULL,
 CONSTRAINT [PK_Sto] PRIMARY KEY CLUSTERED 
(
	[StoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Usluga]    Script Date: 11.6.2024. 14:54:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Usluga](
	[PorudzbinaID] [int] NOT NULL,
	[StavkaID] [int] NOT NULL,
	[Količina] [int] NOT NULL,
	[Ukupno] [money] NOT NULL
) ON [PRIMARY]
GO
INSERT [dbo].[Osoblje] ([OsobljeID], [Ime], [Prezime], [Uloga], [Kontakt], [Plata], [Admin], [SmenaID]) VALUES (1, N'Marko', N'Stajić', N'Menadžer', N'06501960324', 100000.0000, 1, 4)
INSERT [dbo].[Osoblje] ([OsobljeID], [Ime], [Prezime], [Uloga], [Kontakt], [Plata], [Admin], [SmenaID]) VALUES (2, N'Nikola', N'Jačmenica', N'Pomoćno osoblje', N'06049546153', 800000.0000, 0, 1)
INSERT [dbo].[Osoblje] ([OsobljeID], [Ime], [Prezime], [Uloga], [Kontakt], [Plata], [Admin], [SmenaID]) VALUES (3, N'Nikola', N'Filipović', N'Kuvar', N'06498912243', 120000.0000, 0, 2)
INSERT [dbo].[Osoblje] ([OsobljeID], [Ime], [Prezime], [Uloga], [Kontakt], [Plata], [Admin], [SmenaID]) VALUES (4, N'Stefan', N'Tadić', N'Kuvar', N'06397471691', 120000.0000, 0, 3)
INSERT [dbo].[Osoblje] ([OsobljeID], [Ime], [Prezime], [Uloga], [Kontakt], [Plata], [Admin], [SmenaID]) VALUES (5, N'Lazar', N'Jonović', N'Barmen', N'06198798354', 90000.0000, 1, 2)
INSERT [dbo].[Osoblje] ([OsobljeID], [Ime], [Prezime], [Uloga], [Kontakt], [Plata], [Admin], [SmenaID]) VALUES (6, N'Miroslav', N'Todorović', N'Konobar', N'06771654516', 70000.0000, 0, 1)
GO
INSERT [dbo].[Smene] ([SmenaID], [RadniDani], [StartSmene], [KrajSmene]) VALUES (1, 63, CAST(N'08:00:00' AS Time), CAST(N'16:00:00' AS Time))
INSERT [dbo].[Smene] ([SmenaID], [RadniDani], [StartSmene], [KrajSmene]) VALUES (2, 31, CAST(N'15:00:00' AS Time), CAST(N'23:00:00' AS Time))
INSERT [dbo].[Smene] ([SmenaID], [RadniDani], [StartSmene], [KrajSmene]) VALUES (3, 96, CAST(N'12:00:00' AS Time), CAST(N'21:00:00' AS Time))
INSERT [dbo].[Smene] ([SmenaID], [RadniDani], [StartSmene], [KrajSmene]) VALUES (4, 127, CAST(N'10:00:00' AS Time), CAST(N'23:00:00' AS Time))
GO
INSERT [dbo].[Stavka] ([StavkaID], [Naziv], [Opis], [Cena], [Kategorija]) VALUES (1, N'Cezar salata', N'Nema', 600.0000, N'Dodatno')
INSERT [dbo].[Stavka] ([StavkaID], [Naziv], [Opis], [Cena], [Kategorija]) VALUES (2, N'Bečka šnicla', N'Nema', 800.0000, N'Glavno jelo')
INSERT [dbo].[Stavka] ([StavkaID], [Naziv], [Opis], [Cena], [Kategorija]) VALUES (3, N'Kisela voda 0.5l', N'Nema', 150.0000, N'Piće')
GO
INSERT [dbo].[Sto] ([StoID], [Kapacitet], [Lokacija], [Rezervisano], [Napomena]) VALUES (1, 4, N'Restoran', 0, NULL)
INSERT [dbo].[Sto] ([StoID], [Kapacitet], [Lokacija], [Rezervisano], [Napomena]) VALUES (2, 6, N'Restoran', 0, NULL)
INSERT [dbo].[Sto] ([StoID], [Kapacitet], [Lokacija], [Rezervisano], [Napomena]) VALUES (3, 6, N'Restoran', 0, NULL)
INSERT [dbo].[Sto] ([StoID], [Kapacitet], [Lokacija], [Rezervisano], [Napomena]) VALUES (4, 2, N'Bar 1', 0, NULL)
INSERT [dbo].[Sto] ([StoID], [Kapacitet], [Lokacija], [Rezervisano], [Napomena]) VALUES (5, 2, N'Bar 2', 0, NULL)
INSERT [dbo].[Sto] ([StoID], [Kapacitet], [Lokacija], [Rezervisano], [Napomena]) VALUES (6, 2, N'Bar 3', 0, NULL)
INSERT [dbo].[Sto] ([StoID], [Kapacitet], [Lokacija], [Rezervisano], [Napomena]) VALUES (7, 6, N'Bašta', 0, NULL)
INSERT [dbo].[Sto] ([StoID], [Kapacitet], [Lokacija], [Rezervisano], [Napomena]) VALUES (8, 6, N'Bašta', 0, NULL)
INSERT [dbo].[Sto] ([StoID], [Kapacitet], [Lokacija], [Rezervisano], [Napomena]) VALUES (9, 4, N'Bašta', 0, NULL)
INSERT [dbo].[Sto] ([StoID], [Kapacitet], [Lokacija], [Rezervisano], [Napomena]) VALUES (10, 8, N'Bašta', 0, NULL)
GO
ALTER TABLE [dbo].[Osoblje]  WITH CHECK ADD  CONSTRAINT [FK_Osoblje_Smene] FOREIGN KEY([SmenaID])
REFERENCES [dbo].[Smene] ([SmenaID])
GO
ALTER TABLE [dbo].[Osoblje] CHECK CONSTRAINT [FK_Osoblje_Smene]
GO
ALTER TABLE [dbo].[Porudzbina]  WITH CHECK ADD  CONSTRAINT [FK_Porudzbina_Osoblje] FOREIGN KEY([ZaposleniID])
REFERENCES [dbo].[Osoblje] ([OsobljeID])
GO
ALTER TABLE [dbo].[Porudzbina] CHECK CONSTRAINT [FK_Porudzbina_Osoblje]
GO
ALTER TABLE [dbo].[Porudzbina]  WITH CHECK ADD  CONSTRAINT [FK_Porudzbina_Sto] FOREIGN KEY([StoID])
REFERENCES [dbo].[Sto] ([StoID])
GO
ALTER TABLE [dbo].[Porudzbina] CHECK CONSTRAINT [FK_Porudzbina_Sto]
GO
ALTER TABLE [dbo].[Usluga]  WITH CHECK ADD  CONSTRAINT [FK_Usluga_Porudzbina] FOREIGN KEY([PorudzbinaID])
REFERENCES [dbo].[Porudzbina] ([PorudzbinaID])
GO
ALTER TABLE [dbo].[Usluga] CHECK CONSTRAINT [FK_Usluga_Porudzbina]
GO
ALTER TABLE [dbo].[Usluga]  WITH CHECK ADD  CONSTRAINT [FK_Usluga_Stavka] FOREIGN KEY([StavkaID])
REFERENCES [dbo].[Stavka] ([StavkaID])
GO
ALTER TABLE [dbo].[Usluga] CHECK CONSTRAINT [FK_Usluga_Stavka]
GO
USE [master]
GO
ALTER DATABASE [Proba2] SET  READ_WRITE 
GO
