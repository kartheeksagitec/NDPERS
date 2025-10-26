IF NOT EXISTS (SELECT 1 FROM [dbo].[SGT_PAYMENT_STEP_REF] WHERE BATCH_SCHEDULE_ID = 80 and SCHEDULE_TYPE_VALUE = 'MTLY' AND RUN_SEQUENCE = 295)
INSERT INTO [dbo].[SGT_PAYMENT_STEP_REF]
           ([STEP_NAME]
           ,[ACTIVE_FLAG]
           ,[SCHEDULE_TYPE_ID]
           ,[SCHEDULE_TYPE_VALUE]
           ,[TRIAL_RUN_FLAG]
           ,[MAIN_STEP_ID]
           ,[RUN_SEQUENCE]
           ,[BATCH_SCHEDULE_ID]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           ('Refunds With Payroll Adjustments Report'
           ,'Y'
           ,2501
           ,'MTLY'
           ,'Y'
           ,2
           ,295
           ,80
           ,'PIR_24308'
           ,GETDATE()
           ,'PIR_24308'
           ,GETDATE()
           ,0)
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[SGT_PAYMENT_STEP_REF] WHERE BATCH_SCHEDULE_ID = 86 and SCHEDULE_TYPE_VALUE = 'ADHC' AND RUN_SEQUENCE = 1795)
INSERT INTO [dbo].[SGT_PAYMENT_STEP_REF]
           ([STEP_NAME]
           ,[ACTIVE_FLAG]
           ,[SCHEDULE_TYPE_ID]
           ,[SCHEDULE_TYPE_VALUE]
           ,[TRIAL_RUN_FLAG]
           ,[MAIN_STEP_ID]
           ,[RUN_SEQUENCE]
           ,[BATCH_SCHEDULE_ID]
           ,[CREATED_BY]
           ,[CREATED_DATE]
           ,[MODIFIED_BY]
           ,[MODIFIED_DATE]
           ,[UPDATE_SEQ])
     VALUES
           ('Refunds With Payroll Adjustments Report'
           ,'Y'
           ,2501
           ,'ADHC'
           ,'Y'
           ,44
           ,1795
           ,86
           ,'PIR_24308'
           ,GETDATE()
           ,'PIR_24308'
           ,GETDATE()
           ,0)
GO

/********************Purpose: PIR 23944******************************
*********************Created By: Saylee Pujari********************************
*********************Comments: Updating initial_enroll_date to Person Account Start Date *****************/


UPDATE TMP SET MODIFIED_BY = 'PIR 23944', MODIFIED_DATE = GETDATE() ,TMP.INITIAL_ENROLL_DATE =
(
SELECT TOP 1 PA.START_DATE FROM SGT_PERSON_ACCOUNT PA 
WHERE PA.PERSON_ACCOUNT_ID = TMP.PERSON_ACCOUNT_ID
AND PLAN_ID = 9
)
FROM SGT_PERSON_ACCOUNT_MEDICARE_PART_D_HISTORY TMP WITH (NOLOCK)
WHERE INITIAL_ENROLL_DATE IS NULL

/********************Purpose: PIR 24309******************************
*********************Created By: Surendra Reddy********************************
*********************Comments: Msg for member person id not having address*****************/

IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID=10438)
	INSERT INTO SGS_MESSAGES (MESSAGE_ID,DISPLAY_MESSAGE,SEVERITY_ID,SEVERITY_VALUE,INTERNAL_INSTRUCTIONS,EMPLOYER_INSTRUCTIONS,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
    VALUES(10438,'Address not on file and must be entered to update.',16,'E',NULL,NULL,'PIR 24309',GETDATE(),'PIR 24309',GETDATE(),0)
GO