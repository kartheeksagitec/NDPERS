/********************Purpose: PIR 22835 ******************************
*********************Created By: Vidya********************************
*********************Comments: *****************/
IF NOT EXISTS (SELECT 1 FROM [dbo].[SGS_CODE_VALUE] WHERE CODE_ID = 3506 AND CODE_VALUE ='FMLA') 
BEGIN
	INSERT INTO [SGS_CODE_VALUE] VALUES(3506,	'FMLA',	'Family and Medical Leave Act',	NULL,	NULL,	NULL,	NULL,	NULL,	NULL,	NULL,	NULL,	'PIR 22835',	GETDATE(),	'PIR 22835',	GETDATE(),0)
END
GO