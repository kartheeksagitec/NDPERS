--------------Created By: Sarvesh Ghante
--------------Purpose   : PIR 26088
--------------Date      : 11/24/2023

IF NOT EXISTS (SELECT * FROM [dbo].[SGS_MESSAGES] WHERE MESSAGE_ID = 10497)
INSERT INTO [dbo].SGS_MESSAGES (MESSAGE_ID,DISPLAY_MESSAGE,SEVERITY_ID,SEVERITY_VALUE,INTERNAL_INSTRUCTIONS,EMPLOYER_INSTRUCTIONS,CREATED_BY,
CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
VALUES (10497,'Dual Meber exists - confirm eligibility',16,'E',NULL,NULL,'PIR 26088',GETDATE(),'PIR 26088',GETDATE(),0)
GO


--------------Created By: Surendra R
--------------Purpose   : Security Testing Issue
--------------Date      : 01/11/2024

INSERT [dbo].SGS_SYSTEM_SETTINGS(SETTING_NAME, SETTING_TYPE, SETTING_VALUE) values ('ParamsToSkipWhileLogging', 'string','ahstParameterNameValues.astrValue;ahstParameterNameValues.astrClientSecret;ahstParameterNameValues.astrClientId')


