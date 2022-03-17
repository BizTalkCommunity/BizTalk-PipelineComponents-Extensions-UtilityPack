USE [master]
GO

/****** Object:  Database [BizTalkArchiveDb]    Script Date: 11/18/2011 21:04:42 ******/
IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'BizTalkArchiveDb')
DROP DATABASE [BizTalkArchiveDb]
GO

USE [master]
GO

/****** Object:  Database [BizTalkArchiveDb]    Script Date: 11/18/2011 21:04:42 ******/
CREATE DATABASE [BizTalkArchiveDb] 
GO
ALTER DATABASE [BizTalkArchiveDb] SET COMPATIBILITY_LEVEL = 100
GO

USE [BizTalkArchiveDb]
GO
/****** Object:  Table [dbo].[Messages]    Script Date: 11/18/2011 21:04:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Messages](
	[MessageId] [uniqueidentifier] NOT NULL,
	[Body] [varbinary](max) NULL,
	[Property] [xml] NULL,
	[Size] [bigint] NOT NULL,
	[IsCompressed] [bit] NOT NULL,
	[CompressedSize] [bigint] NOT NULL,
	[CreatedOn] [date] NOT NULL,
 CONSTRAINT [PK_Messages] PRIMARY KEY CLUSTERED 
(
	[MessageId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[InsMessages]    Script Date: 11/18/2011 21:04:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[InsMessages] 
(
@MessageId UNIQUEIDENTIFIER
,@Body VARBINARY(MAX)
,@Property XML
,@Size BIGINT
,@IsCompressed BIT=0
,@CompressedSize BIGINT=0
)
AS
BEGIN
	INSERT INTO [Messages] (MessageId, Body, Property,Size,IsCompressed,CompressedSize)
	VALUES (@MessageId, @Body, @Property,@Size,@IsCompressed,@CompressedSize)
END
GO
/****** Object:  Default [DF_Messages_Size]    Script Date: 11/18/2011 21:04:00 ******/
ALTER TABLE [dbo].[Messages] ADD  CONSTRAINT [DF_Messages_Size]  DEFAULT ((0)) FOR [Size]
GO
/****** Object:  Default [DF_Messages_IsZip]    Script Date: 11/18/2011 21:04:00 ******/
ALTER TABLE [dbo].[Messages] ADD  CONSTRAINT [DF_Messages_IsZip]  DEFAULT ((0)) FOR [IsCompressed]
GO
/****** Object:  Default [DF_Messages_CompressedSize]    Script Date: 11/18/2011 21:04:00 ******/
ALTER TABLE [dbo].[Messages] ADD  CONSTRAINT [DF_Messages_CompressedSize]  DEFAULT ((0)) FOR [CompressedSize]
GO
/****** Object:  Default [DF_Messages_CreatedOn]    Script Date: 11/18/2011 21:04:00 ******/
ALTER TABLE [dbo].[Messages] ADD  CONSTRAINT [DF_Messages_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO
