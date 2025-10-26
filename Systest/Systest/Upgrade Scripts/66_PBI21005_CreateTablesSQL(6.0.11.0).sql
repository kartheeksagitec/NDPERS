-----------------------------------------
--Created By	:	Mansi Shah
--Created On	:	4th March 2020
--Description	:	Adding tables SGW_BPM_ACTIVITY_ESC_MESSAGES,SGW_BPM_PROCESS_ESC_MESSAGES and default data
------------------------------------------------------------------------------------------------------------------------ 
IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE  TABLE_NAME = 'SGW_BPM_ACTIVITY_ESC_MESSAGES'))
BEGIN
CREATE TABLE [dbo].[SGW_BPM_ACTIVITY_ESC_MESSAGES](
	[ACTIVITY_ESC_MESSAGE_ID] [int] IDENTITY(1,1) NOT NULL,
	[ESCALATION_TYPE_VALUE] [varchar](4) NOT NULL,
	[ESCALATION_MESSAGE] [varchar](max) NOT NULL,
 CONSTRAINT [PK_ACTIVITY_ESC] PRIMARY KEY CLUSTERED 
(
	[ACTIVITY_ESC_MESSAGE_ID] ASC
)) 

--Insert default messages
insert into [SGW_BPM_ACTIVITY_ESC_MESSAGES] (ESCALATION_TYPE_VALUE,ESCALATION_MESSAGE) values ('LTBE','The case {0} with case instance id {1} at step {2} is due on {3}.')
insert into [SGW_BPM_ACTIVITY_ESC_MESSAGES] (ESCALATION_TYPE_VALUE,ESCALATION_MESSAGE) values ('LTAE','The case {0} with case instance id {1} at step {2} is delayed whose due date is on {3}.')
insert into [SGW_BPM_ACTIVITY_ESC_MESSAGES] (ESCALATION_TYPE_VALUE,ESCALATION_MESSAGE) values ('LTSA','The case {0} with case instance id {1} at step {2} is suspended since {3}.')
insert into [SGW_BPM_ACTIVITY_ESC_MESSAGES] (ESCALATION_TYPE_VALUE,ESCALATION_MESSAGE) values ('LTOE','The case {0} with case instance id {1} at step {2} is due now.')
insert into [SGW_BPM_ACTIVITY_ESC_MESSAGES] (ESCALATION_TYPE_VALUE,ESCALATION_MESSAGE) values ('LTNA','The case {0} with case instance id {1} at step {2} is un assigned.')

END
GO

IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE  TABLE_NAME = 'SGW_BPM_PROCESS_ESC_MESSAGES'))
BEGIN
CREATE TABLE [dbo].[SGW_BPM_PROCESS_ESC_MESSAGES](
	[PROCESS_ESC_MESSAGE_ID] [int] IDENTITY(1,1) NOT NULL,
	[ESCALATION_TYPE_VALUE] [varchar](4) NOT NULL,
	[ESCALATION_MESSAGE] [varchar](max) NOT NULL,
 CONSTRAINT [PK_PROCESS_ESC] PRIMARY KEY CLUSTERED 
(
	[PROCESS_ESC_MESSAGE_ID] ASC
))

--Insert default messages
insert into [SGW_BPM_PROCESS_ESC_MESSAGES] (ESCALATION_TYPE_VALUE,ESCALATION_MESSAGE) values ('LTAE','The process {0} with case instance id {1} is delayed whose due date is on {2}.')
insert into [SGW_BPM_PROCESS_ESC_MESSAGES] (ESCALATION_TYPE_VALUE,ESCALATION_MESSAGE) values ('LTBE','The process {0} with case instance id {1} is due on {2}.')
insert into [SGW_BPM_PROCESS_ESC_MESSAGES] (ESCALATION_TYPE_VALUE,ESCALATION_MESSAGE) values ('LTOE','The process {0} with case instance id {1} is due now.')

END
GO