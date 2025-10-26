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
 
IF NOT EXISTS (SELECT * FROM [dbo].[SGW_ACTIVITY] WHERE ACTIVITY_ID = 346)
INSERT INTO [DBO].[SGW_ACTIVITY] ([ACTIVITY_ID], [PROCESS_ID], [NAME], [DISPLAY_NAME], [STANDARD_TIME_IN_MINUTES], [ROLE_ID], [SUPERVISOR_ROLE_ID], [SORT_ORDER], [IS_DELETED_FLAG], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES(346, 378, 'Process Missing RHIC Record', 'Process Missing RHIC Record',	300, 140,	148,	1,	NULL,	'PIR 19158',	getdate(),	'PIR 19158',	getdate(),	0)
GO

--------------Created By: Sanket Chougale
--------------Purpose   : PIR 19158
--------------Date      : 01/29/2024

IF NOT EXISTS (SELECT * FROM [dbo].[SGS_MESSAGES] WHERE MESSAGE_ID = 10500)
INSERT INTO [DBO].[SGS_MESSAGES] ([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[INTERNAL_INSTRUCTIONS],[EMPLOYER_INSTRUCTIONS],[CREATED_BY],
[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES (10500,'Remember to reinstate RHIC benefits',16,'E',NULL,NULL,'PIR 19158',GETDATE(),'PIR 19158',GETDATE(),0)
GO

--------------Created By: Sarvesh Ghante
--------------Purpose   : PIR 18061
--------------Date      : 03/13/2024
IF NOT EXISTS (SELECT * FROM [dbo].[SGS_COR_TEMPLATES] WHERE [TEMPLATE_NAME] = 'ORG-1504')
INSERT INTO [dbo].[SGS_COR_TEMPLATES] ([TEMPLATE_NAME], [TEMPLATE_DESC], [TEMPLATE_GROUP_ID], [TEMPLATE_GROUP_VALUE], [ACTIVE_FLAG], [DESTINATION_ID], [DESTINATION_VALUE], 
[ASSOCIATED_FORMS], [FILTER_OBJECT_ID], [FILTER_OBJECT_FIELD], [FILTER_OBJECT_VALUE], [CONTACT_ROLE_ID], [CONTACT_ROLE_VALUE], [BATCH_FLAG], [ONLINE_FLAG], [AUTO_PRINT_FLAG], [PRINTER_NAME_ID], 
[PRINTER_NAME_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ],
IMAGE_DOC_CATEGORY_ID,IMAGE_DOC_CATEGORY_VALUE,FILENET_DOCUMENT_TYPE_ID,FILENET_DOCUMENT_TYPE_VALUE,DOCUMENT_CODE) 
VALUES ('ORG-1504', 'Employee Waiver of Participation for Past Service', '19', 'MMBR', 'Y', '601', 'IMG', 'wfmOrganizationMaintenance;', NULL, 
		NULL, NULL, '515',NULL, 'N', 'Y', 'N', '44', NULL,  'PIR 18061', getdate(), 'PIR 18061', getdate(), '0',603,'MEMB',604,'FORM',1527)
GO
--------------Created By: Surendra
--------------Purpose   : PIR 26425
--------------Date      : 03/27/2024
IF EXISTS (SELECT 1 FROM SGT_BPM_ACTIVITY_ROLE_XR  WHERE ACTIVITY_ID = 707 AND ROLE_ID != 138)
BEGIN
	UPDATE SGT_BPM_ACTIVITY_ROLE_XR SET ROLE_ID = 138 WHERE ACTIVITY_ID = 707
END
GO


--------------Created By: Sanket Chougale
--------------Purpose   : PIR 19158
--------------Date      : 03/29/2024
IF NOT EXISTS (SELECT 1 FROM [dbo].[SGW_ACTIVITY] WITH(NOLOCK) WHERE [ACTIVITY_ID] = 347)
BEGIN
INSERT INTO [DBO].[SGW_ACTIVITY] ([ACTIVITY_ID],[PROCESS_ID], [NAME], [DISPLAY_NAME], [STANDARD_TIME_IN_MINUTES], [ROLE_ID], [SUPERVISOR_ROLE_ID], [SORT_ORDER], [IS_DELETED_FLAG], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES(347, 230,'Audit Medicare Age 65','Audit Medicare Age 65',300, 142,148,	null,	NULL,	'PIR 26111',	getdate(),	'PIR 26111',	getdate(),	0)
END

/********************Purpose: PIR 18493******************************
*********************Created By: Sanket Chougale********************************
*********************Comments: Updated the message *****************/
IF EXISTS (SELECT 1 FROM [DBO].[SGS_MESSAGES] WHERE [MESSAGE_ID] = 10436)
BEGIN
UPDATE [DBO].[SGS_MESSAGES] SET [DISPLAY_MESSAGE] = 'Your application has been successfully submitted and must be on file for at least 31 days prior to payment. Your payment date will be determined after your employer submits your termination date and your last monthly retirement contribution is received by NDPERS. Your payment will be issued in approximately 60 to 90 days, subject to tax notification requirements, and will be issued the first business day of the month. You will receive a letter from NDPERS regarding your date of distribution.',MODIFIED_BY = 'PIR 18493',MODIFIED_DATE = GETDATE()  
WHERE [MESSAGE_ID] = 10436
END
