/********************Purpose: PIR 26948******************************
*********************Created By: Bharat Reddy********************************/

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE [Name] = N'PLAN_NAME' AND Object_ID = Object_ID(N'dbo.SGT_WELCOME_ORG_BENEFIT_PLAN'))
BEGIN
    ALTER TABLE dbo.[SGT_WELCOME_ORG_BENEFIT_PLAN]
	ADD PLAN_NAME VARCHAR(200) NULL 
END
GO

/********************Purpose: PIR 27148******************************
*********************Created By: Sanket Chougale********************************/

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE [Name] = N'EMPLOYEE_NEVER_STARTED' AND Object_ID = Object_ID(N'dbo.SGT_WSS_EMPLOYMENT_CHANGE_REQUEST'))
BEGIN
    ALTER TABLE [DBO].[SGT_WSS_EMPLOYMENT_CHANGE_REQUEST]
	ADD EMPLOYEE_NEVER_STARTED varchar(1) NULL 
END
GO