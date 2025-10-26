/*Schema_Change_PIR_24060 */
/********************Purpose: PIR 24060 ******************************
*********************Created By: Nurul Gondane********************************
*********************Comments: Multiple screen changes*****************/
IF NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'SGT_DEPOSIT'
                 AND COLUMN_NAME = 'DEPOSIT_DATE') 
	ALTER TABLE SGT_DEPOSIT 
	ADD DEPOSIT_DATE DATETIME NULL
	
GO

/********************Purpose: PIR 24243******************************
*********************Created By: Vidya Fulsoundar********************************
*********************Comments: *****************/
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'SGT_PERSON_BASE'
AND COLUMN_NAME = 'Limit_401a')
ALTER TABLE SGT_PERSON_BASE
ADD Limit_401a char(1) null
GO

EXEC sys.sp_refreshview SGT_PERSON	
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'SGT_SERVICE_PURCHASE_HEADER'
AND COLUMN_NAME = 'OVERRIDDEN_FINAL_AVERAGE_SALARY')
ALTER TABLE SGT_SERVICE_PURCHASE_HEADER
ADD OVERRIDDEN_FINAL_AVERAGE_SALARY decimal(13, 2) null
GO
/********************Purpose: PIR 23999******************************
*********************Created By: Abhijeet Malwadkar********************************
*********************Comments: *****************/
IF NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'SGT_EMPLOYER_PAYROLL_HEADER'
                 AND COLUMN_NAME = 'ORIGINAL_EMPLOYER_PAYROLL_HEADER_ID') 
	ALTER TABLE SGT_EMPLOYER_PAYROLL_HEADER 
	ADD ORIGINAL_EMPLOYER_PAYROLL_HEADER_ID INT NULL
GO