----------------------------------------------------------------------------------------------------------------------------------
-- Name - Pathan Vasim
-- Date - 05/10/2018
-- Purpose - SGS_EXCEPTION_LOG
----------------------------------------------------------------------------------------------------------------------------------

/****** Object:  Table [dbo].[SGS_EXCEPTION_LOG]    Script Date: 05/10/2018 4:11:17 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SGS_EXCEPTION_LOG](
	[EXCEPTION_LOG_ID] [int] IDENTITY(1,1) NOT NULL,
	[EXCEPTION_TIMESTAMP] [datetime] NULL,
	[EXCEPTION_INFO] [varchar](max) NULL,
	[EXCEPTION_PROCESS_NAME] [varchar](max) NULL,
	[EXCEPTION_MESSAGE] [varchar](max) NULL,
	[STACK_TRACE] [varchar](max) NULL,
	[TRANSACTION_ID] [uniqueidentifier] NULL,
	[USER_ID] [varchar](max) NULL,
	[APPLICATION_NAME] [varchar](max) NULL,
	[FORM_NAME] [varchar](max) NULL,
	[MACHINE_NAME] [varchar](max) NULL,
	[APPDOMAIN_NAME] [varchar](max) NULL,
	[WINDOWS_IDENTITY] [varchar](max) NULL,
	[INNER_EXCEPTION] [varchar](max) NULL,
	[EXCEPTION_SOURCE] [varchar](max) NULL,
	[PARAMETER_DETAILS] [varchar](max) NULL,
	[EXCEPTION_INSTANCE_ID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_SGS_EXCEPTION_LOG] PRIMARY KEY CLUSTERED 
(
	[EXCEPTION_LOG_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


