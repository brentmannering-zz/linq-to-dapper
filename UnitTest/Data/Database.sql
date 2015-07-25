USE [Test]
GO
/****** Object:  Table [dbo].[Field]    Script Date: 07/14/2015 11:10:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Field](
	[FieldId] [int] IDENTITY(1,1) NOT NULL,
	[DataTypeId] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Created] [datetime] NOT NULL,
 CONSTRAINT [PK_Field] PRIMARY KEY CLUSTERED 
(
	[FieldId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DataType]    Script Date: 07/14/2015 11:10:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DataType](
	[DataTypeId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
 CONSTRAINT [PK_DataType] PRIMARY KEY CLUSTERED 
(
	[DataTypeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[DataType] ON
INSERT [dbo].[DataType] ([DataTypeId], [Name], [IsActive], [Created]) VALUES (1, N'text', 1, CAST(0x0000A4CA0093F826 AS DateTime))
INSERT [dbo].[DataType] ([DataTypeId], [Name], [IsActive], [Created]) VALUES (2, N'int', 0, CAST(0x0000A4CA0093FD2A AS DateTime))
INSERT [dbo].[DataType] ([DataTypeId], [Name], [IsActive], [Created]) VALUES (3, N'number', 0, CAST(0x0000A4CA00940203 AS DateTime))
INSERT [dbo].[DataType] ([DataTypeId], [Name], [IsActive], [Created]) VALUES (4, N'date', 1, CAST(0x0000A4CA0094074E AS DateTime))
INSERT [dbo].[DataType] ([DataTypeId], [Name], [IsActive], [Created]) VALUES (5, N'checkbox', 0, CAST(0x0000A4CA0094086A AS DateTime))
SET IDENTITY_INSERT [dbo].[DataType] OFF
/****** Object:  Default [DF_DataType_IsActive]    Script Date: 07/14/2015 11:10:18 ******/
ALTER TABLE [dbo].[DataType] ADD  CONSTRAINT [DF_DataType_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO
/****** Object:  Default [DF_DataType_Created]    Script Date: 07/14/2015 11:10:18 ******/
ALTER TABLE [dbo].[DataType] ADD  CONSTRAINT [DF_DataType_Created]  DEFAULT (getdate()) FOR [Created]
GO
/****** Object:  Default [DF_Field_Created]    Script Date: 07/14/2015 11:10:18 ******/
ALTER TABLE [dbo].[Field] ADD  CONSTRAINT [DF_Field_Created]  DEFAULT (getdate()) FOR [Created]
GO
