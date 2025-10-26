
/*Schema_Change_PIR_23829 */
/********************Purpose: PIR 21588 ******************************
*********************Created By: Abhijeet********************************
*********************Comments: *****************/
IF NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'SGT_EMPLOYER_PAYROLL_DETAIL'
                 AND COLUMN_NAME = 'PLAN_ID_ORIGINAL') 
	ALTER TABLE SGT_EMPLOYER_PAYROLL_DETAIL 
	ADD PLAN_ID_ORIGINAL INT NULL
GO
