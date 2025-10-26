/********************Purpose: PIR 26202******************************
*********************Created By: Bharat Reddy********************************
*********************Comments: New resource addded*****************/

BEGIN
DECLARE @cnt INT
BEGIN TRANSACTION Trans
SELECT @cnt = count(*) FROM [DBO].[SGS_RESOURCES]  WHERE RESOURCE_ID=2087 AND RESOURCE_DESCRIPTION='Payment History Distribution – Restricted Status Buttons'; 
   if @cnt = 0   
   Begin    
       INSERT INTO [DBO].[SGS_RESOURCES] ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
       ,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
  VALUES
           (2087,12,'U','Payment History Distribution – Restricted Status Buttons','PIR 26202',GETDATE(),'PIR 26202',GETDATE(),0)
   END
   ELSE 
   PRINT 'RESOURCE_ID 2087 and RESOURCE_DESCRIPTION Payment History Distribution – Restricted Status Buttons already exists in SGS_RESOURCES table';
 COMMIT TRANSACTION Trans
 END

/*********************Comments: New resource linked to role 139 and 140 with Execute action*****************/


BEGIN TRANSACTION Trans2

IF NOT EXISTS(SELECT 1 FROM [DBO].[SGS_SECURITY] WHERE RESOURCE_ID = 2087 AND ROLE_ID = 139)
BEGIN
INSERT INTO [DBO].[SGS_SECURITY]
VALUES(139, 2087, 11, 5, 'PIR 26202', GETDATE(), 'PIR 26202', GETDATE(), 0)
END

IF NOT EXISTS(SELECT 1 FROM SGS_SECURITY WHERE RESOURCE_ID = 2087 AND ROLE_ID NOT IN (139))
BEGIN
INSERT INTO [DBO].[SGS_SECURITY]([ROLE_ID], [RESOURCE_ID], [SECURITY_ID], [SECURITY_VALUE], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
SELECT [ROLE_ID], 2087, 11, 0, 'PIR 26202', GETDATE(), 'PIR 26202', GETDATE(), 0  FROM [DBO].[SGS_ROLES] WHERE ROLE_ID NOT IN (139,140)
END

COMMIT TRANSACTION Trans2


/********************Purpose: PIR 25652******************************
*********************Created By: Bharat Reddy********************************
*********************Comments: For New BPM nfmServicePurchasePaymentInstallmentsEmploymentChange*****************/


IF NOT EXISTS (SELECT * FROM [dbo].[SGW_PROCESS] WHERE PROCESS_ID = 377)
INSERT INTO [dbo].[SGW_PROCESS] ([PROCESS_ID], [DESCRIPTION], [NAME], [PRIORITY], [TYPE_ID], [TYPE_VALUE], [STATUS_ID], [STATUS_VALUE], [USE_NEW_MAP_FLAG], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES(377,'Service Purchase Payment Installments Employment Change',	'nfmServicePurchasePaymentInstallmentsEmploymentChange',	0,	1603,	'PERS',	5003,	'ACT',	NULL,	'PIR 25652',	getdate(),	'PIR 25652',	getdate(),	0)
GO

IF NOT EXISTS (SELECT * FROM [dbo].[SGW_BPM_PROCESS_ID_DESC] WHERE PROCESS_ID = 377)
INSERT INTO [dbo].[SGW_BPM_PROCESS_ID_DESC] ([PROCESS_ID], [DESCRIPTION])
VALUES(377, 'Service Purchase Payment Installments Employment Change')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[SGW_ACTIVITY] WHERE ACTIVITY_ID = 344)
INSERT INTO [dbo].[SGW_ACTIVITY] ([ACTIVITY_ID], [PROCESS_ID], [NAME], [DISPLAY_NAME], [STANDARD_TIME_IN_MINUTES], [ROLE_ID], [SUPERVISOR_ROLE_ID], [SORT_ORDER], [IS_DELETED_FLAG], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES(344, 377, 'Service Purchase Payment Installments Employment Change', 'Service Purchase Payment Installments Employment Change',	300, 140,	148,	1,	NULL,	'PIR 25652',	getdate(),	'PIR 25652',	getdate(),	0)
GO

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

--------------Created By: Saylee Pujari
--------------Purpose   : PIR 26051
--------------Date      : 01/29/2024
IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID=10498)
INSERT INTO SGS_MESSAGES (MESSAGE_ID,DISPLAY_MESSAGE,SEVERITY_ID,SEVERITY_VALUE,INTERNAL_INSTRUCTIONS,EMPLOYER_INSTRUCTIONS,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
VALUES(10498,'PeopleSoft ID already exists.',16,'E',NULL,NULL,'PIR 26051',GETDATE(),'PIR 26051',GETDATE(),0)
GO

