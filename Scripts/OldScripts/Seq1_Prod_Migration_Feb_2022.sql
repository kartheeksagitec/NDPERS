/**********************************************
**************PIR 24460 - Schema Changes*******
***********************************************/

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SGT_BOARD_MEMBER_ELECTION')
CREATE TABLE SGT_BOARD_MEMBER_ELECTION 
(
	ELECTION_ID INT PRIMARY KEY IDENTITY(1,1),
	[START_DATE] DATE,
	END_DATE DATE,
	AUDIENCE_ID INT,
	AUDIENCE_VALUE VARCHAR(4),
	ALLOWED_VOTES_PER_PERSON tinyint, 
	CREATED_BY VARCHAR(50),
	CREATED_DATE DATETIME,
	MODIFIED_BY VARCHAR(50),
	MODIFIED_DATE DATETIME,
	UPDATE_SEQ INT
)
GO

/****** Object:  Table [dbo].[SGT_BOARD_MEMBER_ELECTION_CANDIDATE]    Script Date: 1/16/2022 9:33:28 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SGT_BOARD_MEMBER_ELECTION_CANDIDATE]') AND type in (N'U'))
DROP TABLE [dbo].[SGT_BOARD_MEMBER_ELECTION_CANDIDATE]
GO

/****** Object:  Table [dbo].[SGT_BOARD_MEMBER_ELECTION_CANDIDATE]    Script Date: 1/16/2022 9:33:28 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SGT_BOARD_MEMBER_ELECTION_CANDIDATE](
	[CANDIDATE_ID] [int] IDENTITY(1,1) NOT NULL,
	[ELECTION_ID] [int] NULL,
	[CANDIDATE_NAME] [varchar](200) NULL,
	[CANDIDATE_BIO] [varchar](2000) NULL,
	[CANDIDATE_PICTURE] [varbinary](max) NULL,
	[CREATED_BY] [varchar](50) NULL,
	[CREATED_DATE] [datetime] NULL,
	[MODIFIED_BY] [varchar](50) NULL,
	[MODIFIED_DATE] [datetime] NULL,
	[UPDATE_SEQ] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[CANDIDATE_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[SGT_BOARD_MEMBER_ELECTION_CANDIDATE]  WITH CHECK ADD FOREIGN KEY([ELECTION_ID])
REFERENCES [dbo].[SGT_BOARD_MEMBER_ELECTION] ([ELECTION_ID])
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SGT_BOARD_MEMBER_VOTE]') AND type in (N'U'))
CREATE TABLE [dbo].[SGT_BOARD_MEMBER_VOTE](
	[BOARD_MEMBER_VOTE_ID] [int] IDENTITY(1,1) NOT NULL,
	[ELECTION_ID] [int] NOT NULL,
	[PERSON_ID] [int] NOT NULL,
	[CANDIDATE_ID] [int] NULL,
	[VOTE_CAST_TIME] [datetime] NOT NULL,
	[CANDIDATE_NAME] [varchar](150) NULL,
	[CREATED_BY] [varchar](50) NULL,
	[CREATED_DATE] [datetime] NULL,
	[MODIFIED_BY] [varchar](50) NULL,
	[MODIFIED_DATE] [datetime] NULL,
	[UPDATE_SEQ] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[BOARD_MEMBER_VOTE_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SGT_BOARD_MEMBER_VOTE]
ADD CONSTRAINT FK_SGT_BOARD_MEMBER_VOTE_ELECTION_ID_SGT_BOARD_MEMBER_ELECTION_ELECTION_ID
FOREIGN KEY ([ELECTION_ID]) REFERENCES SGT_BOARD_MEMBER_ELECTION([ELECTION_ID])
GO

ALTER TABLE [dbo].[SGT_BOARD_MEMBER_VOTE] CHECK CONSTRAINT [FK_SGT_BOARD_MEMBER_VOTE_ELECTION_ID_SGT_BOARD_MEMBER_ELECTION_ELECTION_ID]
GO

ALTER TABLE [dbo].[SGT_BOARD_MEMBER_VOTE]  WITH CHECK ADD  CONSTRAINT [FK_SGT_BOARD_MEMBER_VOTE_PERSON_ID_SGT_PERSON_BASE_PERSON_ID] FOREIGN KEY([PERSON_ID])
REFERENCES [dbo].[sgt_person_base] ([PERSON_ID])
GO

ALTER TABLE [dbo].[SGT_BOARD_MEMBER_VOTE] CHECK CONSTRAINT [FK_SGT_BOARD_MEMBER_VOTE_PERSON_ID_SGT_PERSON_BASE_PERSON_ID]
GO

UPDATE SGS_CODE_VALUE SET DATA1 = 'Y' WHERE CODE_VALUE IN ('ACTV', 'RETR') AND CODE_ID = 7008
GO

/**********************************************
**************PIR 24460 - Data Changes*******
***********************************************/

