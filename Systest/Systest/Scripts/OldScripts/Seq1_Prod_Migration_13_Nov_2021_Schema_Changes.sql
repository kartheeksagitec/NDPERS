/********************Purpose: PIR 24340******************************
*********************Created By: Vidya Fulsoundar********************************
*********************Comments: *****************/
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'SGT_PERSON_DEPENDENT'
AND COLUMN_NAME = 'GUARDIANSHIP_EXPIRATION_DATE')
ALTER TABLE SGT_PERSON_DEPENDENT
ADD GUARDIANSHIP_EXPIRATION_DATE datetime null
GO

/********************Purpose: PIR 18493******************************
*********************Created By: Abhijeet Malwadkar********************************
*********************Comments: *****************/
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'SGT_WSS_BEN_APP_ROLLOVER_DETAIL'
AND COLUMN_NAME = 'FOREIGN_PROVINCE')
	ALTER TABLE SGT_WSS_BEN_APP_ROLLOVER_DETAIL
	ADD FOREIGN_PROVINCE varchar(50) null
GO
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'SGT_WSS_BEN_APP_ROLLOVER_DETAIL'
AND COLUMN_NAME = 'FOREIGN_POSTAL_CODE')
	ALTER TABLE SGT_WSS_BEN_APP_ROLLOVER_DETAIL
	ADD FOREIGN_POSTAL_CODE varchar(10) null
GO
