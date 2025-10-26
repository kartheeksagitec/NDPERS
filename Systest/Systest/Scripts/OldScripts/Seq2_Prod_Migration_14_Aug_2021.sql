/*Schema_Change_PIR_24075 */
/********************Purpose: PIR 24075 ******************************
*********************Created By: Vidya********************************
*********************Comments: *****************/
IF NOT EXISTS (SELECT 1 FROM SGS_CODE_VALUE WHERE [CODE_ID]=52 AND [CODE_VALUE] ='HERL')
BEGIN
INSERT INTO SGS_CODE_VALUE ([CODE_ID], [CODE_VALUE], [DESCRIPTION], [DATA1], [DATA2], [DATA3], [COMMENTS], [START_DATE], [END_DATE], [CODE_VALUE_ORDER], [LEGACY_CODE_ID], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES(52,	'HERL',	'https://prd.hcm.ndus.edu/psp/hehp/?cmd=login',	NULL,	NULL,	NULL,	'People Soft HIED URL',	NULL,	NULL,	NULL,	NULL,	'PIR 24075', GETDATE(),	'PIR 24075',	GETDATE(),	0)
END
GO

/*Schema_Change_PIR_23963 */
/********************Purpose: PIR 23963 ******************************
*********************Created By: Vidya********************************
*********************Comments: *****************/

IF NOT EXISTS (SELECT 1 FROM SGS_MESSAGES WHERE MESSAGE_ID = 10419)
BEGIN
INSERT INTO SGS_MESSAGES ([MESSAGE_ID], [DISPLAY_MESSAGE], [SEVERITY_ID], [SEVERITY_VALUE], [INTERNAL_INSTRUCTIONS], [EMPLOYER_INSTRUCTIONS], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES(10419,'Date of Last Regular Paycheck should be within 6 months of Last Date of Service with Agency',	16,	'E','','', 'PIR 23963', GETDATE(),	'PIR 23963',	GETDATE(),	0)
END
GO
