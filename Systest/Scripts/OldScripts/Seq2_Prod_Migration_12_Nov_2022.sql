IF EXISTS (SELECT 1 FROM SGT_WSS_ACKNOWLEDGEMENT WHERE ACKNOWLEDGEMENT_ID=46)
BEGIN
UPDATE SGT_WSS_ACKNOWLEDGEMENT SET ACKNOWLEDGEMENT_TEXT ='I hereby forfeit insurance coverage at this time. I fully understand that if I or my Eligible Dependents desire to be covered under my employer''s insurance<br/> Benefit Plan in the future, I and my Eligible Dependents may have a Waiting Period for Preexisting Conditions and one of the following must apply:' WHERE ACKNOWLEDGEMENT_ID=46
END
GO


/********************Purpose: PIR 24727 ******************************
*********************Created By: Abhijeet Malwadkar********************************
*********************Comments: New Role for BPM ER Trainer *****************/

IF NOT EXISTS(SELECT 1 FROM SGS_ROLES WHERE ROLE_DESCRIPTION = 'ORG-Agent-ER-Trainer')
BEGIN

INSERT INTO SGS_ROLES (ROLE_DESCRIPTION,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
VALUES ('ORG-Agent-ER-Trainer','PIR 24727',GETDATE(),'PIR 24727',GETDATE(),0)

END

/********************Purpose: PIR 25212******************************
*********************Created By: Bharat Reddy********************************
*********************Comments: *****************/

IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID=10460)
	INSERT INTO SGS_MESSAGES (MESSAGE_ID,DISPLAY_MESSAGE,SEVERITY_ID,SEVERITY_VALUE,INTERNAL_INSTRUCTIONS,EMPLOYER_INSTRUCTIONS,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
    VALUES(10460,'Confirm correct multiplier for legislative increases',16,'E',NULL,NULL,'PIR 24212',GETDATE(),'PIR 25212',GETDATE(),0)
GO

/********************Purpose: PIR 25069******************************
*********************Created By: Vidya Fulsoundar********************************
*********************Comments: *****************/

IF NOT EXISTS (SELECT 1 FROM SGS_COR_TEMPLATES WHERE TEMPLATE_NAME='PER-0358')
BEGIN
	INSERT INTO SGS_COR_TEMPLATES VALUES('PER-0358',	'Missing Contributions',	19,	'MMBR',	'Y',	601,	NULL,	'wfmDeferredCompProviderMaintenance;',	NULL,	NULL,	NULL,	515,	NULL,	'N',	'Y',	'N',	44,	NULL,	'PIR 25069',	getdate(),	'PIR 25069',	getdate(),	0,	603,	'MEMB',	604,	'CORR',	NULL,	NULL)
END
GO


/********************Purpose: PIR 25183 ******************************
*********************Created By: Abhijeet Malwadkar********************************
*********************Comments:  *****************/
IF NOT EXISTS(SELECT 1 FROM SGW_BPM_SERVICE_TIMER WHERE ACTION_METHOD = 'CheckRequests')
BEGIN
INSERT INTO SGW_BPM_SERVICE_TIMER (ACTION_METHOD,INTERVAL,ENABLED,BPM_TRACING_FLAG,BPM_QUERY_TRACING_FLAG,BPM_QUERY_LIMIT,BPM_QUERY_LIMIT_ALERT,BPM_QUERY_HIGH_LIMIT,BPM_QUERY_HIGH_LIMIT_ALERT,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
VALUES ('CheckRequests',900000,'Y','Y','Y',1000,NULL,3000,NULL,'PIR 25183',GETDATE(),'PIR 25183',GETDATE(),0)
END

/********************Purpose: PIR 25221 ******************************
*********************Created By: Vidya********************************
*********************Comments:  *****************/
IF NOT EXISTS (SELECT * FROM SGS_MESSAGES WHERE MESSAGE_ID=10462)
	INSERT INTO SGS_MESSAGES (MESSAGE_ID,DISPLAY_MESSAGE,SEVERITY_ID,SEVERITY_VALUE,INTERNAL_INSTRUCTIONS,EMPLOYER_INSTRUCTIONS,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE,UPDATE_SEQ)
    VALUES(10462,'You Cannot Enroll In Flex Comp During This Plan Year.',16,'E',NULL,NULL,'PIR 25221',GETDATE(),'PIR 25221',GETDATE(),0)
GO