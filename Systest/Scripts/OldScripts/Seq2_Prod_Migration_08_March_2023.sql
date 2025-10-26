
/********************Purpose: PIR 24893******************************
*********************Created By: Bharat Reddy********************************
*********************Comments: New code value for Org Maintenance Screen*****************/

IF NOT EXISTS (SELECT 1 FROM SGS_CODE_VALUE WHERE CODE_ID = 7021 AND CODE_VALUE = 'OCOM')
INSERT INTO [dbo].[SGS_CODE_VALUE]
           ([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3],[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES
           (7021 ,'OCOM','Org Contact Maintenance Screen',null,null,null,null,null,null,null,null,'PIR 24893',GETDATE(),'PIR 24893',GETDATE(),0)
GO

/*********************Comments: New role added*****************/

IF NOT EXISTS(SELECT 1 FROM SGS_ROLES WHERE ROLE_DESCRIPTION = 'Delete Notes - Org Contact')
BEGIN

INSERT INTO SGS_ROLES (ROLE_DESCRIPTION,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
VALUES ('Delete Notes - Org Contact','PIR 24893',GETDATE(),'PIR 24893',GETDATE(),0)

END

/*********************Comments: New resource addded*****************/

BEGIN
DECLARE @cnt INT
BEGIN TRANSACTION Trans
SELECT @cnt = count(*) FROM SGS_RESOURCES  WHERE RESOURCE_ID=2085 AND RESOURCE_DESCRIPTION='Org Contact - Notes - Delete'; 
   if @cnt = 0   
   Begin    
       INSERT INTO SGS_RESOURCES ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
       ,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
  VALUES
           (2085,12,'U','Org Contact - Notes - Delete','PIR 24893',GETDATE(),'PIR 24893',GETDATE(),0)
   END
   ELSE 
   PRINT 'RESOURCE_ID 2085 and RESOURCE_DESCRIPTION Org Contact - Notes - Delete is alredy exist in SGS_RESOURCES table';
 COMMIT TRANSACTION Trans
 END
 
/*********************Comments: Linked new resource to new role*****************/

BEGIN TRANSACTION Trans2
IF NOT EXISTS(SELECT 1 FROM SGS_SECURITY WHERE RESOURCE_ID = 2085)
BEGIN
INSERT INTO SGS_SECURITY
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'PIR 24893', GETDATE(), 'PIR 24893', GETDATE(), 0
  FROM SGS_ROLES ROL, SGS_RESOURCES RES
 WHERE NOT EXISTS (SELECT ROLE_ID 
                     FROM SGS_SECURITY SEC
                    WHERE ROL.ROLE_ID = SEC.ROLE_ID
                      AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2085
END
COMMIT TRANSACTION Trans2

--BEGIN TRANSACTION TRANS3
--IF EXISTS(SELECT 1 FROM SGS_SECURITY WHERE RESOURCE_ID = 2085 AND SECURITY_VALUE = 0)
--BEGIN
--UPDATE SGS_SECURITY SET SECURITY_VALUE = 1 WHERE RESOURCE_ID = 2085 AND SECURITY_VALUE = 0
--END
--COMMIT TRANSACTION TRANS3

/********************Purpose: PIR 24893******************************
*********************Created By: Bharat Reddy********************************
*********************Comments: New code value for Org Maintenance Screen*****************/
IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID=10480)
	INSERT INTO SGS_MESSAGES (MESSAGE_ID,DISPLAY_MESSAGE,SEVERITY_ID,SEVERITY_VALUE,INTERNAL_INSTRUCTIONS,EMPLOYER_INSTRUCTIONS,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
    VALUES(10480,'Role is required – at least one role must be selected',16,'E',NULL,NULL,'PIR 24893',GETDATE(),'PIR 24893',GETDATE(),0)
GO

IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID=10481)
	INSERT INTO SGS_MESSAGES (MESSAGE_ID,DISPLAY_MESSAGE,SEVERITY_ID,SEVERITY_VALUE,INTERNAL_INSTRUCTIONS,EMPLOYER_INSTRUCTIONS,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
    VALUES(10481,'Plan is required – at least one plan must be selected',16,'E',NULL,NULL,'PIR 24893',GETDATE(),'PIR 24893',GETDATE(),0)
GO

/********************Purpose: PIR 25536******************************
*********************Created By: Abhijeet M********************************
*********************Comments: MSS Member email authentication *****************/
IF NOT EXISTS (SELECT 1 FROM SGS_CODE_VALUE WHERE CODE_ID = 7013 AND CODE_VALUE = 'MMAO')
INSERT INTO [dbo].[SGS_CODE_VALUE]
           ([CODE_ID],[CODE_VALUE],[DESCRIPTION],[DATA1],[DATA2],[DATA3],[COMMENTS],[START_DATE],[END_DATE],[CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
     VALUES
           (7013 ,'MMAO','MSS Member Authentication OTP Source',null,null,null,null,null,null,null,null,'PIR 25536',GETDATE(),'PIR 25536',GETDATE(),0)
GO 

/********************Purpose: PIR 24893******************************
*********************Created By: Abhijeet********************************
*********************Comments: *****************/
INSERT SGS_USER_ROLES VALUES (165, 216, '2023-01-01 00:00:00.000', null, 'PIR_24893', GETDATE(), 'PIR_24893', GETDATE(), 0)
INSERT SGS_USER_ROLES VALUES (124, 216, '2023-01-01 00:00:00.000', null, 'PIR_24893', GETDATE(), 'PIR_24893', GETDATE(), 0)
INSERT SGS_USER_ROLES VALUES (117, 216, '2023-01-01 00:00:00.000', null, 'PIR_24893', GETDATE(), 'PIR_24893', GETDATE(), 0)


UPDATE SGS_SECURITY  SET SECURITY_VALUE = 5 WHERE ROLE_ID = 216 AND RESOURCE_ID = 2085

/********************Purpose: PIR 25548****************************************
*********************Created By: Surendra Reddy********************************
*********************Comments: GL Entries For RMD taxes************************
*********************P081 RMD Federal Tax Amount*******************************
*********************P082 RMD State Tax Amount*********************************
*********************P012 RMD Federal Tax Amount*******************************
*********************P013 RMD State Tax Amount*********************************/
INSERT INTO [dbo].[SGT_ACCOUNT_REFERENCE]
           ([PLAN_ID]
           ,[FUND_ID]
           ,[FUND_VALUE]
           ,[DEPT_ID]
           ,[DEPT_VALUE]
           ,[ITEM_TYPE_ID]
           ,[ITEM_TYPE_VALUE]
           ,[FROM_ITEM_TYPE_ID]
           ,[FROM_ITEM_TYPE_VALUE]
           ,[TO_ITEM_TYPE_ID]
           ,[TO_ITEM_TYPE_VALUE]
           ,[TRANSACTION_TYPE_ID]
           ,[TRANSACTION_TYPE_VALUE]
           ,[STATUS_TRANSITION_ID]
           ,[STATUS_TRANSITION_VALUE]
           ,[SOURCE_TYPE_ID]
           ,[SOURCE_TYPE_VALUE]
           ,[GENERATE_GL_FLAG]
           ,[DEBIT_ACCOUNT_ID]
           ,[CREDIT_ACCOUNT_ID]
           ,[JOURNAL_DESCRIPTION]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
SELECT		[PLAN_ID]
           ,[FUND_ID]
           ,[FUND_VALUE]
           ,[DEPT_ID]
           ,[DEPT_VALUE]
           ,[ITEM_TYPE_ID]
           ,CASE WHEN [ITEM_TYPE_VALUE] = 'P012' THEN 'P081' ELSE 'P082' END AS [ITEM_TYPE_VALUE]
           ,[FROM_ITEM_TYPE_ID]
           ,[FROM_ITEM_TYPE_VALUE]
           ,[TO_ITEM_TYPE_ID]
           ,[TO_ITEM_TYPE_VALUE]
           ,[TRANSACTION_TYPE_ID]
           ,[TRANSACTION_TYPE_VALUE]
           ,[STATUS_TRANSITION_ID]
           ,[STATUS_TRANSITION_VALUE]
           ,[SOURCE_TYPE_ID]
           ,[SOURCE_TYPE_VALUE]
           ,[GENERATE_GL_FLAG]
           ,[DEBIT_ACCOUNT_ID]
           ,[CREDIT_ACCOUNT_ID]
           ,[JOURNAL_DESCRIPTION]
           ,'PIR_25548'
           ,GETDATE()
           ,'PIR_25548'
           ,GETDATE()
           ,0
FROM 
	SGT_ACCOUNT_REFERENCE 
WHERE 
	SOURCE_TYPE_VALUE = 'BEPY' AND 
	ITEM_TYPE_VALUE IN ('P012', 'P013')

	
/********************Purpose: PIR 25481******************************
*********************Created By: Sarvesh G********************************
*********************Comments: *****************/

IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID=10482)
	INSERT INTO SGS_MESSAGES (MESSAGE_ID,DISPLAY_MESSAGE,SEVERITY_ID,SEVERITY_VALUE,INTERNAL_INSTRUCTIONS,EMPLOYER_INSTRUCTIONS,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
    VALUES(10482,'Purchase amount reported Is exactly half of the Expected Installment amount',16,'E',NULL,NULL,'PIR 25481',GETDATE(),'PIR 25481',GETDATE(),0)
GO

/********************Purpose: PIR 24834&& PIR 24755******************************
*********************Created By: Sarvesh Ghante********************************
*********************Comments: *****************/
IF EXISTS (SELECT 1 FROM SGS_COR_TEMPLATES where  TEMPLATE_ID = 500)
BEGIN
update SGS_COR_TEMPLATES set ASSOCIATED_FORMS = 'wfmPersonMaintenance;wfmPaymentElectionAdjustmentMaintenance;wfmPremiumDetailLTDMaintenance;wfmHealthDentalVisonMaintenance;' 
where TEMPLATE_ID = 500
END
GO