IF NOT EXISTS (SELECT 1 FROM SGS_MESSAGES WHERE MESSAGE_ID = 10439)
INSERT INTO [dbo].[SGS_MESSAGES]
           ([MESSAGE_ID]
           ,[DISPLAY_MESSAGE]
           ,[SEVERITY_ID]
           ,[SEVERITY_VALUE]
           ,[INTERNAL_INSTRUCTIONS]
           ,[EMPLOYER_INSTRUCTIONS]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (10439
           ,'Number Of Votes Should Be Greater Than Zero.'
           ,16
           ,'E'
           ,NULL
           ,NULL
           ,'PIR_24460'
           ,GETDATE()
           ,'PIR_24460'
           ,GETDATE()
           ,0)
GO

IF NOT EXISTS (SELECT 1 FROM SGS_MESSAGES WHERE MESSAGE_ID = 10440)
INSERT INTO [dbo].[SGS_MESSAGES]
           ([MESSAGE_ID]
           ,[DISPLAY_MESSAGE]
           ,[SEVERITY_ID]
           ,[SEVERITY_VALUE]
           ,[INTERNAL_INSTRUCTIONS]
           ,[EMPLOYER_INSTRUCTIONS]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (10440
           ,'Candidate Name Is Required.'
           ,16
           ,'E'
           ,NULL
           ,NULL
           ,'PIR_24460'
           ,GETDATE()
           ,'PIR_24460'
           ,GETDATE()
           ,0)
GO

IF NOT EXISTS (SELECT 1 FROM SGS_MESSAGES WHERE MESSAGE_ID = 10441)
INSERT INTO [dbo].[SGS_MESSAGES]
           ([MESSAGE_ID]
           ,[DISPLAY_MESSAGE]
           ,[SEVERITY_ID]
           ,[SEVERITY_VALUE]
           ,[INTERNAL_INSTRUCTIONS]
           ,[EMPLOYER_INSTRUCTIONS]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (10441
           ,'Candidate Bio Is Required.'
           ,16
           ,'E'
           ,NULL
           ,NULL
           ,'PIR_24460'
           ,GETDATE()
           ,'PIR_24460'
           ,GETDATE()
           ,0)
GO
IF NOT EXISTS (SELECT 1 FROM SGS_MESSAGES WHERE MESSAGE_ID = 10442)
INSERT INTO [dbo].[SGS_MESSAGES]
           ([MESSAGE_ID]
           ,[DISPLAY_MESSAGE]
           ,[SEVERITY_ID]
           ,[SEVERITY_VALUE]
           ,[INTERNAL_INSTRUCTIONS]
           ,[EMPLOYER_INSTRUCTIONS]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (10442
           ,'Candidate Picture Is Required.'
           ,16
           ,'E'
           ,NULL
           ,NULL
           ,'PIR_24460'
           ,GETDATE()
           ,'PIR_24460'
           ,GETDATE()
           ,0)
GO

IF NOT EXISTS(SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE = 'BMST')
INSERT INTO [dbo].[SGT_WSS_ACKNOWLEDGEMENT]
           ([EFFECTIVE_DATE]
           ,[SCREEN_STEP_ID]
           ,[SCREEN_STEP_VALUE]
           ,[DISPLAY_SEQUENCE]
           ,[ACKNOWLEDGEMENT_TEXT]
           ,[SHOW_CHECK_BOX_FLAG]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           ('2022-01-01 00:00:00.000'
           ,6000
           ,'BMST'
           ,1
           ,'Welcome to the {0} NDPERS Board election. There is an opening for an active member on the North Dakota Public Employees Retirement System Board.<br/> The term is for five years starting {1} through {2}.<br/><br/> Vote for {3} of the candidates or write the name of another eligible NDPERS active member by the end of the day on {4}.<br/><br/> Get to know the candidates by reading the following biographies. To cast your vote, click on the “Vote now” button. The NDPERS Board appreciates your time.'
           ,'N'
           ,'PIR_24460'
           ,GETDATE()
           ,'PIR_24460'
           ,GETDATE()
           ,0)
GO
IF NOT EXISTS (SELECT 1 FROM SGS_CODE_VALUE WHERE CODE_ID = 6000 AND CODE_VALUE = 'BMST')
INSERT INTO [dbo].[SGS_CODE_VALUE]
           ([CODE_ID]
           ,[CODE_VALUE]
           ,[DESCRIPTION]
           ,[DATA1]
           ,[DATA2]
           ,[DATA3]
           ,[COMMENTS]
           ,[START_DATE]
           ,[END_DATE]
           ,[CODE_VALUE_ORDER]
           ,[LEGACY_CODE_ID]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (6000
           ,'BMST'
           ,'Board Member Static Text'
           ,null
           ,null
           ,null
           ,null
           ,null
           ,null
           ,null
           ,null
           ,'PIR_24460'
           ,GETDATE()
           ,'PIR_24460'
           ,GETDATE()
           ,0)
GO

IF NOT EXISTS(SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE = 'BTST')
INSERT INTO [dbo].[SGT_WSS_ACKNOWLEDGEMENT]
           ([EFFECTIVE_DATE]
           ,[SCREEN_STEP_ID]
           ,[SCREEN_STEP_VALUE]
           ,[DISPLAY_SEQUENCE]
           ,[ACKNOWLEDGEMENT_TEXT]
           ,[SHOW_CHECK_BOX_FLAG]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           ('2022-01-01 00:00:00.000'
           ,6000
           ,'BTST'
           ,1
           ,'Vote for {0} of the candidates or write the name of another eligible NDPERS {1} member by the end of the day on {2}. <br/><br/> Your vote is confidential. You can only cast your vote one time. Scroll up to read the biographies again.'
           ,'N'
           ,'PIR_24460'
           ,GETDATE()
           ,'PIR_24460'
           ,GETDATE()
           ,0)
GO
IF NOT EXISTS (SELECT 1 FROM SGS_CODE_VALUE WHERE CODE_ID = 6000 AND CODE_VALUE = 'BTST')
INSERT INTO [dbo].[SGS_CODE_VALUE]
           ([CODE_ID]
           ,[CODE_VALUE]
           ,[DESCRIPTION]
           ,[DATA1]
           ,[DATA2]
           ,[DATA3]
           ,[COMMENTS]
           ,[START_DATE]
           ,[END_DATE]
           ,[CODE_VALUE_ORDER]
           ,[LEGACY_CODE_ID]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (6000
           ,'BTST'
           ,'Ballet Static Text'
           ,null
           ,null
           ,null
           ,null
           ,null
           ,null
           ,null
           ,null
           ,'PIR_24460'
           ,GETDATE()
           ,'PIR_24460'
           ,GETDATE()
           ,0)
GO

IF NOT EXISTS (SELECT 1 FROM SGS_MESSAGES WHERE MESSAGE_ID = 10443)
INSERT INTO [dbo].[SGS_MESSAGES]
           ([MESSAGE_ID]
           ,[DISPLAY_MESSAGE]
           ,[SEVERITY_ID]
           ,[SEVERITY_VALUE]
           ,[INTERNAL_INSTRUCTIONS]
           ,[EMPLOYER_INSTRUCTIONS]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (10443
           ,'Please Vote For At Least One.'
           ,16
           ,'E'
           ,NULL
           ,NULL
           ,'PIR_24460'
           ,GETDATE()
           ,'PIR_24460'
           ,GETDATE()
           ,0)
GO
IF NOT EXISTS (SELECT 1 FROM SGS_MESSAGES WHERE MESSAGE_ID = 10444)
INSERT INTO [dbo].[SGS_MESSAGES]
           ([MESSAGE_ID]
           ,[DISPLAY_MESSAGE]
           ,[SEVERITY_ID]
           ,[SEVERITY_VALUE]
           ,[INTERNAL_INSTRUCTIONS]
           ,[EMPLOYER_INSTRUCTIONS]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (10444
           ,'You Are Voting For More Than Allowed.'
           ,16
           ,'E'
           ,NULL
           ,NULL
           ,'PIR_24460'
           ,GETDATE()
           ,'PIR_24460'
           ,GETDATE()
           ,0)
GO

IF NOT EXISTS (SELECT 1 FROM SGS_MESSAGES WHERE MESSAGE_ID = 10445)
INSERT INTO [dbo].[SGS_MESSAGES]
           ([MESSAGE_ID]
           ,[DISPLAY_MESSAGE]
           ,[SEVERITY_ID]
           ,[SEVERITY_VALUE]
           ,[INTERNAL_INSTRUCTIONS]
           ,[EMPLOYER_INSTRUCTIONS]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (10445
           ,'Please Enter Write-In Candidate Name.'
           ,16
           ,'E'
           ,NULL
           ,NULL
           ,'PIR_24460'
           ,GETDATE()
           ,'PIR_24460'
           ,GETDATE()
           ,0)
GO
IF NOT EXISTS (SELECT 1 FROM SGS_MESSAGES WHERE MESSAGE_ID = 10446)
INSERT INTO [dbo].[SGS_MESSAGES]
           ([MESSAGE_ID]
           ,[DISPLAY_MESSAGE]
           ,[SEVERITY_ID]
           ,[SEVERITY_VALUE]
           ,[INTERNAL_INSTRUCTIONS]
           ,[EMPLOYER_INSTRUCTIONS]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (10446
           ,'Audience Is Required.'
           ,16
           ,'E'
           ,NULL
           ,NULL
           ,'PIR_24460'
           ,GETDATE()
           ,'PIR_24460'
           ,GETDATE()
           ,0)
GO


IF NOT EXISTS(SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE = 'BMET')
INSERT INTO [dbo].[SGT_WSS_ACKNOWLEDGEMENT]
           ([EFFECTIVE_DATE]
           ,[SCREEN_STEP_ID]
           ,[SCREEN_STEP_VALUE]
           ,[DISPLAY_SEQUENCE]
           ,[ACKNOWLEDGEMENT_TEXT]
           ,[SHOW_CHECK_BOX_FLAG]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           ('2022-01-01 00:00:00.000'
           ,6000
           ,'BMET'
           ,1
           ,'Learn about each of the candidates and cast your vote by {0}'
           ,'N'
           ,'PIR_24460'
           ,GETDATE()
           ,'PIR_24460'
           ,GETDATE()
           ,0)
GO
IF NOT EXISTS (SELECT 1 FROM SGS_CODE_VALUE WHERE CODE_ID = 6000 AND CODE_VALUE = 'BMET')
INSERT INTO [dbo].[SGS_CODE_VALUE]
           ([CODE_ID]
           ,[CODE_VALUE]
           ,[DESCRIPTION]
           ,[DATA1]
           ,[DATA2]
           ,[DATA3]
           ,[COMMENTS]
           ,[START_DATE]
           ,[END_DATE]
           ,[CODE_VALUE_ORDER]
           ,[LEGACY_CODE_ID]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (6000
           ,'BMET'
           ,'Board Member Election Text'
           ,null
           ,null
           ,null
           ,null
           ,null
           ,null
           ,null
           ,null
           ,'PIR_24460'
           ,GETDATE()
           ,'PIR_24460'
           ,GETDATE()
           ,0)
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'TERM_START_DATE'
          AND Object_ID = Object_ID(N'SGT_BOARD_MEMBER_ELECTION'))
ALTER TABLE SGT_BOARD_MEMBER_ELECTION
ADD TERM_START_DATE DATE NULL
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'TERM_END_DATE'
          AND Object_ID = Object_ID(N'SGT_BOARD_MEMBER_ELECTION'))
ALTER TABLE SGT_BOARD_MEMBER_ELECTION
ADD TERM_END_DATE DATE NULL
GO

IF NOT EXISTS (SELECT 1 FROM SGS_MESSAGES WHERE MESSAGE_ID = 10447)
INSERT INTO [dbo].[SGS_MESSAGES]
           ([MESSAGE_ID]
           ,[DISPLAY_MESSAGE]
           ,[SEVERITY_ID]
           ,[SEVERITY_VALUE]
           ,[INTERNAL_INSTRUCTIONS]
           ,[EMPLOYER_INSTRUCTIONS]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (10447
           ,'Term Start Date Is Required.'
           ,16
           ,'E'
           ,NULL
           ,NULL
           ,'PIR_24460'
           ,GETDATE()
           ,'PIR_24460'
           ,GETDATE()
           ,0)
GO
IF NOT EXISTS (SELECT 1 FROM SGS_MESSAGES WHERE MESSAGE_ID = 10448)
INSERT INTO [dbo].[SGS_MESSAGES]
           ([MESSAGE_ID]
           ,[DISPLAY_MESSAGE]
           ,[SEVERITY_ID]
           ,[SEVERITY_VALUE]
           ,[INTERNAL_INSTRUCTIONS]
           ,[EMPLOYER_INSTRUCTIONS]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (10448
           ,'Term End Date Is Required.'
           ,16
           ,'E'
           ,NULL
           ,NULL
           ,'PIR_24460'
           ,GETDATE()
           ,'PIR_24460'
           ,GETDATE()
           ,0)
GO

UPDATE SGS_CODE_VALUE SET [DESCRIPTION] = 'Board Member Static Text For Active' WHERE CODE_ID = 6000 AND CODE_VALUE = 'BMST'
GO

UPDATE SGS_CODE_VALUE SET [DESCRIPTION] = 'Ballot Static Text For Active' WHERE CODE_ID = 6000 AND CODE_VALUE = 'BTST'
GO

UPDATE SGS_CODE_VALUE SET [DESCRIPTION] = 'Board Member Election Text For Active' WHERE CODE_ID = 6000 AND CODE_VALUE = 'BMET'
GO



IF NOT EXISTS(SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE = 'BMTR')
INSERT INTO [dbo].[SGT_WSS_ACKNOWLEDGEMENT]
           ([EFFECTIVE_DATE]
           ,[SCREEN_STEP_ID]
           ,[SCREEN_STEP_VALUE]
           ,[DISPLAY_SEQUENCE]
           ,[ACKNOWLEDGEMENT_TEXT]
           ,[SHOW_CHECK_BOX_FLAG]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           ('2022-01-01 00:00:00.000'
           ,6000
           ,'BMTR'
           ,1
           ,'Welcome to the {0} NDPERS Board election. There is an opening for retiree member on the North Dakota Public Employees Retirement System Board.<br/> The term is for five years starting {1} through {2}.<br/><br/> Vote for {3} of the candidates or write the name of another eligible NDPERS retiree member by the end of the day on {4}.<br/><br/> Get to know the candidates by reading the following biographies. To cast your vote, click on the “Vote now” button. The NDPERS Board appreciates your time.'
           ,'N'
           ,'PIR_24460'
           ,GETDATE()
           ,'PIR_24460'
           ,GETDATE()
           ,0)
GO
IF NOT EXISTS (SELECT 1 FROM SGS_CODE_VALUE WHERE CODE_ID = 6000 AND CODE_VALUE = 'BMTR')
INSERT INTO [dbo].[SGS_CODE_VALUE]
           ([CODE_ID]
           ,[CODE_VALUE]
           ,[DESCRIPTION]
           ,[DATA1]
           ,[DATA2]
           ,[DATA3]
           ,[COMMENTS]
           ,[START_DATE]
           ,[END_DATE]
           ,[CODE_VALUE_ORDER]
           ,[LEGACY_CODE_ID]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (6000
           ,'BMTR'
           ,'Board Member Static Text For Retirees'
           ,null
           ,null
           ,null
           ,null
           ,null
           ,null
           ,null
           ,null
           ,'PIR_24460'
           ,GETDATE()
           ,'PIR_24460'
           ,GETDATE()
           ,0)
GO

IF NOT EXISTS(SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE = 'BTTR')
INSERT INTO [dbo].[SGT_WSS_ACKNOWLEDGEMENT]
           ([EFFECTIVE_DATE]
           ,[SCREEN_STEP_ID]
           ,[SCREEN_STEP_VALUE]
           ,[DISPLAY_SEQUENCE]
           ,[ACKNOWLEDGEMENT_TEXT]
           ,[SHOW_CHECK_BOX_FLAG]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           ('2022-01-01 00:00:00.000'
           ,6000
           ,'BTTR'
           ,1
           ,'Vote for {0} of the candidates or write the name of another eligible NDPERS {1} member by the end of the day on {2}. <br/><br/> Your vote is confidential. You can only cast your vote one time. Scroll up to read the biographies again.'
           ,'N'
           ,'PIR_24460'
           ,GETDATE()
           ,'PIR_24460'
           ,GETDATE()
           ,0)
GO
IF NOT EXISTS (SELECT 1 FROM SGS_CODE_VALUE WHERE CODE_ID = 6000 AND CODE_VALUE = 'BTTR')
INSERT INTO [dbo].[SGS_CODE_VALUE]
           ([CODE_ID]
           ,[CODE_VALUE]
           ,[DESCRIPTION]
           ,[DATA1]
           ,[DATA2]
           ,[DATA3]
           ,[COMMENTS]
           ,[START_DATE]
           ,[END_DATE]
           ,[CODE_VALUE_ORDER]
           ,[LEGACY_CODE_ID]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (6000
           ,'BTTR'
           ,'Ballot Static Text For Retirees'
           ,null
           ,null
           ,null
           ,null
           ,null
           ,null
           ,null
           ,null
           ,'PIR_24460'
           ,GETDATE()
           ,'PIR_24460'
           ,GETDATE()
           ,0)
GO

IF NOT EXISTS(SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE SCREEN_STEP_VALUE = 'BETR')
INSERT INTO [dbo].[SGT_WSS_ACKNOWLEDGEMENT]
           ([EFFECTIVE_DATE]
           ,[SCREEN_STEP_ID]
           ,[SCREEN_STEP_VALUE]
           ,[DISPLAY_SEQUENCE]
           ,[ACKNOWLEDGEMENT_TEXT]
           ,[SHOW_CHECK_BOX_FLAG]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           ('2022-01-01 00:00:00.000'
           ,6000
           ,'BETR'
           ,1
           ,'Learn about each of the candidates and cast your vote by {0}'
           ,'N'
           ,'PIR_24460'
           ,GETDATE()
           ,'PIR_24460'
           ,GETDATE()
           ,0)
GO
IF NOT EXISTS (SELECT 1 FROM SGS_CODE_VALUE WHERE CODE_ID = 6000 AND CODE_VALUE = 'BETR')
INSERT INTO [dbo].[SGS_CODE_VALUE]
           ([CODE_ID]
           ,[CODE_VALUE]
           ,[DESCRIPTION]
           ,[DATA1]
           ,[DATA2]
           ,[DATA3]
           ,[COMMENTS]
           ,[START_DATE]
           ,[END_DATE]
           ,[CODE_VALUE_ORDER]
           ,[LEGACY_CODE_ID]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           (6000
           ,'BETR'
           ,'Board Member Election Text For Retirees'
           ,null
           ,null
           ,null
           ,null
           ,null
           ,null
           ,null
           ,null
           ,'PIR_24460'
           ,GETDATE()
           ,'PIR_24460'
           ,GETDATE()
           ,0)
GO