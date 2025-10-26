/********************Purpose: PIR 26139******************************
*********************Created By: Bharat Reddy********************************
*********************Comments: Added new column CARE_OF*****************/

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE [Name] = N'CARE_OF' AND Object_ID = Object_ID(N'dbo.SGT_PERSON_ADDRESS'))
BEGIN
    ALTER TABLE [DBO].[SGT_PERSON_ADDRESS]
	ADD CARE_OF VARCHAR(100) NULL 
END
GO
