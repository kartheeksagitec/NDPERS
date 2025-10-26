/*Schema_Change_PIR_13155 */
/********************Purpose: PIR 13155 Add Notes field to Contact Maintenance screen ******************************
*********************Created By: Abhijeet********************************
*********************Comments: *****************/
IF NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SGT_SCREEN_NOTES') 
BEGIN
	SET ANSI_NULLS ON
	
	SET QUOTED_IDENTIFIER ON
	
	SET ANSI_PADDING ON
	
	CREATE TABLE [dbo].[SGT_SCREEN_NOTES](
		[SCREEN_NOTES_ID] [int] IDENTITY(1,1) NOT NULL,	
		[SCREEN_ID] [int] NULL,
		[SCREEN_VALUE] [varchar](4) NULL,
		[SCREEN_PRIMARY_ID] [int] NULL,
		[NOTES] [varchar](2000) NULL,	
		[CREATED_BY] [varchar](50) NOT NULL,
		[CREATED_DATE] [datetime] NOT NULL,
		[MODIFIED_BY] [varchar](50) NOT NULL,
		[MODIFIED_DATE] [datetime] NOT NULL,
		[UPDATE_SEQ] [int] NOT NULL,
	 CONSTRAINT [PK_SGT_SCREEN_NOTES] PRIMARY KEY CLUSTERED 
	(
		[SCREEN_NOTES_ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	SET ANSI_PADDING OFF
END

/********************Purpose: PIR 23567 when choosing ‘Remove Dependent’ change reason set path of divorce if Is Applied for divorce value ******************************
*********************Created By: Abhijeet M ********************************
*********************Comments: when choosing ‘Remove Dependent’ change reason set path of divorce if Is Applied for divorce value "Yes" or No *****************/

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'SGT_WSS_PERSON_ACCOUNT_ENROLLMENT_REQUEST'
                 AND COLUMN_NAME = 'IS_APPLIED_FOR_DIVORCE')
ALTER TABLE SGT_WSS_PERSON_ACCOUNT_ENROLLMENT_REQUEST
ADD IS_APPLIED_FOR_DIVORCE VARCHAR(1) DEFAULT 'N'
