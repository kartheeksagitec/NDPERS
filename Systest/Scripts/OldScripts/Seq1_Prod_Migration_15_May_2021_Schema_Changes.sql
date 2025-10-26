
/*Schema_Change_PIR_23829 */
/********************Purpose: PIR 23829 Add another field REFERENT_ID to the PIR process for Notification ******************************
*********************Created By: Abhijeet********************************
*********************Comments: *****************/
IF NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'SGS_PIR'
                 AND COLUMN_NAME = 'REFERENT_ID') 
	ALTER TABLE SGS_PIR 
	ADD REFERENT_ID INT NULL
GO



/*Schema_Change_PIR_23439_&_23392 */
/********************Purpose: PIR 23439 & 23392******************************
*********************Created By: Saylee P********************************
*********************Comments: *****************/

IF NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'SGT_DEATH_NOTICE'
                 AND COLUMN_NAME = 'DEATH_ALREADY_REPORTED') 
	ALTER TABLE SGT_DEATH_NOTICE 
	ADD DEATH_ALREADY_REPORTED VARCHAR(1) NULL
GO

IF NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'SGT_APPOINTMENT_SCHEDULE'
                 AND COLUMN_NAME = 'NO_APPOINTMENT_NECESSARY') 
	ALTER TABLE SGT_APPOINTMENT_SCHEDULE 
	ADD NO_APPOINTMENT_NECESSARY VARCHAR(1) NULL
GO