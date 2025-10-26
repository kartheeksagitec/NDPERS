/*Schema_Change_PIR 24737 */
/********************Purpose: PIR 24737 – Temp Health enrollment ******************************
*********************Created By: Nurul********************************
*********************Comments: *****************/
IF NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SGT_WSS_EMPLOYMENT_ACA_CERT') 
BEGIN
	SET ANSI_NULLS ON
	
	SET QUOTED_IDENTIFIER ON
	
	SET ANSI_PADDING ON
	
	CREATE TABLE [dbo].[SGT_WSS_EMPLOYMENT_ACA_CERT](
	    [WSS_EMPLOYMENT_ACA_CERT_ID] [int] IDENTITY(1,1) NOT NULL,
		[PERSON_ID] [int] NULL,	
		[PERSON_EMPLOYMENT_ID] [int] NULL,
		[CONTACT_ID] [int] NULL,
		[MET_REQ] [varchar](1) NULL,
		[METHOD] [varchar](20) NULL,
		[LB_MEASURE] [varchar](20) NULL,
		[FROM_DATE] [datetime]  NULL,
		[TO_DATE] [datetime]  NULL,
		[CREATED_BY] [varchar](50) NOT NULL,
		[CREATED_DATE] [datetime] NOT NULL,
		[MODIFIED_BY] [varchar](50) NOT NULL,
		[MODIFIED_DATE] [datetime] NOT NULL,
		[UPDATE_SEQ] [int] NOT NULL,
	 CONSTRAINT [PK_SGT_WSS_EMPLOYMENT_ACA_CERT] PRIMARY KEY CLUSTERED 
	(
		[WSS_EMPLOYMENT_ACA_CERT_ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	SET ANSI_PADDING OFF
END


------------------------------------------------------------------------------------------------------------------------------------------------------------

IF NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE [TABLE_NAME] = 'SGT_WSS_EMPLOYMENT_ACA_CERT' AND [COLUMN_NAME] = 'PER_EMP_DTL_ID')
BEGIN
ALTER table SGT_WSS_EMPLOYMENT_ACA_CERT
ADD PER_EMP_DTL_ID INT NULL
END
GO
--------------------------------------------------------------------------------------------------------------------------------------------------------

IF NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE [TABLE_NAME] = 'SGT_WSS_PERSON_ACCOUNT_GHDV' AND [COLUMN_NAME] = 'WAIVE_REASON')
BEGIN
ALTER TABLE SGT_WSS_PERSON_ACCOUNT_GHDV 
ADD WAIVE_REASON [VARCHAR](5) NULL
END
GO

IF NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE [TABLE_NAME] = 'SGT_WSS_PERSON_ACCOUNT_GHDV' AND [COLUMN_NAME] = 'WAIVE_REASON_TEXT')
BEGIN
ALTER TABLE SGT_WSS_PERSON_ACCOUNT_GHDV 
ADD WAIVE_REASON_TEXT [VARCHAR](200) NULL
END
GO
---------------------------------------------------------------------------------------------------------------------------------------------------------

