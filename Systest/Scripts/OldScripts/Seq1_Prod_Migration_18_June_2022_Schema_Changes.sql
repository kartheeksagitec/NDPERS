
/********************Purpose: PIR 24847 – Email notifications for Urgent ESS messages ******************************
*********************Created By: Nurul********************************
*********************Comments: *****************/
IF NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SGT_WSS_DASHBOARD_MESSAGES') 
BEGIN
	SET ANSI_NULLS ON
	
	SET QUOTED_IDENTIFIER ON
	
	SET ANSI_PADDING ON
	
	CREATE TABLE [dbo].[SGT_WSS_DASHBOARD_MESSAGES](
	    [WSS_DASHBOARD_MESSAGES_ID] [int] IDENTITY(1,1) NOT NULL,
		[MESSAGE_TEXT] [varchar](1000) NULL,
		[PRIORITY_ID] [int] NOT NULL,
		[PRIORITY_VALUE] [varchar](10) NULL,
		[AUDIENCE_ID] [int] NOT NULL,
		[AUDIENCE_VALUE] [varchar](5)  NULL,
		[CREATED_BY] [varchar](50) NOT NULL,
		[CREATED_DATE] [datetime] NOT NULL,
		[MODIFIED_BY] [varchar](50) NOT NULL,
		[MODIFIED_DATE] [datetime] NOT NULL,
		[UPDATE_SEQ] [int] NOT NULL,
	 CONSTRAINT [PK_SGT_WSS_DASHBOARD_MESSAGES] PRIMARY KEY CLUSTERED 
	(
		[WSS_DASHBOARD_MESSAGES_ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	SET ANSI_PADDING OFF
END




-------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE [TABLE_NAME] = 'SGT_WSS_MESSAGE_DETAIL' AND [COLUMN_NAME] = 'ESS_EMAIL_SENT_FLAG')
BEGIN
ALTER TABLE SGT_WSS_MESSAGE_DETAIL 
ADD ESS_EMAIL_SENT_FLAG  [VARCHAR](1) NULL
END
GO






