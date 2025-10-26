/*Schema_Change_PIR_11281 */
/********************Purpose: PIR 11281 ******************************
*********************Created By: Vidya********************************
*********************Comments: *****************/
IF NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE [TABLE_NAME] = 'SGT_REMITTANCE' AND [COLUMN_NAME] = 'REFUND_NOTES')
BEGIN
ALTER TABLE SGT_REMITTANCE
ADD REFUND_NOTES VARCHAR(1000) NULL
END
GO


/********************Purpose: PIR 23338  New column Batch Print Flag ******************************
*********************Created By: Abhijeet Malwadkar********************************
*********************Comments: New column Batch Print Flag *****************/
IF NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'SGS_COR_TEMPLATES'
                 AND COLUMN_NAME = 'BATCH_PRINT_FLAG') 
	ALTER TABLE SGS_COR_TEMPLATES 
	ADD BATCH_PRINT_FLAG CHAR(1) NULL
GO

/********************Purpose: PIR 24046  New column to store last regulare paycheck date ******************************
*********************Created By: Abhijeet Malwadkar********************************
*********************Comments: New column to store last regulare paycheck date *****************/
IF NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'SGT_PERSON_EMPLOYMENT'
                 AND COLUMN_NAME = 'DATE_OF_LAST_REGULAR_PAYCHECK') 
	ALTER TABLE SGT_PERSON_EMPLOYMENT 
	ADD DATE_OF_LAST_REGULAR_PAYCHECK Datetime NULL
GO