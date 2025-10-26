/*Schema_Change_PIR_24036 */
/********************Purpose: PIR 24036 ******************************
*********************Created By: Vidya********************************
*********************Comments: *****************/
IF NOT EXISTS (select 1 from SGS_MESSAGES where [MESSAGE_ID] = 10417)
BEGIN
INSERT INTO SGS_MESSAGES ([MESSAGE_ID],[DISPLAY_MESSAGE], [SEVERITY_ID], [SEVERITY_VALUE], [INTERNAL_INSTRUCTIONS], [EMPLOYER_INSTRUCTIONS], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES(10417,'Retirement effective date cannot be a past date.',	16,	'E',	NULL,	NULL,	'PIR-24036' ,	GETDATE(),	'PIR-24036', 	GETDATE(),	0)
END
GO

/*Data_Changes_PIR 23338*/
/********************Purpose: PIR 23338******************************
*********************Created By: Vidya********************************
*********************Comments: *****************/

IF NOT EXISTS (SELECT 1 FROM SGS_CODE_VALUE WHERE CODE_ID = 13 and CODE_VALUE='REBP')
	INSERT [SGS_CODE_VALUE] ([CODE_ID], [CODE_VALUE], [DESCRIPTION], [DATA1], [DATA2], 
							[DATA3], [COMMENTS], [START_DATE], [END_DATE], [CODE_VALUE_ORDER],[LEGACY_CODE_ID],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ]) 
	VALUES (13,'REBP', 'Ready for Batch Printing',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL, 'PIR 23338', GETDATE(), 'PIR 23338',GETDATE(), 0)
GO

IF NOT EXISTS (SELECT 1 FROM [SGS_BATCH_SCHEDULE] WHERE BATCH_SCHEDULE_ID = 152)
INSERT INTO [SGS_BATCH_SCHEDULE] (BATCH_SCHEDULE_ID,	STEP_NO,	STEP_NAME,	STEP_DESCRIPTION,	FREQUENCY_IN_DAYS,	FREQUENCY_IN_MONTHS,	NEXT_RUN_DATE,	STEP_PARAMETERS,	ACTIVE_FLAG,	REQUIRES_TRANSACTION_FLAG,	EMAIL_NOTIFICATION,	ORDER_NO,	CUTOFF_START,	CUTOFF_END,	CREATED_BY,	CREATED_DATE,	MODIFIED_BY,	MODIFIED_DATE,	UPDATE_SEQ)
VALUES(152,	621,'Printing Batch Letters',	NULL,	1,	NULL,	GETDATE(),	NULL,	'Y','N',	NULL,	NULL,	NULL,	NULL,	'PIR 23338',GETDATE(),	'PIR 23338',	GETDATE(),	0)
GO

/********************Purpose: PIR 23338  New column Batch Print Flag ******************************
*********************Created By: Abhijeet Malwadkar********************************
*********************Comments: New column Batch Print Flag *****************/

IF NOT EXISTS (SELECT 1 FROM [dbo].[SGS_MESSAGES] WHERE MESSAGE_ID = 10415)
BEGIN
INSERT INTO SGS_MESSAGES([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[INTERNAL_INSTRUCTIONS],[EMPLOYER_INSTRUCTIONS],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES (10415,'Selecting Batch Print is only allowed when Auto Print is also selected.',16,'W',NULL,NULL,'PIR23338',GETDATE(),'PIR23338',GETDATE(),0)
END
GO 


/********************Purpose: PIR 18493  ******************************
*********************Created By: Abhijeet Malwadkar ********************************
*********************Comments: Added new Message *****************/

IF NOT EXISTS (SELECT 1 FROM [dbo].[SGS_MESSAGES] WHERE MESSAGE_ID = 10416)
BEGIN
INSERT INTO SGS_MESSAGES([MESSAGE_ID],[DISPLAY_MESSAGE],[SEVERITY_ID],[SEVERITY_VALUE],[INTERNAL_INSTRUCTIONS],[EMPLOYER_INSTRUCTIONS],[CREATED_BY],[CREATED_DATE],[MODIFIED_BY],[MODIFIED_DATE],[UPDATE_SEQ])
VALUES (10416,'Percentage is required',16,'E',NULL,NULL,'PIR18493',GETDATE(),'PIR18493',GETDATE(),0)
END
GO 

/*Data_Changes_PIR_23878*/
/********************Purpose: PIR 23878******************************
*********************Created By: Vidya********************************
*********************Comments: *****************/

IF NOT EXISTS (select 1 from SGS_MESSAGES where [MESSAGE_ID] = 10414)
BEGIN
INSERT INTO SGS_MESSAGES ([MESSAGE_ID],[DISPLAY_MESSAGE], [SEVERITY_ID], [SEVERITY_VALUE], [INTERNAL_INSTRUCTIONS], [EMPLOYER_INSTRUCTIONS], [CREATED_BY], [CREATED_DATE], [MODIFIED_BY], [MODIFIED_DATE], [UPDATE_SEQ])
VALUES(10414,'Person Employment End Date is greater than 12 months from System Management Date.',	16,	'E',	NULL,	NULL,	'PIR 23878' ,	GETDATE(),	'PIR 23878', 	GETDATE(),	0)
END
GO
