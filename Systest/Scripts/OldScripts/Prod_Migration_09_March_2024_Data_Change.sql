--------------Created By: Vidya Fulsoundar
--------------Purpose   : PIR 25782
--------------Date      : 02/19/2024

IF NOT EXISTS (SELECT 1 FROM [dbo].[SGW_PROCESS] WHERE [PROCESS_ID] = 376)
BEGIN
INSERT INTO [DBO].[SGW_PROCESS] ([PROCESS_ID], [DESCRIPTION], [NAME], [PRIORITY], [TYPE_ID], [TYPE_VALUE], [STATUS_ID], [STATUS_VALUE], [USE_NEW_MAP_FLAG], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES(376,'Process Marital Status Change',	'nfmProcessMaritalStatusChange',	0,	1603,	'PERS',	5003,	'ACT',	NULL,	'PIR 25782',	getdate(),	'PIR 25782',	getdate(),	0)
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[SGW_BPM_PROCESS_ID_DESC] WHERE [PROCESS_ID] = 376 AND [DESCRIPTION] = 'Process Marital Status Change')
BEGIN
INSERT INTO [DBO].[SGW_BPM_PROCESS_ID_DESC] ([PROCESS_ID], [DESCRIPTION])
VALUES(376, 'Process Marital Status Change')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[SGW_ACTIVITY] WHERE [ACTIVITY_ID] = 343)
BEGIN
INSERT INTO [DBO].[SGW_ACTIVITY] ([ACTIVITY_ID],[PROCESS_ID], [NAME], [DISPLAY_NAME], [STANDARD_TIME_IN_MINUTES], [ROLE_ID], [SUPERVISOR_ROLE_ID], [SORT_ORDER], [IS_DELETED_FLAG], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES(343, 376,	'Update Enrollment and Dependents',	'Update Enrollment and Dependents',	300, 143,	148,	1,	NULL,	'PIR 25782',	getdate(),	'PIR 25782',	getdate(),	0)
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[SGW_ACTIVITY] WITH(NOLOCK) WHERE [ACTIVITY_ID] = 345)
BEGIN
INSERT INTO [DBO].[SGW_ACTIVITY] ([ACTIVITY_ID],[PROCESS_ID], [NAME], [DISPLAY_NAME], [STANDARD_TIME_IN_MINUTES], [ROLE_ID], [SUPERVISOR_ROLE_ID], [SORT_ORDER], [IS_DELETED_FLAG], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES(345, 376,'Audit Enrollment and Dependent Update','Audit Enrollment and Dependent Update',	300, 142,	148,	2,	NULL,	'PIR 25782',	getdate(),	'PIR 25782',	getdate(),	0)
END
/********************Purpose: PIR 26255******************************
*********************Created By: Sarvesh Ghante********************************
*********************Comments: Updated the column to change Associated form*****************/

BEGIN TRANSACTION Trans3
IF EXISTS (SELECT 1 FROM [DBO].[SGS_COR_TEMPLATES] WHERE TEMPLATE_ID = 468 AND TEMPLATE_NAME = 'PER-0353')
BEGIN
UPDATE [DBO].[SGS_COR_TEMPLATES] SET ASSOCIATED_FORMS = 'wfmPersonMaintenance;',MODIFIED_BY = 'PIR 26255',MODIFIED_DATE = GETDATE()  
WHERE TEMPLATE_ID = 468 AND TEMPLATE_NAME = 'PER-0353'
END
COMMIT TRANSACTION Trans3

/********************Purpose: PIR 26285******************************
*********************Created By: Sarvesh Ghante********************************
*********************Comments: Updated the column to change Associated form*****************/

IF NOT EXISTS (SELECT 1 FROM [dbo].[SGS_CODE_VALUE] WHERE [CODE_ID] = 332 AND CODE_VALUE = 'CSDE')
BEGIN
INSERT INTO [DBO].[SGS_CODE_VALUE] ([CODE_ID], [CODE_VALUE], [DESCRIPTION], [DATA1], [DATA2], [DATA3], [COMMENTS], [START_DATE], [END_DATE], [CODE_VALUE_ORDER],
							[LEGACY_CODE_ID], [CREATED_BY],	[CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES(332, 'CSDE', 'Spouse’s or dependent’s eligibility for medical flex or health insurance through own employer', 'Y', 'Y', NULL, NULL, NULL,
         NULL, NULL, NULL, 'PIR 26285', GETDATE(), 'PIR 26285', GETDATE(), 0 )
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[SGT_WSS_AUTO_POSTING_CROSS_REF] WHERE [AUTO_POSTING_CROSS_REF_ID] = 160)
BEGIN
INSERT INTO [DBO].[SGT_WSS_AUTO_POSTING_CROSS_REF] ([AUTO_POSTING_CROSS_REF_ID], [PLAN_ID], [PLAN_ENROLLMENT_OPTION_ID], [PLAN_ENROLLMENT_OPTION_VALUE], [CHANGE_REASON_ID], [CHANGE_REASON_VALUE], [CHANGE_EFFECTIVE_DATE_ID], [CHANGE_EFFECTIVE_DATE_VALUE], [AUTO_POST_FLAG], [WORKFLOW_PROCESS_ID], [PROMPT_USER_TEXT], [DOCUMENT])
VALUES(160, 18, 6003, 'QCIS', 332, 'CSDE', 6005, 'NOAP', 'N', 351, 'Your spouse or dependent gains eligibility for medical flexible spending account or health insurance through his/her employer.<br/>You may only decrease or terminate participation in your medical flexible spending account coverage with this qualifying event.', 'Flexcomp Change in Status SFN 53511')
END

BEGIN TRANSACTION Trans1
IF EXISTS (SELECT 1 FROM [DBO].[SGS_CODE_VALUE] WHERE [CODE_SERIAL_ID] = 16190)
BEGIN
UPDATE [DBO].[SGS_CODE_VALUE] SET [DESCRIPTION] = 'Change in participant’s marital status', MODIFIED_BY = 'PIR 26285',MODIFIED_DATE = GETDATE()  
WHERE CODE_SERIAL_ID = 16190 
END
COMMIT TRANSACTION Trans1

BEGIN TRANSACTION Trans2
IF EXISTS (SELECT 1 FROM [DBO].[SGS_CODE_VALUE] WHERE [CODE_SERIAL_ID] = 16191)
BEGIN
UPDATE [DBO].[SGS_CODE_VALUE] SET [DESCRIPTION] = 'Change in number of participant’s dependent children', MODIFIED_BY = 'PIR 26285',MODIFIED_DATE = GETDATE()  
WHERE CODE_SERIAL_ID = 16191
END
COMMIT TRANSACTION Trans2

BEGIN TRANSACTION Trans4
IF EXISTS (SELECT 1 FROM [DBO].[SGS_CODE_VALUE] WHERE [CODE_SERIAL_ID] = 16194)
BEGIN
UPDATE [DBO].[SGS_CODE_VALUE] SET [DESCRIPTION] = 'Certain judgments, decrees, orders', MODIFIED_BY = 'PIR 26285',MODIFIED_DATE = GETDATE()  
WHERE CODE_SERIAL_ID = 16194
END
COMMIT TRANSACTION Trans4

BEGIN TRANSACTION Trans5
IF EXISTS (SELECT 1 FROM [DBO].[SGS_CODE_VALUE] WHERE [CODE_SERIAL_ID] = 16195)
BEGIN
UPDATE [DBO].[SGS_CODE_VALUE] SET [DESCRIPTION] = 'Entitlement to Medicare or Medicaid', MODIFIED_BY = 'PIR 26285',MODIFIED_DATE = GETDATE()  
WHERE CODE_SERIAL_ID = 16195
END
COMMIT TRANSACTION Trans5

BEGIN TRANSACTION Trans6
IF EXISTS (SELECT 1 FROM [DBO].[SGT_WSS_AUTO_POSTING_CROSS_REF] WHERE [AUTO_POSTING_CROSS_REF_ID] = 73)
BEGIN
UPDATE [DBO].[SGT_WSS_AUTO_POSTING_CROSS_REF] SET [PROMPT_USER_TEXT] = 'There is a change in your employment status or the employment status of your spouse or any dependents.<br/>The employment status change must affect eligibility under this Plan or a plan maintained by the employer of your spouse<br/>or dependent due to termination of employment, a change from full-time to part-time or part-time to full-time employment, or return from an unpaid leave of absence.<br/>If you change employment status from full-time to part-time or part-time to full-time, your election change must correspond with the gain or loss of coverage.<br/>If your spouse or dependents have an employment status change that affects eligibility under their employer’s plan and coverage is lost, then you may increase coverage under this Plan.<br/>If the status change results in your spouse or dependents gaining coverage under their employer’s plan, you may decrease or terminate coverage under this Plan.'  
WHERE AUTO_POSTING_CROSS_REF_ID = 73
END
COMMIT TRANSACTION Trans6

BEGIN TRANSACTION Trans7
IF EXISTS (SELECT 1 FROM [DBO].[SGT_WSS_AUTO_POSTING_CROSS_REF] WHERE [AUTO_POSTING_CROSS_REF_ID] = 72)
BEGIN
UPDATE [DBO].[SGT_WSS_AUTO_POSTING_CROSS_REF] SET [PROMPT_USER_TEXT] = 'Your legal marital status changes through marriage, divorce, death of spouse, legal separation, or annulment.'
WHERE AUTO_POSTING_CROSS_REF_ID = 72
END
COMMIT TRANSACTION Trans7

BEGIN TRANSACTION Trans8
IF EXISTS (SELECT 1 FROM [DBO].[SGT_WSS_AUTO_POSTING_CROSS_REF] WHERE [AUTO_POSTING_CROSS_REF_ID] = 114)
BEGIN
UPDATE [DBO].[SGT_WSS_AUTO_POSTING_CROSS_REF] SET [PROMPT_USER_TEXT] = 'One of your dependents satisfies or ceases to satisfy the requirement for coverage under the Health Insurance Plan.<br/>For dependents, attainment of age, a change in student status, or marital status would allow you to<br/>make a corresponding change to increase or decrease coverage under this Plan for the dependent.'
WHERE AUTO_POSTING_CROSS_REF_ID = 114
END
COMMIT TRANSACTION Trans8

BEGIN TRANSACTION Trans9
IF EXISTS (SELECT 1 FROM [DBO].[SGT_WSS_AUTO_POSTING_CROSS_REF] WHERE [AUTO_POSTING_CROSS_REF_ID] = 74)
BEGIN
UPDATE [DBO].[SGT_WSS_AUTO_POSTING_CROSS_REF] SET [PROMPT_USER_TEXT] = 'You are served with a judgment, decree or court order. This includes divorce, legal separation, annulment,<br/>or change in legal custody (including a qualified medical child support order) that requires health coverage for your child.<br/>It would allow you to make an election change to your Medical Spending Account. The change is allowed in order to provide coverage<br/>for the child if the order requires coverage under your Plan; or cancel coverage for the child if the order requires your former spouse to provide coverage.'
WHERE AUTO_POSTING_CROSS_REF_ID = 74
END
COMMIT TRANSACTION Trans9

BEGIN TRANSACTION Trans10
IF EXISTS (SELECT 1 FROM [DBO].[SGT_WSS_AUTO_POSTING_CROSS_REF] WHERE [AUTO_POSTING_CROSS_REF_ID] = 117)
BEGIN
UPDATE [DBO].[SGT_WSS_AUTO_POSTING_CROSS_REF] SET [PROMPT_USER_TEXT] = 'You, your spouse, or any of your dependents become eligible for coverage under Medicare or Medicaid.<br/>Your election change must correspond with the gain of coverage'  
WHERE AUTO_POSTING_CROSS_REF_ID = 117
END
COMMIT TRANSACTION Trans10

--------------Created By: Sanket Chougale
--------------Purpose   : PIR 19158
--------------Date      : 01/29/2024

IF NOT EXISTS (SELECT * FROM [dbo].SGS_BATCH_SCHEDULE WHERE BATCH_SCHEDULE_ID = 163)
INSERT INTO [DBO].[SGS_BATCH_SCHEDULE] ([BATCH_SCHEDULE_ID], [STEP_NO], [STEP_NAME], [STEP_DESCRIPTION], [FREQUENCY_IN_DAYS], [FREQUENCY_IN_MONTHS], [NEXT_RUN_DATE], [STEP_PARAMETERS], [ACTIVE_FLAG],[REQUIRES_TRANSACTION_FLAG],[EMAIL_NOTIFICATION],[ORDER_NO],[CUTOFF_START],[CUTOFF_END], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES(163, 1019, 'Missing RHIC', NULL,NULL,NULL,'2024-01-06 00:00:00.000',NULL,'Y','N',NULL,NULL,NULL,NULL,	'PIR 19158',	getdate(),	'PIR 19158',	getdate(),	0)
GO


--------------Created By: Sanket Chougale
--------------Purpose   : PIR 19158
--------------Date      : 01/29/2024

IF NOT EXISTS (SELECT * FROM [dbo].[SGW_PROCESS] WHERE PROCESS_ID = 378)
INSERT INTO [DBO].[SGW_PROCESS] ([PROCESS_ID], [DESCRIPTION], [NAME], [PRIORITY], [TYPE_ID], [TYPE_VALUE], [STATUS_ID], [STATUS_VALUE], [USE_NEW_MAP_FLAG], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES(378,'Process Missing RHIC Record',	'nfmProcessMissingRHICRecord',	0,	1603,	'PERS',	5003,	'ACT',	NULL,	'PIR 19158',	getdate(),	'PIR 19158',	getdate(),	0)
GO
 
IF NOT EXISTS (SELECT * FROM [dbo].[SGW_BPM_PROCESS_ID_DESC] WHERE PROCESS_ID = 378)
INSERT INTO [DBO].[SGW_BPM_PROCESS_ID_DESC] ([PROCESS_ID], [DESCRIPTION])
VALUES(378, 'Process Missing RHIC Record')
GO
 
IF NOT EXISTS (SELECT * FROM [dbo].[SGW_ACTIVITY] WHERE ACTIVITY_ID = 345)
INSERT INTO [DBO].[SGW_ACTIVITY] ([ACTIVITY_ID], [PROCESS_ID], [NAME], [DISPLAY_NAME], [STANDARD_TIME_IN_MINUTES], [ROLE_ID], [SUPERVISOR_ROLE_ID], [SORT_ORDER], [IS_DELETED_FLAG], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES(345, 378, 'Process Missing RHIC Record', 'Process Missing RHIC Record',	300, 140,	148,	1,	NULL,	'PIR 19158',	getdate(),	'PIR 19158',	getdate(),	0)
GO

--------------Created By: Sanket Chougale
--------------Purpose   : PIR 19158
--------------Date      : 01/29/2024

IF NOT EXISTS (SELECT * FROM [dbo].[SGS_MESSAGES] WHERE MESSAGE_ID = 10500)
INSERT INTO [DBO].[SGS_MESSAGES] ([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[INTERNAL_INSTRUCTIONS],[EMPLOYER_INSTRUCTIONS],[CREATED_BY],
[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES (10500,'Remember to reinstate RHIC benefits',16,'E',NULL,NULL,'PIR 19158',GETDATE(),'PIR 19158',GETDATE(),0)
GO
