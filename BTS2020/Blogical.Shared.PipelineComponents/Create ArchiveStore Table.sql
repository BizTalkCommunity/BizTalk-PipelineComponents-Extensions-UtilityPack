GO
/****** Object:  Table [dbo].[ArchiveStore]    Script Date: 04/23/2008 23:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ArchiveStore](
	[InstanceID] [uniqueidentifier] NOT NULL,
	[DateTime] [datetime] NOT NULL CONSTRAINT [DF_ArchiveStore_DateTime]  DEFAULT (getdate()),
	[Source] [varchar](255) NOT NULL,
	[Message] [image] NOT NULL,
 CONSTRAINT [PK_ArchiveStore] PRIMARY KEY CLUSTERED 
(
	[InstanceID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF