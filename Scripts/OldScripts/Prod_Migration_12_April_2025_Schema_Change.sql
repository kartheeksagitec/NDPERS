/********************Purpose: PIR 26863******************************
*********************Created By: Sanket Chougale********************************/

IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE [Name] = N'BENEFIT_PERCENTAGE' AND Object_ID = Object_ID(N'dbo.SGT_BENEFIT_CALCULATION_PAYEE'))
BEGIN
    ALTER TABLE [dbo].[SGT_BENEFIT_CALCULATION_PAYEE]
	ALTER COLUMN [BENEFIT_PERCENTAGE] DECIMAL(11,3) NULL 
END
GO

/********************Purpose: PIR 26504 PIR 26504 System should generate the voya port file only for member whose coverage modified by batch id 25.******************************
*********************Created By: Abhijeet Malwadkar********************************/
/*---------------------------//----PIR 26504 Batch ID 145 selection-----//-------------------------------------------*/

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE [Name] = N'IS_END_DATED_DUE_TO_LOSS_OF_SUPP_LIFE' AND Object_ID = Object_ID(N'dbo.SGT_PERSON_ACCOUNT_LIFE_HISTORY'))
BEGIN
    ALTER TABLE SGT_PERSON_ACCOUNT_LIFE_HISTORY
	ADD IS_END_DATED_DUE_TO_LOSS_OF_SUPP_LIFE CHAR(1) NULL 
END
GO

/********************Purpose: PIR 27023******************************
*********************Created By: Sarvesh ********************************/
IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE [Name] = N'DC25_REMINDER_LETTER_FLAG' AND Object_ID = Object_ID(N'dbo.SGT_PERSON_ACCOUNT'))
BEGIN
    ALTER TABLE SGT_PERSON_ACCOUNT
ADD DC25_REMINDER_LETTER_FLAG CHAR NULL 
END
GO