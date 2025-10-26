/********************Purpose: PIR 25712******************************
*********************Created By: Saylee P********************************
*********************Comments: *****************/


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SGT_LIFE_REF_CONFIG](
	[LIFE_REF_CONFIG_ID] [int] IDENTITY(1,1) NOT NULL,
	[EFFECTIVE_DATE] [datetime] NOT NULL,
	[LIFE_INSURANCE_TYPE_ID] [int] NULL,
	[LIFE_INSURANCE_TYPE_VALUE] [varchar](4) NULL,
	[LEVEL_OF_COVERAGE_ID] [int] NULL,
	[LEVEL_OF_COVERAGE_VALUE] [varchar](4) NULL,
	[PRE_TAX_LIMIT] [decimal](13, 2) NULL,
	[GI_LIMIT] [decimal](13, 2) NULL,
	[INCREMENT] [decimal](13, 2) NULL,	
	[COVERAGE_LIMIT_PERCENTAGE] [decimal](13, 2) NULL,
	[CREATED_BY] [varchar](50) NOT NULL,
	[CREATED_DATE] [datetime] NOT NULL,
	[MODIFIED_BY] [varchar](50) NOT NULL,
	[MODIFIED_DATE] [datetime] NOT NULL,
	[UPDATE_SEQ] [int] NOT NULL,	
	[INCREMENT_LIMIT] [decimal](13, 2) NULL,
 CONSTRAINT [PK_SGT_LIFE_REF_CONFIG] PRIMARY KEY CLUSTERED 
(
	[LIFE_REF_CONFIG_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO