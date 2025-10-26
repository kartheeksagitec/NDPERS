
/****** Object:  Table [dbo].[SGS_XML_DEPLOYMENT_HEADER]    Script Date: 11/20/2017 1:06:11 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[SGS_XML_DEPLOYMENT_HEADER](
	[xml_deployment_header_id] [int] IDENTITY(1,1) NOT NULL,
	[deployed_from_machine] [varchar](250) NULL,
	[deployed_from_location] [varchar](250) NULL,
	[date_created] [datetime] NOT NULL,
 CONSTRAINT [sgs_deployment_header_PK] PRIMARY KEY CLUSTERED 
(
	[xml_deployment_header_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


/****** Object:  Table [dbo].[SGS_XML_DEPLOYMENT_DETAIL]    Script Date: 11/20/2017 1:06:28 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[SGS_XML_DEPLOYMENT_DETAIL](
	[xml_deployment_detail_id] [int] IDENTITY(1,1) NOT NULL,
	[deployment_header_id] [int] NOT NULL,
	[xml_document_name] [varchar](250) NULL,
	[cached_data] [image] NULL,
	[xml_document_hashcode] [varchar](250) NOT NULL,
	[detail_status] [varchar](4) NOT NULL,
	[deployment_message] [varchar](max) NOT NULL,
 CONSTRAINT [sgs_xml_deployment_detail_PK] PRIMARY KEY CLUSTERED 
(
	[xml_deployment_detail_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


