/********************Purpose: PIR 24035 Line of Duty Survivors******************************
*********************Created By: Saylee P********************************
*********************Comments: *****************/

--PIR 24035 

IF NOT EXISTS (SELECT 1 FROM [dbo].[SGS_CODE] WHERE CODE_ID = 7022) 
BEGIN
INSERT [dbo].[SGS_CODE] ([CODE_ID], [DESCRIPTION], [DATA1_CAPTION], [DATA1_TYPE], [DATA2_CAPTION], [DATA2_TYPE], [DATA3_CAPTION], [DATA3_TYPE], [FIRST_LOOKUP_ITEM], 
[FIRST_MAINTENANCE_ITEM], [COMMENTS], [LEGACY_CODE_ID], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ]) 
VALUES (7022, N'Line of Duty Survivors Organizations', N'Org Code', N'str ', NULL, NULL, NULL, NULL, N'All', NULL, NULL, NULL, 'PIR 24035', 
GETDATE(), 'PIR 24035', GETDATE(), 1)
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[SGS_CODE_VALUE] WHERE CODE_ID = 7022 AND CODE_VALUE = 'LDSV') 
BEGIN
INSERT [dbo].[SGS_CODE_VALUE] ([CODE_ID], [CODE_VALUE], [DESCRIPTION], [DATA1], [DATA2], [DATA3], [COMMENTS], [START_DATE], [END_DATE],
 [CODE_VALUE_ORDER], [LEGACY_CODE_ID], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ]) 
 VALUES (7022, N'LDSV', N'Line of Duty Survivors Non-Wellness', N'500139', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'PIR 24035', 
 GETDATE(), 'PIR 24035', GETDATE(), 0)
END
GO


IF NOT EXISTS (SELECT 1 FROM [dbo].[SGW_PROCESS] WHERE PROCESS_ID=375)
INSERT INTO [dbo].[SGW_PROCESS] (PROCESS_ID,DESCRIPTION,NAME,PRIORITY,TYPE_ID,TYPE_VALUE,STATUS_ID,STATUS_VALUE,USE_NEW_MAP_FLAG,CREATED_BY,
CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
VALUES
(375,'Maintain Line of Duty Survivor','nfmMaintainLineOfDutySurvivor',0,1603,'PERS',5003,'ACT',NULL,'PIR 24035',GETDATE(),'PIR 24035',
GETDATE(),0)
GO


IF NOT EXISTS (SELECT * FROM SGW_ACTIVITY WHERE PROCESS_ID = 375 AND ACTIVITY_ID = 342) 
INSERT [SGW_ACTIVITY] ([ACTIVITY_ID], [PROCESS_ID], [NAME], [DISPLAY_NAME], [STANDARD_TIME_IN_MINUTES], [ROLE_ID], [SUPERVISOR_ROLE_ID], [SORT_ORDER], 
[IS_DELETED_FLAG], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ]) 
VALUES (342, 375, N'Research age of Line of Duty Survivor', N'Research age of Line of Duty Survivor', 0, 144, 0, 1, NULL, 'Studio', GETDATE(), 
'Studio', GETDATE(), 0)
GO


IF NOT EXISTS (SELECT 1 FROM [SGS_BATCH_SCHEDULE] WHERE BATCH_SCHEDULE_ID = 156)
INSERT INTO [SGS_BATCH_SCHEDULE] (BATCH_SCHEDULE_ID,STEP_NO,STEP_NAME,STEP_DESCRIPTION,FREQUENCY_IN_DAYS,FREQUENCY_IN_MONTHS,NEXT_RUN_DATE,STEP_PARAMETERS,ACTIVE_FLAG,	
REQUIRES_TRANSACTION_FLAG,EMAIL_NOTIFICATION,ORDER_NO,CUTOFF_START,CUTOFF_END,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
VALUES(156,1012,'Line of Duty Survivors',NULL,NULL,1,GETDATE(),NULL,'Y','N',NULL,NULL,NULL,NULL,	
'PIR 24035',GETDATE(),'PIR 24035',GETDATE(),0)
GO

IF NOT EXISTS (SELECT 1 FROM SGS_CODE_VALUE WHERE CODE_ID = 1302 AND CODE_VALUE = 'HLUG')
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
           (1302
           ,'HLUG'
           ,'Health/Life Underwriting Gain'
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,'PIR_24035'
           ,GETDATE()
           ,'PIR_24035'
           ,GETDATE()
           ,0)
GO

IF NOT EXISTS (SELECT 1 FROM [SGT_ACCOUNT_REFERENCE] WHERE PLAN_ID = 12 AND SOURCE_TYPE_VALUE = 'EMPR' AND TRANSACTION_TYPE_VALUE = 'ITLV' AND ITEM_TYPE_VALUE = 'HLUG')
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
     VALUES
           (12
           ,1300
           ,'470'
           ,1301
           ,'2001'
           ,1302
           ,'HLUG'
           ,1302
           ,NULL
           ,1302
           ,NULL
           ,1303
           ,'ITLV'
           ,1309
           ,NULL
           ,1304
           ,'EMPR'
           ,'1'
           ,58
           ,10
           ,'Employer Reporting Line of Duty Survivor'
           ,'PIR_24035'
           ,GETDATE()
           ,'PIR_24035'
           ,GETDATE()
           ,0)
GO

/********************************************************************************
************************ New resource Board Member Election ****************************
*********************************************************************************/

BEGIN
DECLARE @cnt INT
BEGIN TRANSACTION Trans
SELECT @cnt = count(*) FROM SGS_RESOURCES  WHERE RESOURCE_ID=2061 AND RESOURCE_DESCRIPTION='Board Member Election'; 
   if @cnt = 0   
   Begin    
       INSERT INTO SGS_RESOURCES ([RESOURCE_ID],[RESOURCE_TYPE_ID],[RESOURCE_TYPE_VALUE],[RESOURCE_DESCRIPTION],[CREATED_BY],[CREATED_DATE]
       ,[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
  VALUES
           (2061,12,'F','Board Member Election','PIR 24460',GETDATE(),'PIR 24460',GETDATE(),0)
   END
   ELSE 
   PRINT 'RESOURCE_ID 2061 and RESOURCE_DESCRIPTION Show System Preferences is alredy exist in SGS_RESOURCES table';
 COMMIT TRANSACTION Trans
 END

 --------------------------------------------------------------------------------------------------------------
 /* Insert query to assign New Resources to all ROLES */
 ---------------------------------------------------------------------------------------------------------------
 
BEGIN TRANSACTION Trans2
IF NOT EXISTS(SELECT 1 FROM SGS_SECURITY WHERE RESOURCE_ID = 2061)
BEGIN
INSERT INTO SGS_SECURITY
SELECT ROL.ROLE_ID, RES.RESOURCE_ID, 11, 0, 'PIR 24460', GETDATE(), 'PIR 24460', GETDATE(), 0
  FROM SGS_ROLES ROL, SGS_RESOURCES RES
 WHERE NOT EXISTS (SELECT ROLE_ID 
                     FROM SGS_SECURITY SEC
                    WHERE ROL.ROLE_ID = SEC.ROLE_ID
                      AND RES.RESOURCE_ID = SEC.RESOURCE_ID) AND RES.RESOURCE_ID = 2061
END
COMMIT TRANSACTION Trans2
