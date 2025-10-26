/********************Purpose: Adhoc - ACH Pull and New Hire Automation******************************
*********************Created By: Nikhil********************************
*********************Comments: *****************/

IF NOT EXISTS (SELECT 1 FROM SGS_BATCH_SCHEDULE WHERE BATCH_SCHEDULE_ID = 157)
BEGIN
	INSERT INTO SGS_BATCH_SCHEDULE (BATCH_SCHEDULE_ID, STEP_NO, STEP_NAME, STEP_DESCRIPTION, FREQUENCY_IN_DAYS, ACTIVE_FLAG, REQUIRES_TRANSACTION_FLAG, CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, UPDATE_SEQ)
	VALUES (157, 1013, 'New Hire Auto-Plan Enrollment', 'New Hire Auto-Plan Enrollment', 1, 'Y', 'N', 'Adhoc - ACH Pull and New Hire Automation', GETDATE(), 'Adhoc - ACH Pull and New Hire Automation', GETDATE(), 0)	
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[SGS_CODE_VALUE] WHERE CODE_ID = 3508 AND CODE_VALUE = 'PEND') 
BEGIN
INSERT [dbo].[SGS_CODE_VALUE] ([CODE_ID], [CODE_VALUE], [DESCRIPTION], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ]) 
VALUES (3508, N'PEND', N'Pending Auto Posting', 'PIR Adhoc - ACH Pull and New Hire Automation', GETDATE(), 'PIR Adhoc - ACH Pull and New Hire Automation', GETDATE(), 0)
END
GO

IF NOT EXISTS (SELECT 1 FROM SGS_MESSAGES WHERE MESSAGE_ID = 10452)
BEGIN
INSERT INTO SGS_MESSAGES ([MESSAGE_ID], [DISPLAY_MESSAGE], [SEVERITY_ID], [SEVERITY_VALUE], [INTERNAL_INSTRUCTIONS], [EMPLOYER_INSTRUCTIONS], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES(10452, 'We Could Not Enroll The Member In The Mandatory Plans For New Hire, Please Enroll The Member Manually.', 16, 'E', NULL, NULL, 'PIR Adhoc - ACH Pull and New Hire Automation', GETDATE(), 'PIR Adhoc - ACH Pull and New Hire Automation', GETDATE(), 0)
END
GO

IF NOT EXISTS (SELECT * FROM SGS_CODE_VALUE WHERE CODE_ID = 5011 AND CODE_VALUE = 'NHAM')
BEGIN
INSERT [dbo].[SGS_CODE_VALUE] ([CODE_ID], [CODE_VALUE], [DESCRIPTION], [DATA1], [DATA3], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ]) 
VALUES (5011, N'NHAM', N'New Hire Automation', '1013', '6', 'PIR Adhoc - ACH Pull and New Hire Automation', GETDATE(), 'PIR Adhoc - ACH Pull and New Hire Automation', GETDATE(), 0)
END
GO
/********************Purpose: PIR 24620******************************
*********************Created By: Bharat********************************
*********************Comments: *****************/

IF NOT EXISTS (SELECT 1 FROM SGS_MESSAGES WHERE MESSAGE_ID = 10453)
BEGIN
INSERT INTO SGS_MESSAGES ([MESSAGE_ID], [DISPLAY_MESSAGE], [SEVERITY_ID], [SEVERITY_VALUE], [INTERNAL_INSTRUCTIONS], [EMPLOYER_INSTRUCTIONS], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES(10453, 'Last Reporting Month for Retirement Contributions has to be in the same month as either Last Date of Service with Agency or Last Date of Regular Paycheck.', 16, 'E', NULL, NULL, 'PIR 24620', GETDATE(), 'PIR 24620', GETDATE(), 0)
END
GO

------------------------------------------------------------------------------------------------------------

/********************Purpose: ACH Pull******************************
*********************Created By: Saylee P********************************
*********************Comments: *****************/

IF NOT EXISTS (SELECT 1 FROM SGS_BATCH_SCHEDULE WHERE BATCH_SCHEDULE_ID = 158)
BEGIN
INSERT INTO SGS_BATCH_SCHEDULE (BATCH_SCHEDULE_ID, STEP_NO, STEP_NAME, STEP_DESCRIPTION, FREQUENCY_IN_DAYS, ACTIVE_FLAG, REQUIRES_TRANSACTION_FLAG, CREATED_BY,
CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, UPDATE_SEQ)
VALUES (158, 1014, 'ACH Pull Automation - Retirement', NULL, 1, 'Y', 'N', 'Adhoc - ACH Pull and New Hire Automation',
GETDATE(), 'Adhoc - ACH Pull and New Hire Automation', GETDATE(), 0)
END
GO



IF NOT EXISTS (SELECT 1 FROM SGS_BATCH_SCHEDULE WHERE BATCH_SCHEDULE_ID = 159)
BEGIN
INSERT INTO SGS_BATCH_SCHEDULE (BATCH_SCHEDULE_ID, STEP_NO, STEP_NAME, STEP_DESCRIPTION, FREQUENCY_IN_DAYS, ACTIVE_FLAG, REQUIRES_TRANSACTION_FLAG, CREATED_BY,
CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, UPDATE_SEQ)
VALUES (159, 1015, 'ACH Pull Automation - Deferred Comp', NULL, 1, 'Y', 'N', 'Adhoc - ACH Pull and New Hire Automation',
GETDATE(), 'Adhoc - ACH Pull and New Hire Automation', GETDATE(), 0)
END
GO



IF NOT EXISTS (SELECT 1 FROM SGS_BATCH_SCHEDULE WHERE BATCH_SCHEDULE_ID = 160)
BEGIN
INSERT INTO SGS_BATCH_SCHEDULE (BATCH_SCHEDULE_ID, STEP_NO, STEP_NAME, STEP_DESCRIPTION, FREQUENCY_IN_DAYS, ACTIVE_FLAG, REQUIRES_TRANSACTION_FLAG, CREATED_BY,
CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, UPDATE_SEQ)
VALUES (160, 1016, 'ACH Pull Automation - Insurance', NULL, 1, 'Y', 'N', 'Adhoc - ACH Pull and New Hire Automation',
GETDATE(), 'Adhoc - ACH Pull and New Hire Automation', GETDATE(), 0)
END
GO


IF NOT EXISTS (SELECT 1 FROM SGS_BATCH_SCHEDULE WHERE BATCH_SCHEDULE_ID = 161)
BEGIN
INSERT INTO SGS_BATCH_SCHEDULE (BATCH_SCHEDULE_ID, STEP_NO, STEP_NAME, STEP_DESCRIPTION, FREQUENCY_IN_MONTHS, ACTIVE_FLAG, REQUIRES_TRANSACTION_FLAG, CREATED_BY,
CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, UPDATE_SEQ)
VALUES (161, 1017, 'ACH Pull Automation - IBS', NULL, 1, 'Y', 'N', 'Adhoc - ACH Pull and New Hire Automation',
GETDATE(), 'Adhoc - ACH Pull and New Hire Automation', GETDATE(), 0)
END
GO
