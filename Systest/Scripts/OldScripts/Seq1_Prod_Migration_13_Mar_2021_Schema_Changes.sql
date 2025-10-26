/********************Purpose: PIR 23631 PS Batch 134******************************
*********************Created By: Saylee P********************************
*********************Comments: Added new Error Column to PS tables *****************/

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'SGT_PS_PERSON'
                 AND COLUMN_NAME = 'ERROR')
ALTER TABLE SGT_PS_PERSON
ADD ERROR VARCHAR(1) DEFAULT 'N'

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'SGT_PS_ADDRESS'
                 AND COLUMN_NAME = 'ERROR')
ALTER TABLE SGT_PS_ADDRESS
ADD ERROR VARCHAR(1) DEFAULT 'N'

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'SGT_PS_EMPLOYMENT'
                 AND COLUMN_NAME = 'ERROR')
ALTER TABLE SGT_PS_EMPLOYMENT
ADD ERROR VARCHAR(1) DEFAULT 'N'

/*Schema_Change_PIR_23557 */
/********************Purpose: PIR 23557 Update MSS flex comp wizard ******************************
*********************Created By: Abhijeet********************************
*********************Comments: *****************/
IF NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'SGT_WSS_PERSON_ACCOUNT_FLEX_COMP_OPTION'
                 AND COLUMN_NAME = 'UPLA_OPTIONS') 
	ALTER TABLE SGT_WSS_PERSON_ACCOUNT_FLEX_COMP_OPTION 
	ADD UPLA_OPTIONS CHAR(1) NULL
GO
