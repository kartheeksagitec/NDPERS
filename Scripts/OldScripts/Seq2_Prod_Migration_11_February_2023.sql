/********************Purpose: PIR 25376******************************
*********************Created By: Bharat Reddy********************************
*********************Comments: *****************/

IF EXISTS (SELECT 1 FROM SGS_MESSAGES WHERE MESSAGE_ID=4743)
	UPDATE SGS_MESSAGES
    SET EMPLOYER_INSTRUCTIONS = 'Billing is based upon enrollment, if changes are necessary, contact NDPERS Benefits Division. If enrollment is correct and payroll isn''t, please pay the billed amount.'
	WHERE MESSAGE_ID = 4743
GO

/********************Purpose: PIR 25345******************************
*********************Created By: Bharat Reddy********************************
*********************Comments: *****************/
IF NOT EXISTS (SELECT 1 FROM SGT_WSS_AUTO_POSTING_CROSS_REF where PLAN_ID = 12 AND CHANGE_REASON_VALUE = 'EMCH')
BEGIN
INSERT INTO SGT_WSS_AUTO_POSTING_CROSS_REF VALUES(154, 12, 6003, 'ENRL', 332, 'EMCH', 6005, 'NOAP', 'Y', NULL, NULL, NULL)
END
GO

IF NOT EXISTS (SELECT 1 FROM SGT_WSS_AUTO_POSTING_CROSS_REF where PLAN_ID = 16 AND CHANGE_REASON_VALUE = 'EMCH')
BEGIN
INSERT INTO SGT_WSS_AUTO_POSTING_CROSS_REF VALUES(155, 16, 6003, 'ENRL', 332, 'EMCH', 6005, 'NOAP', 'Y', NULL, NULL, NULL)
END
GO

IF NOT EXISTS (SELECT 1 FROM SGT_WSS_AUTO_POSTING_CROSS_REF where PLAN_ID = 17 AND CHANGE_REASON_VALUE = 'EMCH')
BEGIN
INSERT INTO SGT_WSS_AUTO_POSTING_CROSS_REF VALUES(156, 17, 6003, 'ENRL', 332, 'EMCH', 6005, 'NOAP', 'Y', NULL, NULL, NULL)
END
GO

/********************Purpose: PIR 23183******************************
*********************Created By: Saylee P********************************
*********************Comments: *****************/

IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID=10463)
	INSERT INTO SGS_MESSAGES (MESSAGE_ID,DISPLAY_MESSAGE,SEVERITY_ID,SEVERITY_VALUE,INTERNAL_INSTRUCTIONS,EMPLOYER_INSTRUCTIONS,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
    VALUES(10463,'System is unable to calculate 3 consecutive 12 month periods. FAS must be calculated manually.',16,'E',NULL,NULL,'PIR 23183',GETDATE(),'PIR 23183',GETDATE(),0)
GO

/********************Purpose: PIR 25276******************************
*********************Created By: Saylee P********************************
*********************Comments: Run workflow to BPM utility*****************/

SELECT * FROM SGW_BPM_PROCESS WHERE [DESCRIPTION] LIKE '%Update%Information'
SELECT * FROM SGW_BPM_PROCESS_EVENT_XR WHERE PROCESS_ID IN (133, 142)


