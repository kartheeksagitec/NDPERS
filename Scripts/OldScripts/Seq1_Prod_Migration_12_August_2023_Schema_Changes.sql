/********************Purpose: PIR 25729******************************
*********************Created By: Vidya Fulsoundar********************************
*********************Comments: *****************/

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE [Name] = N'BENEFIT_MULTIPLIER_TYPE_ID' AND Object_ID = Object_ID(N'dbo.SGT_BENEFIT_PROVISION_MULTIPLIER'))
BEGIN
    ALTER TABLE SGT_BENEFIT_PROVISION_MULTIPLIER
	ADD BENEFIT_MULTIPLIER_TYPE_ID INT NULL 
END

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE [Name] = N'BENEFIT_MULTIPLIER_TYPE_VALUE' AND Object_ID = Object_ID(N'dbo.SGT_BENEFIT_PROVISION_MULTIPLIER'))
BEGIN
    ALTER TABLE SGT_BENEFIT_PROVISION_MULTIPLIER
	ADD BENEFIT_MULTIPLIER_TYPE_VALUE VARCHAR(4) NULL
END