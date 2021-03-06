USE [crud]
GO
/****** Object:  Table [dbo].[suscripcion]    Script Date: 14/5/2020 11:50:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[suscripcion](
	[idasociacion] [int] IDENTITY(1,1) NOT NULL,
	[idsuscriptor] [int] NOT NULL,
	[fechaalta] [datetime] NULL,
	[fechabaja] [datetime] NULL,
	[motivo] [nchar](10) NULL,
 CONSTRAINT [PK_suscripcion] PRIMARY KEY CLUSTERED 
(
	[idasociacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[suscriptor]    Script Date: 14/5/2020 11:50:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[suscriptor](
	[idsuscriptor] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [nvarchar](50) NULL,
	[apellido] [nvarchar](50) NULL,
	[numerodocumento] [int] NOT NULL,
	[mail] [nvarchar](50) NULL,
	[direccion] [nvarchar](50) NULL,
	[usuario] [nvarchar](50) NULL,
	[passowrd] [nvarchar](50) NULL,
	[tipodoc] [nvarchar](50) NULL,
	[telefono] [nvarchar](50) NULL,
 CONSTRAINT [PK_suscriptor] PRIMARY KEY CLUSTERED 
(
	[idsuscriptor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tipodocumento]    Script Date: 14/5/2020 11:50:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tipodocumento](
	[tipodoc] [nchar](10) NULL,
	[idtipo] [numeric](18, 0) NOT NULL,
 CONSTRAINT [PK_tipodocumento] PRIMARY KEY CLUSTERED 
(
	[idtipo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[suscripcion]  WITH CHECK ADD  CONSTRAINT [FK_suscripcion_suscriptor] FOREIGN KEY([idsuscriptor])
REFERENCES [dbo].[suscriptor] ([idsuscriptor])
GO
ALTER TABLE [dbo].[suscripcion] CHECK CONSTRAINT [FK_suscripcion_suscriptor]
GO
